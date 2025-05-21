using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine;

public class DeleteTower : MonoBehaviour
{
    [SerializeField] Tilemap _targetTileMap;    //選択できるタイルの種類
    private HashSet<Vector3Int> _placedTiles;   //タワーが設置されているマス
    private List<GameObject> _placedObjects;    //設置されているタワー
    private CreateTower _createTower;           //CreateTowerの取得
    private const float _judgmentRange = 0.3f;  //判定範囲の調整

    public static class Tags
    {
        public const string PlacedTower = "PlacedTower";
    }

    void Start()
    {
        _createTower = GetComponent<CreateTower>();
        if(_createTower != null)
        {
            _placedTiles = _createTower.GetPlacedTiles();
            _placedObjects = _createTower.GetPlacedObject();
        }
    }

    void Update()
    {
        //右クリックするとタワーの削除の処理を行う
        if(Input.GetMouseButtonDown(1))
        {
            Vector3Int cellPosition = GetMouseTilePosition();
            if(_placedTiles != null && _placedTiles.Contains(cellPosition))
            {
                RemoveObject(cellPosition);
            }
        }
    }
    /// <summary>
    /// マスに置いているタワーを削除する
    /// </summary>
    /// <param name="cellPosition">タワーが設置されている位置</param>
    private void RemoveObject(Vector3Int cellPosition)
    {
        Vector3 worldPosition = _targetTileMap.GetCellCenterWorld(cellPosition);

        //worldPositionを中心に_judgementRangeのサイズの円を作り、その円内のコライダーを取得
        Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPosition, _judgmentRange);
        Collider2D closestCollider = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D collider in colliders)
        {
            // タグが一致するオブジェクトのみ処理
            if (collider.CompareTag(Tags.PlacedTower))
            {
                float distance = Vector2.Distance(worldPosition, collider.bounds.center);
                // 最も近いオブジェクトを探す
                if (distance < closestDistance)  
                {
                    closestDistance = distance;
                    closestCollider = collider;
                }
            }
        }

        // 削除対象が見つかった場合のみ削除
        if (closestCollider != null) 
        {
            _placedObjects.Remove(closestCollider.gameObject);
            Destroy(closestCollider.gameObject);
            _placedTiles.Remove(cellPosition);
            _createTower.Reinstallable(cellPosition);
        }
    }
    /// <summary>
    /// マウスカーソルの位置を取る
    /// </summary>
    /// <returns></returns>
    Vector3Int GetMouseTilePosition()
    {
        Vector3 mouseworldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseworldPos.z = 0;
        return _targetTileMap.WorldToCell(mouseworldPos);
    }
}
