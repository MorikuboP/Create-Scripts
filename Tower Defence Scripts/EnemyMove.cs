using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    [SerializeField] private Transform[] _wayPoints;  //�i�ޓ��̂��ݒ肷��
    [SerializeField] private float _moveSpeed;        //�ړ����x�̐ݒ�
    private int _correntPoint = 0;                    //���݂̖ړI�n�̐ݒ�
    private Rigidbody2D _rb;                          //���W�b�g�{�f�B�̎擾
    private Animator _animator;                       //�A�j���[�V�������擾
    private bool isReset = false;                     //���Z�b�g�t���O
    
    private bool isSideWalkR;                          // �A�j���[�V�����̃t���O
    private bool isSideWalkL;
    private bool isFrontWalk;
    private bool isBackWalk;
   
    private Vector2 lastDirection;                    // ���݂̕������L�^����
    private const float _value = 0.1f;

    public static class Tags
    {
        public const string IsSideL = "IsSideWalk";
        public const string IsSideR = "IsSideWalkR";
        public const string IsFront = "IsFrontWalk";
        public const string IsBack = "IsBackWalk";
    }

    public int CorrentPoint
    {
        get { return _correntPoint; }
        set { _correntPoint = value; }
    }

    public bool IsResetFlag
    {
        get { return isReset; }
        set { isReset = value; }
    }


    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        if (_animator == null)
        {
            Debug.LogError("Animator��������܂���I", this);
        }
    }

    void Update()
    {
        if (_wayPoints.Length == 0)
        {
            return;
        }

        // Transform��position��Vector2�^�ɕϊ����Čv�Z
        Transform target = _wayPoints[_correntPoint];
        Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
        _rb.velocity = direction * _moveSpeed;

        // �������ς������A�j���[�V�������X�V
        if (direction != lastDirection)
        {
            UpdateAnimation(direction);
            // �������X�V
            lastDirection = direction; 
        }

        if (Vector2.Distance(transform.position, target.position) < _value)
        {
            _correntPoint++;

            if (_correntPoint >= _wayPoints.Length)
            {
                gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// �A�j���[�V�������X�V����
    /// </summary>
    /// <param name="direction"></param>
    private void UpdateAnimation(Vector2 direction)
    {
        // �S�A�j���[�V�������I�t�ɂ���
        _animator.SetBool(Tags.IsSideL, false);
        _animator.SetBool(Tags.IsSideR, false);
        _animator.SetBool(Tags.IsFront, false);
        _animator.SetBool(Tags.IsBack, false);

        // �i�s�����ɉ����ēK�؂ȃA�j���[�V�������I���ɂ���
        if (direction.x < _value) // ��
        {
            _animator.SetBool(Tags.IsSideL, true);
        }
        else if(direction.x > _value)//�E
        {
            _animator.SetBool(Tags.IsSideR, true);
        }
        else if (direction.y > _value) // ��
        {
            _animator.SetBool(Tags.IsFront, true);
        }
        else if (direction.y < _value) // ��
        {
            _animator.SetBool(Tags.IsBack, true);
        }
    }
}
