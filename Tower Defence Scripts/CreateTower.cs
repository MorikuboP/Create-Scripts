using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Collections.Generic;

public class CreateTower : MonoBehaviour
{
    [SerializeField] private Tilemap _targetTilemap; // �ݒu�\�ȃ^�C���}�b�v
    [SerializeField] private Tilemap _backgroundTilemap; // �w�i�p�^�C���}�b�v�i�ݒu�s�j
    [SerializeField] private GameObject[] _placeableObjects; // �ݒu�\�ȃI�u�W�F�N�g
    [SerializeField] private Button[] _sideBarButton; // Sidebar�̃{�^��
    [SerializeField] private Slider _MPSlider;
    [SerializeField] private Text _towerCountText;
    [SerializeField] private int[] _objectCost;
    [SerializeField] private int _maxTower;
    private TowerAttack _towerAttack;
    private float _costRecoverRate = 5f;
    private float _maxCost = 100f;
    private GameObject _selectedObject; // �I�𒆂̃I�u�W�F�N�g
    private GameObject _previewObject; // ���z�u�I�u�W�F�N�g
    private bool IsPlacing = false; // �ݒu���t���O
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
        // �{�^���Ƀ^���[�I���@�\��R�Â���
        for (int i = 0; i < _sideBarButton.Length; i++)
        {
            // �����_���p�̃��[�J���ϐ�
            int index = i;
            _sideBarButton[i].onClick.AddListener(() => SelectObject(index));
        }
        // MP�X���C�_�[�̏�����
        _MPSlider.maxValue = _maxCost;
        _MPSlider.value = _maxCost;

        // MP�񕜂����I�Ɏ��s
        InvokeRepeating(Tags.ReCoverCost, 1f, 1f);

        // �^���[���\���X�V
        UpdateTowerCountText();
    }

    void Update()
    {
        // �ݒu���łȂ��ꍇ�͏������Ȃ�
        if (!IsPlacing || _selectedObject == null) return;

        // �}�E�X�̈ʒu���擾���A�^�C�����W�ɕϊ�
        Vector3Int cellPosition = GetMouseTilePosition();

        // �ݒu�\�ȃ^�C��������
        if (!IsPlaceableTile(cellPosition)) return;

        // ���[���h���W���擾
        Vector3 worldPosition = _targetTilemap.GetCellCenterWorld(cellPosition);

        // �v���r���[�I�u�W�F�N�g�̍X�V
        if (_previewObject == null)
        {
            _previewObject = Instantiate(_selectedObject);
            SetPreviewMode(_previewObject, true);
        }
        _previewObject.transform.position = worldPosition;

        // ���N���b�N�Őݒu�m��
        if (Input.GetMouseButtonDown(0))
        {
            // �^���[�ݒu����`�F�b�N
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

                // �^���[�̍U����L����
                _towerAttack = tower.GetComponent<TowerAttack>();
                if (_towerAttack != null)
                {
                    _towerAttack.ShootingFlag = true;
                }

                // �G�t�F�N�g�Đ�
                ParticleSystem particle = tower.GetComponentInChildren<ParticleSystem>();
                if (particle != null)
                {
                    particle.Play();
                }

                _MPSlider.value -= cost;
                UpdateTowerCountText();
            }
        }

        // �E�N���b�N�Őݒu�L�����Z��
        if (Input.GetMouseButtonDown(1))
        {
            IsPlacing = false;
            Destroy(_previewObject);
        }
    }

    /// <summary>
    /// �^���[��I�����鏈��
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
    ///  �^�C�����W���擾
    /// </summary>
    /// <returns></returns>
    Vector3Int GetMouseTilePosition()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0; // 2D�Ȃ̂�Z���W��0�ɌŒ�
        return _targetTilemap.WorldToCell(mouseWorldPos);
    }

    /// <summary>
    ///  �ݒu�\���`�F�b�N
    /// </summary>
    /// <param name="cellPosition">�ݒu����Z���̃|�W�V����</param>
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
     /// �v���r���[�̐ݒ�
     /// </summary>
     /// <param name="obj">�I�������^���[</param>
     /// <param name="isPreview">�v���r���[��Ԃ��̃t���O</param>
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
    /// MP�Q�[�W�����X�ɉ񕜂���
    /// </summary>
    private void ReCoverCost()
    {
        _MPSlider.value = Mathf.Min(_MPSlider.value + _costRecoverRate, _maxCost);
    }

    /// <summary>
    /// �^���[��ݒu�����^�C���̃��X�g����^���[���폜�����^�C�������X�g����폜����
    /// </summary>
    /// <param name="cellPosition">�^�C���̈ʒu</param>
    public void Reinstallable(Vector3Int cellPosition)
    {
        _placedTiles.Remove(cellPosition);
        UpdateTowerCountText();
    }

    /// <summary>
    /// �^���[�̌��ݐݒu�����X�V����
    /// </summary>
    private void UpdateTowerCountText()
    {
        if (_towerCountText != null)
        {
            _towerCountText.text = $"�^���[��:{_placedObjects.Count}/{_maxTower}";
        }
    }
}
