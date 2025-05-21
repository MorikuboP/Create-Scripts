using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveUpdater : MonoBehaviour
{
    #region Fields
    [SerializeField] Transform _player;                             // �v���C���[��Transform
    [SerializeField] private MazeGenerator _mazeGenerator;         // GridGenerator ���A�^�b�`���Ċi�[
    [SerializeField] private float _detectionRange = default;       // ������͈̔�
    [SerializeField] private float _viewAngle = default;            // ����p
    [SerializeField] private float _slowSpeed = default;           // �ړ��X�s�[�h���x���Ȃ�Ƃ��̃X�s�[�h
    [SerializeField] private float _moveSpeed = default;           // �ړ��X�s�[�h
    private EnemyEscapeMove _escapeMove;
    private EnemyNormalMove _normalMove;
    private const float _halfAngleDivisor = 2f;
    #endregion

    void Start()
    {
        //Script�̎擾
        _escapeMove = GetComponent<EnemyEscapeMove>();
        _normalMove = GetComponent<EnemyNormalMove>();
        //�G�l�~�[�̈ړ��X�s�[�h�̐ݒ�
        _normalMove.MoveSpeed = _moveSpeed;
        _escapeMove.MoveSpeed = _moveSpeed;

        // _escapeMove �Ƀv���C���[����n��
        if (_player != null)
        {
            _escapeMove.SetPlayer(_player);
        }
    }

    void Update()
    {
            // �v���C���[�𔭌�������G�X�P�[�v�A����ȊO�Ȃ玩�R�ړ�
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
    /// ����Script�ɐ؂�ւ�
    /// </summary>
    /// <param name="active">�N�����Ă��邩</param>
    public void SetEscape(bool active)
    {
        _escapeMove.enabled = active;
        _normalMove.enabled = !active;
    }
    /// <summary>
    /// �ʏ�ړ�Script�ɐ؂�ւ�
    /// </summary>
    /// <param name="active">�N�����Ă��邩</param>
    public void SetFreeMove(bool active)
    {
        _normalMove.enabled = active;
        _escapeMove.enabled = !active;
       
    }

    /// <summary>
    /// ��������[�h�̓��e
    /// </summary>
    public void LaxityMode()
    {
        // �ړ����x��ቺ
        _normalMove.MoveSpeed = _slowSpeed;
        _escapeMove.MoveSpeed = _slowSpeed;

    }
    /// <summary>
    /// ������Ƀv���C���[�����邩�̔��f
    /// </summary>
    /// <returns>������ɓG�����邩�ǂ���</returns>
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
    /// �f�o�b�O�p�Ɏ���͈͂�Gizmos�ŉ���
    /// </summary>
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }
}
