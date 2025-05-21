using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    [SerializeField] private float _slowSpeed ;                       // 移動スピードが遅くなるときのスピード
    [SerializeField] private float _moveSpeed = 3f;                            // 移動スピード
    [SerializeField] private Transform _playerTransform = default;             // プレイヤーのTransform
    [SerializeField] private MazeGenerator _mazeGenerator;                     // GridGenerator をアタッチして格納
    [SerializeField] private float _minDistanceFromPlayer = 1f;                // 最小距離
    [SerializeField] private float _maxDistanceFromPlayer = 7f;                // 最大距離
    [SerializeField] private float minDistanceFromCurrentTarget = 1.0f;        // 現在のターゲットからの最小距離
    [SerializeField] private float predictionTime = 1.5f;                      // 予測時間（0.5～2.0推奨）
    private PlayerMove _playerMove;                                            // プレイヤーの移動スクリプトを取得
    private Vector3 _playerSpeed;
    private float _stoppingDistance = default;                                 // ターゲットとの停止距離
    //private NavMeshObstacle _playerObstacle = default;                       // プレイヤーのNavMeshObstacle参照
    private Rigidbody _playerRigidbody;                                        //プレイヤーのRigidbody
    private bool _isMove = false;                                              // 移動フラグ
    private NavMeshAgent agent;                                                // ナビゲーションエージェント
    private bool hasSetInitialTarget = false;                                  // 最初のターゲット設定済みか
    private List<Vector3> walkableCells = new List<Vector3>();                 // 歩行可能なセルのリスト
    private float _checkRadius = 5f;                                            // プレイヤーとエージェントのチェック半径
    private float _easyCheckRadius = 2f;                                        // プレイヤーとエージェントのチェック半径（簡易）
    private Vector3 targetPosition = default;                                  // 現在のターゲット位置
    private float lastTargetResetTime = default;                               // 最後のターゲット再設定時間
    private float time = default;                                              // 時間計測用の変数
    private int targetSelectionCount = 0;                                      // ターゲット選択回数をカウント
   


    private void Awake()
    {
        // NavMeshAgentの取得
        agent = GetComponent<NavMeshAgent>();
    }

    public static class Tags
    {
        public const string Player = "Player";

    }
    private void Start()
    {

        agent.speed = _moveSpeed; // 初期移動速度を設定

        if (_mazeGenerator != null)
        {
            walkableCells = _mazeGenerator.WalkableCells;
        }
        else
        {
            Debug.LogError("GridGenerator がシーン内に見つかりません。");
        }

        // プレイヤーオブジェクトを取得
        GameObject player = GameObject.FindGameObjectWithTag(Tags.Player);

        if (player != null)
        {
            _playerTransform = player.transform;
            _playerMove = player.GetComponent<PlayerMove>();
        }
        else
        {
            Debug.LogError("プレイヤーオブジェクトが見つかりません！");
        }

        GridCheck();

        StartMesh();

    }



    private void GridCheck()
    {
        walkableCells = _mazeGenerator.WalkableCells;
        //walkableCells = _gridGenerator.walkableCells; // プロパティ経由で取得

        if (_mazeGenerator == null)
        {

            Debug.Log("ない");
            return;
        }

        if (walkableCells == null || walkableCells.Count == 0)
        {
            Debug.Log("ある");
            return;
        }
    }

    private void StartMesh()
    {
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position); // 強制移動
            agent.enabled = true;
        }
        else
        {
            agent.enabled = false;
            Debug.LogWarning("NavMesh上に配置できませんでした");
        }
    }
    ///<summary>
    ///移動管理フラグ
    /// </summary>
    public bool IsMove
    {
        get { return _isMove; }
        set { _isMove = value; }
    }
    private void Update()
    {
        // 移動フラグが有効ならエージェントの移動を開始
        if (_isMove)
        {
            AgentMove();
        }

    }

    /// <summary>
    /// 敵の移動の内容
    /// </summary>
    private void AgentMove()
    {
        if (!hasSetInitialTarget)
        {
            SetNewTargetPosition(); // 新しいターゲット位置を設定
            hasSetInitialTarget = true;
        }

        if (!agent.enabled) return; // NavMeshAgentが無効なら処理しない

        agent.speed = _moveSpeed; // エージェントの速度を設定
        time += Time.deltaTime; // 時間を経過させる

        // プレイヤーが現在のパスにいるかチェック
        if (Time.time - lastTargetResetTime > predictionTime)
        {
            if (IsPlayerOnPath()) // プレイヤーがパスにいる場合
            {
                SetNewTargetPosition(); // 新しいターゲット位置を設定
                lastTargetResetTime = Time.time; // ターゲット設定時間を記録
            }
        }

        if (!agent.pathPending && agent.enabled)
        {
            float AgentDistance = agent.remainingDistance; // 安全に取得

            if (AgentDistance <= _stoppingDistance)
            {
                SetNewTargetPosition();
            }
        }
    }

    /// <summary>
    /// 手加減モードの内容
    /// </summary>
    public void LaxityMode()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(Tags.Player);

       
        foreach (GameObject obj in objects)
        {
            NavMeshObstacle obstacle = obj.GetComponent<NavMeshObstacle>();
            if (obstacle != null)
            {
                obstacle.enabled = false;
               
            }
        }

        _moveSpeed = _slowSpeed; // 移動速度を低下
        _checkRadius = _easyCheckRadius; // チェック半径を縮小
    }

    /// <summary>
    /// 新しいターゲット位置を設定する
    /// プレイヤーの現在位置と予測位置を考慮し、適切な移動先を選択する
    /// </summary>
    private void SetNewTargetPosition()
    {
        // もし walkableCells（移動可能なマス）が空なら、処理を終了
        if (walkableCells == null || walkableCells.Count == 0)
        {
            return;
        }

        // プレイヤーの参照が null ならエラーを出して処理を終了
        if (_playerTransform == null || _playerMove == null)
        {
            Debug.LogError("プレイヤーの参照がありません！");
            return;
        }

        // 現在のプレイヤー位置とターゲット位置を取得
        Vector3 playerPosition = _playerTransform.position;
        Vector3 currentTarget = agent.destination;

        // プレイヤーの移動速度を取得し、予測時間を考慮して予測位置を算出
        _playerSpeed = _playerMove.PlayerSpeed;
        float predictionTime = 1.0f;
        Vector3 predictedPlayerPosition = playerPosition + (_playerSpeed * predictionTime);

        // 移動可能なターゲット候補を格納するリスト
        List<Vector3> validTargets = new List<Vector3>();

        // 移動可能なセルをチェックして、条件に合うものを validTargets に追加
        foreach (Vector3 cell in walkableCells)
        {
            float distanceToPredictedPosition = Vector3.Distance(predictedPlayerPosition, cell);
            float distanceToCurrentTarget = Vector3.Distance(currentTarget, cell);

            // 通常の条件でターゲットを選択
            if (distanceToPredictedPosition >= _minDistanceFromPlayer &&
                distanceToPredictedPosition <= _maxDistanceFromPlayer &&
                distanceToCurrentTarget >= minDistanceFromCurrentTarget)
            {
                validTargets.Add(cell);
            }
            // 条件を少し緩めて選択肢を増やす
            else if (distanceToPredictedPosition >= (_minDistanceFromPlayer * 0.8f) &&
                     distanceToPredictedPosition <= (_maxDistanceFromPlayer * 1.2f))
            {
                validTargets.Add(cell);
            }
        }

        // ターゲットが見つからなかった場合はエラーを出して処理を終了
        if (validTargets.Count == 0)
        {
            Debug.LogError("条件を緩めてもターゲットが見つかりませんでした。");
            return;
        }

        // 選択回数をカウント（インクのマスを一定確率で優先するため）
        targetSelectionCount++;

        Vector3 selectedTarget;
        if (targetSelectionCount % 3 == 0 && _mazeGenerator.InkCells.Count > 0)
        {
            // 3回に1回はインクのあるマスを選択
            selectedTarget = _mazeGenerator.InkCells[Random.Range(0, _mazeGenerator.InkCells.Count)];
        }
        else
        {
            // 通常のターゲットをランダムに選択
            selectedTarget = validTargets[Random.Range(0, validTargets.Count)];
        }

        // NavMeshAgent に新しいターゲット位置を設定
        agent.SetDestination(selectedTarget);
    }


    // プレイヤーがターゲットパス上にいるかチェック
    private bool IsPlayerOnPath()
    {
        if (_playerTransform == null)
        {
            return false;
        }


        Vector3 agentPosition = agent.transform.position;
        Vector3 targetPosition = agent.destination;
        Vector3 playerPosition = _playerTransform.position;

        // プレイヤーがターゲットまたは現在位置付近にいる場合にのみ判定
        if (Vector3.Distance(playerPosition, targetPosition) < _checkRadius ||
            Vector3.Distance(playerPosition, agentPosition) < _checkRadius)
        {
            return true; // プレイヤーがパス上にいる
        }

        return false; // プレイヤーがパス上にいない
    }

    // Gizmosでターゲット位置を表示
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetPosition + Vector3.up * 1, 0.5f);
    }
}