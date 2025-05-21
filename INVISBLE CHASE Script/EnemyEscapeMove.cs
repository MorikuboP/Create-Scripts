using UnityEngine.AI;
using UnityEngine;

public class EnemyEscapeMove : MonoBehaviour
{
    #region Fields
    [SerializeField] private float escapeDistance = default; // プレイヤーからどれだけ離れたいか
    [SerializeField] private float checkInterval = default; // 何秒ごとに経路更新するか
    private NavMeshAgent _agent;
    private Transform _player;
    private float _timer = 0f;
    private float _moveSpeed;
    private const float _hitAreaDistance = 2f;
    private const float _randomPositionMove = 3f;
    private const float _enemyRotationSpeed = 100f;
    #endregion

    #region Property
    public float MoveSpeed
    {
        get { return _moveSpeed; }
        set { _moveSpeed = value; }
    }
    #endregion

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (_player == null) return;

        // 一定間隔で逃げる方向を更新
        _timer += Time.deltaTime;
        if (_timer >= checkInterval)
        {
            _timer = 0f;
            EscapeFromPlayer();
        }

       
    }

    private void FixedUpdate()
    {
        // プレイヤーの方向を向く
        LookAwayFromPlayer();
    }
    /// <summary>
    /// プレイヤーを設定する
    /// </summary>
    /// <param name="player">エネミーが逃げる対象</param>
    public void SetPlayer(Transform player)
    {
        _player = player;
    }
    /// <summary>
    /// プレイヤーと反対方向に進みながら逃走先を設定する
    /// </summary>
    void EscapeFromPlayer()
    {
        Vector3 directionToPlayer = _player.position - transform.position; // プレイヤーへのベクトル
        Vector3 escapeTarget = transform.position - directionToPlayer.normalized * escapeDistance; // 反対方向へ移動

        // NavMesh上の移動可能な位置を探す
        NavMeshHit hit;
        if (NavMesh.SamplePosition(escapeTarget, out hit, _hitAreaDistance, NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position);
        }
        else
        {
            // スタックを避けるために周囲に少しランダムに逃げる
            Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * _randomPositionMove; // ランダムなオフセット
            escapeTarget = transform.position + randomOffset;  // 新しいターゲット位置
            if (NavMesh.SamplePosition(escapeTarget, out hit, _hitAreaDistance, NavMesh.AllAreas))
            {
                _agent.SetDestination(hit.position);
            }
        }
    }
    /// <summary>
    /// プレイヤーの方向に向く
    /// </summary>
    void LookAwayFromPlayer()
    {
        Vector3 direction = _player.position - transform.position; // プレイヤーの方向を計算
        direction.y = 0; // 水平方向の回転のみ
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _enemyRotationSpeed);
        }
    }

}
