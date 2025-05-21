using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveUpdater : MonoBehaviour
{
    #region Fields
    [SerializeField] Transform _player;                             // プレイヤーのTransform
    [SerializeField] private MazeGenerator _mazeGenerator;         // GridGenerator をアタッチして格納
    [SerializeField] private float _detectionRange = default;       // 視野内の範囲
    [SerializeField] private float _viewAngle = default;            // 視野角
    [SerializeField] private float _slowSpeed = default;           // 移動スピードが遅くなるときのスピード
    [SerializeField] private float _moveSpeed = default;           // 移動スピード
    private EnemyEscapeMove _escapeMove;
    private EnemyNormalMove _normalMove;
    private const float _halfAngleDivisor = 2f;
    #endregion

    void Start()
    {
        //Scriptの取得
        _escapeMove = GetComponent<EnemyEscapeMove>();
        _normalMove = GetComponent<EnemyNormalMove>();
        //エネミーの移動スピードの設定
        _normalMove.MoveSpeed = _moveSpeed;
        _escapeMove.MoveSpeed = _moveSpeed;

        // _escapeMove にプレイヤー情報を渡す
        if (_player != null)
        {
            _escapeMove.SetPlayer(_player);
        }
    }

    void Update()
    {
            // プレイヤーを発見したらエスケープ、それ以外なら自由移動
            if (IsPlayerInSight())
            {
                SetEscape(true);
            }
            else
            {
                SetFreeMove(true);
            }
        
    }
    /// <summary>
    /// 逃走Scriptに切り替え
    /// </summary>
    /// <param name="active">起動しているか</param>
    public void SetEscape(bool active)
    {
        _escapeMove.enabled = active;
        _normalMove.enabled = !active;
    }
    /// <summary>
    /// 通常移動Scriptに切り替え
    /// </summary>
    /// <param name="active">起動しているか</param>
    public void SetFreeMove(bool active)
    {
        _normalMove.enabled = active;
        _escapeMove.enabled = !active;
       
    }

    /// <summary>
    /// 手加減モードの内容
    /// </summary>
    public void LaxityMode()
    {
        // 移動速度を低下
        _normalMove.MoveSpeed = _slowSpeed;
        _escapeMove.MoveSpeed = _slowSpeed;

    }
    /// <summary>
    /// 視野内にプレイヤーが居るかの判断
    /// </summary>
    /// <returns>視野内に敵かいるかどうか</returns>
    public bool IsPlayerInSight()
    {
        Vector3 directionToPlayer = _player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > _detectionRange)
            return false;

        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer.normalized);
        if (angleToPlayer > _viewAngle / _halfAngleDivisor)
            return false;

        return true;
    }

    /// <summary>
    /// デバッグ用に視野範囲をGizmosで可視化
    /// </summary>
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }
}
