using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterJampAI : MonoBehaviour
{
    #region Fields
    [SerializeField] private ParticleSystem particle;     // �p�[�e�B�N��
    [SerializeField] private Transform target;            // �ǔ��^�[�Q�b�g
    [SerializeField] private float bounceHeight = 2f;     // �o�E���h�̍���
    [SerializeField] private float bounceInterval = 0.5f; // �o�E���h�̊Ԋu
    [SerializeField] private float _moveSpeed = 3f;        // �ړ����x
    [SerializeField] private AudioClip hitSound;          // �q�b�g���̃T�E���h
    [SerializeField] private ObjectPool objectPool;        // �I�u�W�F�N�g�v�[��
    private AudioSource audioSource;
    private Vector3 initialLocalPosition;
    private Rigidbody rb;
    private float bounceTimer;                            // �o�E���h�p�^�C�}�[
    private Vector3 bounceOffset;                         // �o�E���h�̃I�t�Z�b�g
    private bool isHit = false;                           // �q�b�g���
    private bool canMove = true;                          // �e�̈ړ���
    private const float _slowSpeed = 2f;                         //�^�[�Q�b�g�̗̑͂����Ȃ��Ȃ������̎�����p���x
    private AIHpManager _hpManager;
    private float _aiHp;
    private const float _aiHpLine = 2f;
    private Animator anim = null;

    #endregion
    public static class Tags
    {
        public const string Arrow = "arrow";
        public const string Die = "Die";
    }

    [System.Obsolete]
    void Start()
    {
        initialLocalPosition = transform.localPosition;
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        _hpManager = FindObjectOfType<AIHpManager>();
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
    }

    void FixedUpdate()
    {
        if (isHit || !canMove) return;

        if (target != null)
        {
            if (isHit || !canMove) return;

            if (target != null)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                _aiHp = _hpManager.AiHp;
                direction.y = 0;
                if (_aiHpLine <= _aiHp)
                {
                    transform.position += direction * _moveSpeed * Time.fixedDeltaTime;
                }
                else
                {
                    transform.position += direction * _slowSpeed * Time.deltaTime;
                }


                bounceTimer += Time.fixedDeltaTime;
                if (bounceTimer >= bounceInterval)
                {
                    rb.velocity = new Vector3(rb.velocity.x, bounceHeight, rb.velocity.z);
                    bounceTimer = 0f;
                }

                // �^�[�Q�b�g����������
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

            }

        }
    }
    /// <summary>
    /// ��ɓ����������̃G�t�F�N�g��SE�A�A�j���[�V�������Đ�����
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (isHit || !collision.gameObject.CompareTag(Tags.Arrow)) return;

        isHit = true;
        canMove = false;

        // �p�[�e�B�N���Đ�
        ParticleSystem newParticle = Instantiate(particle);
        Vector3 particlePosition = transform.position + Vector3.up * 1f;
        newParticle.transform.position = particlePosition;

        // �A�j���[�V�����؂�ւ�
        anim.SetBool(Tags.Die, true);

        audioSource.PlayOneShot(hitSound); // �q�b�g���̃T�E���h�Đ�
        newParticle.Play();

        StartCoroutine(ReturnParticleToPool(newParticle));
    }
    /// <summary>
    /// �G�t�F�N�g�̃v�[���Ǘ�
    /// </summary>
    /// <param name="particleSystem"></param>
    /// <returns></returns>
    private IEnumerator ReturnParticleToPool(ParticleSystem particleSystem)
    {
        while (particleSystem.isPlaying)
        {
            yield return null;
        }

        ResetMonster();
    }
    /// <summary>
    /// �G���v�[���ɖ߂��ۂɃ��Z�b�g���Ă���߂�
    /// </summary>
    private void ResetMonster()
    {

        isHit = false;
        canMove = true;

        // Rigidbody �̏�Ԃ����Z�b�g
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
        }

        objectPool.ReturnObjectToPool(gameObject);
        transform.localPosition = initialLocalPosition;
    }

    public void InitializeTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
