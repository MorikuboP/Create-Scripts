using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterMoveAI : MonoBehaviour
{
    #region Fields
    [SerializeField] private GameObject _target;�@�@�@�@�@�@�@�@// �ǔ��ΏہiAI�v���C���[�̃I�u�W�F�N�g�j
    [SerializeField] private float _moveSpeed = 2f;�@�@�@�@   �@// �G�̈ړ����x
    [SerializeField] private ObjectPool _objectPoolManager;�@   // �I�u�W�F�N�g�v�[���}�l�[�W���[
    [SerializeField] private ParticleSystem _particle;�@        // �q�b�g���ɍĐ�����p�[�e�B�N���G�t�F�N�g
    [SerializeField] private AudioClip _hitSound;�@�@           // �q�b�g���̃T�E���h
    private AudioSource _audioSource;�@�@                       // �I�[�f�B�I�\�[�X
    private Animator _animator;�@                               // �A�j���[�V��������p
    public bool CanMove { get; private set; } = true;�@         // �ǔ��\�t���O�itrue�Ȃ�ړ��j
    private bool _isHit = false;�@�@                            // ��ɓ����������ǂ����̃t���O
    private float _initialMoveSpeed;�@�@                        // �����̈ړ����x��ۑ�
    private const float _slowSpeed = 2f;                         //�^�[�Q�b�g�̗̑͂����Ȃ��Ȃ������̎�����p���x
    private AIHpManager _hpManager;
    private float _aiHp;
    private const float _aiHpLine = 2f;
    #endregion

    #region Properties

    public GameObject Target
    {
        get { return _target; }
        set { _target = value; }
    }

    #endregion

    public static class Tags
    {
        public const string Arrow = "arrow";
        public const string Die = "Die";
    }

    #region Unity Methods

    [System.Obsolete]
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _hpManager = FindObjectOfType<AIHpManager>();
        _initialMoveSpeed = _moveSpeed;

        // _target �� null �Ȃ�G���[���b�Z�[�W���o��
        if (_target == null)
        {
            Debug.LogError("ObjectMove: _target ���ݒ肳��Ă��܂���I", this);
            return;
        }

        transform.LookAt(_target.transform.position);
    }

    private void Update()
    {
        if (_isHit) return;
        HandleMovement();
    }

    #endregion

    #region Private Methods

    private void HandleMovement()
    {
        if (_target == null) return; // _target �� null �̏ꍇ�͏������Ȃ�

        transform.LookAt(_target.transform.position);
        _aiHp = _hpManager.AiHp;


        if (CanMove)
        {
            Vector3 direction = (_target.transform.position - transform.position).normalized;
            transform.position += direction * _moveSpeed * Time.deltaTime;

            if(_aiHpLine <= _aiHp)
            {
                transform.position += direction * _moveSpeed * Time.deltaTime;
            }
            else
            {
                transform.position += direction * _slowSpeed * Time.deltaTime;
            }

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isHit || !collision.gameObject.CompareTag(Tags.Arrow)) return;

        _isHit = true;
        CanMove = false;

        ParticleSystem newParticle = Instantiate(_particle, transform.position + Vector3.up * 2f, Quaternion.identity);
        newParticle.Play();
        _audioSource.PlayOneShot(_hitSound);
        _animator.SetBool(Tags.Die, true);

        StartCoroutine(ReturnParticleToPool(newParticle));
    }

    private IEnumerator ReturnParticleToPool(ParticleSystem particleSystem)
    {
        while (particleSystem.isPlaying)
        {
            yield return null;
        }

        Destroy(particleSystem.gameObject);
        _objectPoolManager.ReturnObjectToPool(gameObject);
        ResetEnemy();
    }

    private void ResetEnemy()
    {
        _moveSpeed = _initialMoveSpeed;
        CanMove = true;
        _isHit = false;
        _animator.SetBool(Tags.Die, false);
    }

    #endregion

}
