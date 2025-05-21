using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class EnemyNormalMove : MonoBehaviour
{
    #region Fields 
    [SerializeField] private MazeGenerator _mazeGenerator;
    [SerializeField] private Transform _player; // プレイヤーのTransform
    private float _moveSpeed = 4f;
    private float _stoppingDistance = 1f;
    private float _moveCount = 0;
    private const float _hitAreaDistance = 100f;
    private const float _rotationSpeed = 5f;
    private const int _inkMoveFrequency = 7;
    private const float _escapeDistance = 5f; // プレイヤーがこの距離に近づいたら逃げる
    private NavMeshAgent _agent;
    private Vector3 _targetPosition = default;
    private List<Vector3> _walkableCells = new List<Vector3>();
    private List<Vector3> _inkCells;
    private bool _hasSetInitialTarget = false;
    private bool _isGridCheack = false;
    private bool _isMove = false;
    #endregion

    #region Property
    public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
    public bool IsMove { get => _isMove; set => _isMove = value; }
    public bool GridCheack { get => _isGridCheack; set => _isGridCheack = value; }
    #endregion

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _moveSpeed;

        if (_mazeGenerator != null)
        {
            _walkableCells = _mazeGenerator.WalkableCells;
            _inkCells = _mazeGenerator.InkCells;
        }
        else
        {
            Debug.LogError("MazeGenerator がシーン内に見つかりません。");
        }

        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (_player == null)
            {
                Debug.LogError("Player が見つかりません。");
            }
        }

        if (_isGridCheack)
        {
            GridCheck();
        }
    }

    private void Update()
    {
        if (_isMove)
        {
            AgentMove();
        }
    }

    private void GridCheck()
    {
        _walkableCells = _mazeGenerator.WalkableCells;

        if (_mazeGenerator == null || _walkableCells == null || _walkableCells.Count == 0)
        {
            return;
        }
    }

    public void AgentMove()
    {
        if (!_agent.enabled) return;

        if (!_hasSetInitialTarget)
        {
            SetNewTargetPosition();
            _hasSetInitialTarget = true;
        }

        if (!_agent.pathPending && _agent.enabled)
        {
            if (_agent.remainingDistance <= _stoppingDistance || IsPlayerInPath())
            {
                SetNewTargetPosition();
            }
        }

        if (_agent.velocity.magnitude > 0.1f)
        {
            RotationEnemy();
        }
    }

    public void SetNewTargetPosition()
    {
        if (_walkableCells.Count == 0 || _player == null) return;

        Vector3 newTarget;
        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

        if (distanceToPlayer <= _escapeDistance)
        {
            newTarget = FindFurthestPointFromPlayer();
        }
        else
        {
            if (_moveCount % _inkMoveFrequency == 0 && _inkCells.Count > 0)
            {
                newTarget = _inkCells[Random.Range(0, _inkCells.Count)];
            }
            else
            {
                newTarget = _walkableCells[Random.Range(0, _walkableCells.Count)];
            }
        }

        _targetPosition = newTarget;
        _agent.SetDestination(_targetPosition);
        _moveCount++;
    }

    private Vector3 FindFurthestPointFromPlayer()
    {
        Vector3 furthestPoint = _walkableCells[0];
        float maxDistance = 0f;

        foreach (var point in _walkableCells)
        {
            float dist = Vector3.Distance(point, _player.position);
            if (dist > maxDistance)
            {
                maxDistance = dist;
                furthestPoint = point;
            }
        }

        return furthestPoint;
    }

    private bool IsPlayerInPath()
    {
        if (_player == null) return false;
        Vector3 toTarget = _targetPosition - transform.position;
        Vector3 toPlayer = _player.position - transform.position;
        return Vector3.Dot(toTarget.normalized, toPlayer.normalized) > 0.8f;
    }

    private void RotationEnemy()
    {
        Vector3 direction = _agent.velocity.normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            _agent.transform.rotation = Quaternion.Slerp(_agent.transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_targetPosition + Vector3.up * 1, 0.5f);
    }
}