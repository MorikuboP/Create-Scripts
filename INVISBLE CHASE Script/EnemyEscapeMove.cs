using UnityEngine.AI;
using UnityEngine;

public class EnemyEscapeMove : MonoBehaviour
{
    #region Fields
    [SerializeField] private float escapeDistance = default; // �v���C���[����ǂꂾ�����ꂽ����
    [SerializeField] private float checkInterval = default; // ���b���ƂɌo�H�X�V���邩
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

        // ���Ԋu�œ�����������X�V
        _timer += Time.deltaTime;
        if (_timer >= checkInterval)
        {
            _timer = 0f;
            EscapeFromPlayer();
        }

       
    }

    private void FixedUpdate()
    {
        // �v���C���[�̕���������
        LookAwayFromPlayer();
    }
    /// <summary>
    /// �v���C���[��ݒ肷��
    /// </summary>
    /// <param name="player">�G�l�~�[��������Ώ�</param>
    public void SetPlayer(Transform player)
    {
        _player = player;
    }
    /// <summary>
    /// �v���C���[�Ɣ��Ε����ɐi�݂Ȃ��瓦�����ݒ肷��
    /// </summary>
    void EscapeFromPlayer()
    {
        Vector3 directionToPlayer = _player.position - transform.position; // �v���C���[�ւ̃x�N�g��
        Vector3 escapeTarget = transform.position - directionToPlayer.normalized * escapeDistance; // ���Ε����ֈړ�

        // NavMesh��̈ړ��\�Ȉʒu��T��
        NavMeshHit hit;
        if (NavMesh.SamplePosition(escapeTarget, out hit, _hitAreaDistance, NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position);
        }
        else
        {
            // �X�^�b�N������邽�߂Ɏ��͂ɏ��������_���ɓ�����
            Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * _randomPositionMove; // �����_���ȃI�t�Z�b�g
            escapeTarget = transform.position + randomOffset;  // �V�����^�[�Q�b�g�ʒu
            if (NavMesh.SamplePosition(escapeTarget, out hit, _hitAreaDistance, NavMesh.AllAreas))
            {
                _agent.SetDestination(hit.position);
            }
        }
    }
    /// <summary>
    /// �v���C���[�̕����Ɍ���
    /// </summary>
    void LookAwayFromPlayer()
    {
        Vector3 direction = _player.position - transform.position; // �v���C���[�̕������v�Z
        direction.y = 0; // ���������̉�]�̂�
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _enemyRotationSpeed);
        }
    }

}
