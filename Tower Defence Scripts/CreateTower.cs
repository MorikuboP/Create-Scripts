using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Collections.Generic;

public class CreateTower : MonoBehaviour
{
    [SerializeField] private Tilemap _targetTilemap; // 設置可能なタイルマップ
    [SerializeField] private Tilemap _backgroundTilemap; // 背景用タイルマップ（設置不可）
    [SerializeField] private GameObject[] _placeableObjects; // 設置可能なオブジェクト
    [SerializeField] private Button[] _sideBarButton; // Sidebarのボタン
    [SerializeField] private Slider _MPSlider;
    [SerializeField] private Text _towerCountText;
    [SerializeField] private int[] _objectCost;
    [SerializeField] private int _maxTower;
    private TowerAttack _towerAttack;
    private float _costRecoverRate = 5f;
    private float _maxCost = 100f;
    private GameObject _selectedObject; // 選択中のオブジェクト
    private GameObject _previewObject; // 仮配置オブジェクト
    private bool IsPlacing = false; // 設置中フラグ
    private HashSet<Vector3Int> _placedTiles = new HashSet<Vector3Int>();
    private List<GameObject> _placedObjects = new List<GameObject>();

    public static class Tags
    {
        public const string ReCoverCost = "ReCoverCost";
    }

    public HashSet<Vector3Int> GetPlacedTiles()
    {
        return _placedTiles;
    }

    public List<GameObject> GetPlacedObject()
    {
        return _placedObjects;
    }

    void Start()
    {
        // ボタンにタワー選択機能を紐づける
        for (int i = 0; i < _sideBarButton.Length; i++)
        {
            // ラムダ式用のローカル変数
            int index = i;
            _sideBarButton[i].onClick.AddListener(() => SelectObject(index));
        }
        // MPスライダーの初期化
        _MPSlider.maxValue = _maxCost;
        _MPSlider.value = _maxCost;

        // MP回復を定期的に実行
        InvokeRepeating(Tags.ReCoverCost, 1f, 1f);

        // タワー数表示更新
        UpdateTowerCountText();
    }

    void Update()
    {
        // 設置中でない場合は処理しない
        if (!IsPlacing || _selectedObject == null) return;

        // マウスの位置を取得し、タイル座標に変換
        Vector3Int cellPosition = GetMouseTilePosition();

        // 設置可能なタイルか判定
        if (!IsPlaceableTile(cellPosition)) return;

        // ワールド座標を取得
        Vector3 worldPosition = _targetTilemap.GetCellCenterWorld(cellPosition);

        // プレビューオブジェクトの更新
        if (_previewObject == null)
        {
            _previewObject = Instantiate(_selectedObject);
            SetPreviewMode(_previewObject, true);
        }
        _previewObject.transform.position = worldPosition;

        // 左クリックで設置確定
        if (Input.GetMouseButtonDown(0))
        {
            // タワー設置上限チェック
            if (_placedObjects.Count >= _maxTower)
            {
                return;
            }

            int cost = _objectCost[System.Array.IndexOf(_placeableObjects, _selectedObject)];
            if (_MPSlider.value >= cost)
            {
                GameObject tower = Instantiate(_selectedObject, worldPosition, Quaternion.identity);
                tower.tag = "PlacedTower";
                _placedTiles.Add(cellPosition);
                _placedObjects.Add(tower);

                // タワーの攻撃を有効化
                _towerAttack = tower.GetComponent<TowerAttack>();
                if (_towerAttack != null)
                {
                    _towerAttack.ShootingFlag = true;
                }

                // エフェクト再生
                ParticleSystem particle = tower.GetComponentInChildren<ParticleSystem>();
                if (particle != null)
                {
                    particle.Play();
                }

                _MPSlider.value -= cost;
                UpdateTowerCountText();
            }
        }

        // 右クリックで設置キャンセル
        if (Input.GetMouseButtonDown(1))
        {
            IsPlacing = false;
            Destroy(_previewObject);
        }
    }

    /// <summary>
    /// タワーを選択する処理
    /// </summary>
    /// <param name="index"></param>
    void SelectObject(int index)
    {
        _selectedObject = _placeableObjects[index];
        IsPlacing = true;

        if (_previewObject != null)
        {
            Destroy(_previewObject);
        }
        _previewObject = Instantiate(_selectedObject);
        SetPreviewMode(_previewObject, true);
    }

    /// <summary>
    ///  タイル座標を取得
    /// </summary>
    /// <returns></returns>
    Vector3Int GetMouseTilePosition()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0; // 2DなのでZ座標を0に固定
        return _targetTilemap.WorldToCell(mouseWorldPos);
    }

    /// <summary>
    ///  設置可能かチェック
    /// </summary>
    /// <param name="cellPosition">設置するセルのポジション</param>
    /// <returns></returns>
    bool IsPlaceableTile(Vector3Int cellPosition)
    {
        if (_backgroundTilemap.HasTile(cellPosition) || !_targetTilemap.HasTile(cellPosition) || _placedTiles.Contains(cellPosition))
        {
            return false;
        }
        return true;
    }

     /// <summary>
     /// プレビューの設定
     /// </summary>
     /// <param name="obj">選択したタワー</param>
     /// <param name="isPreview">プレビュー状態かのフラグ</param>
    void SetPreviewMode(GameObject obj, bool isPreview)
    {
        _towerAttack = obj.GetComponent<TowerAttack>();
        if (_towerAttack != null)
        {
            _towerAttack.ShootingFlag = false;
        }

        SpriteRenderer spriterenderer = obj.GetComponent<SpriteRenderer>();
        if (spriterenderer != null)
        {
            spriterenderer.color = isPreview ? new Color(1, 1, 1, 0.5f) : Color.white;
        }
    }
    /// <summary>
    /// MPゲージを徐々に回復する
    /// </summary>
    private void ReCoverCost()
    {
        _MPSlider.value = Mathf.Min(_MPSlider.value + _costRecoverRate, _maxCost);
    }

    /// <summary>
    /// タワーを設置したタイルのリストからタワーを削除したタイルをリストから削除する
    /// </summary>
    /// <param name="cellPosition">タイルの位置</param>
    public void Reinstallable(Vector3Int cellPosition)
    {
        _placedTiles.Remove(cellPosition);
        UpdateTowerCountText();
    }

    /// <summary>
    /// タワーの現在設置数を更新する
    /// </summary>
    private void UpdateTowerCountText()
    {
        if (_towerCountText != null)
        {
            _towerCountText.text = $"タワー数:{_placedObjects.Count}/{_maxTower}";
        }
    }
}
