using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵を追跡し、ゴール方向に向かっている敵を識別するクラス。
/// 指定したゴールオブジェクトの位置を基準に、
/// トリガーに入った敵がゴールに向かっているかどうかを判定する。
/// </summary>
public class EnemyTracker : MonoBehaviour
{
    [SerializeField] private GameObject _goalPrefab;                   // ゴール地点のプレハブ
    [SerializeField] private bool _changeAxis = false;                 // false = X軸を基準, true = Y軸を基準

    private TowerAttack _towerAttack;
    private List<GameObject> _allEnemies = new List<GameObject>();     // 検出された全ての敵
    private List<GameObject> _targetEnemies = new List<GameObject>();  // ゴール方向に向かっている敵
    private Vector2Int _goalPosition;                                  // ゴールの座標
    private Vector2Int _myPosition;                                    // このオブジェクトの座標
    private int _goalDirection;                                        // ゴールへの方向（1 or -1）

    /// <summary>
    /// 現在トラッキングしている全ての敵リストを取得。
    /// </summary>
    public List<GameObject> GetAllEnemies => _allEnemies;

    /// <summary>
    /// ゴールに向かっている敵のリストを取得。
    /// </summary>
    public List<GameObject> GetTargetEnemies => _targetEnemies;

    /// <summary>
    /// ゲーム内で敵を識別するタグを管理するクラス。
    /// </summary>
    public static class Tags
    {
        public static string Enemy = "Enemy";
    }

    /// <summary>
    /// 初期化処理。ゴールの位置と自身の位置を取得し、
    /// ゴールへの方向を計算する。
    /// </summary>
    private void Start()
    {
        _towerAttack = GetComponent<TowerAttack>();

        Transform goalTransform = _goalPrefab.transform;
        _goalPosition = new Vector2Int(Mathf.FloorToInt(goalTransform.position.x), Mathf.FloorToInt(goalTransform.position.y));
        _myPosition = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));

        // ゴールがどの方向にあるかを決定（X軸 or Y軸）
        _goalDirection = _changeAxis
            ? (_goalPosition.y > _myPosition.y ? 1 : -1) // Y軸の方向判定
            : (_goalPosition.x > _myPosition.x ? 1 : -1); // X軸の方向判定

    }

    /// <summary>
    /// トリガーに敵が入ったときに呼ばれる。
    /// 敵をリストに追加し、ゴール方向に向かっているか判定。
    /// </summary>
    /// <param name="other">トリガーに入ったオブジェクト</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Tags.Enemy) && !_allEnemies.Contains(other.gameObject))
        {
            _allEnemies.Add(other.gameObject);
            CheckEnemyDirection(other.gameObject);
        }
    }

    /// <summary>
    /// トリガーから敵が出たときに呼ばれる。
    /// 敵をリストから削除。
    /// </summary>
    /// <param name="other">トリガーから出たオブジェクト</param>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(Tags.Enemy))
        {
            _allEnemies.Remove(other.gameObject);
            _targetEnemies.Remove(other.gameObject);
        }
    }

    /// <summary>
    /// 毎フレーム、トラッキングしている敵が
    /// ゴール方向に向かっているかをチェック。
    /// </summary>
    private void Update()
    {
        for (int i = _allEnemies.Count - 1; i >= 0; i--)
        {
            CheckEnemyDirection(_allEnemies[i]);
        }
    }

    /// <summary>
    /// 敵がゴール方向に向かっているかを判定。
    /// </summary>
    /// <param name="enemy">チェックする敵のGameObject</param>
    private void CheckEnemyDirection(GameObject enemy)
    {
        // 敵が破棄されていたら処理を終了
        if (enemy == null)
        {
            return;
        }


        Vector2Int enemyPosition = new Vector2Int(Mathf.FloorToInt(enemy.transform.position.x), Mathf.FloorToInt(enemy.transform.position.y));

        // 敵がゴール方向にいるかどうかを判定
        bool isInGoalDirection = _changeAxis
            ? (enemyPosition.y - _myPosition.y) * _goalDirection > 0 // Y軸で判定
            : (enemyPosition.x - _myPosition.x) * _goalDirection > 0; // X軸で判定

        if (isInGoalDirection)
        {
            // まだリストに入っていなければ追加
            if (!_targetEnemies.Contains(enemy))
            {
                _targetEnemies.Add(enemy);
                _towerAttack.SelectNewTarget();
            }
        }
        else
        {
            // ゴール方向にいなくなった場合はリストから削除
            _targetEnemies.Remove(enemy);
            _towerAttack.SelectNewTarget();
        }
    }
}
