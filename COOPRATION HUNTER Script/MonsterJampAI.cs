using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterJampAI : MonoBehaviour
{
    #region Fields
    [SerializeField] private ParticleSystem particle;     // パーティクル
    [SerializeField] private Transform target;            // 追尾ターゲット
    [SerializeField] private float bounceHeight = 2f;     // バウンドの高さ
    [SerializeField] private float bounceInterval = 0.5f; // バウンドの間隔
    [SerializeField] private float _moveSpeed = 3f;        // 移動速度
    [SerializeField] private AudioClip hitSound;          // ヒット時のサウンド
    [SerializeField] private ObjectPool objectPool;        // オブジェクトプール
    private AudioSource audioSource;
    private Vector3 initialLocalPosition;
    private Rigidbody rb;
    private float bounceTimer;                            // バウンド用タイマー
    private Vector3 bounceOffset;                         // バウンドのオフセット
    private bool isHit = false;                           // ヒット状態
    private bool canMove = true;                          // 弾の移動可否
    private const float _slowSpeed = 2f;                         //ターゲットの体力が少なくなった時の手加減用速度
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

                // ターゲット方向を向く
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

            }

        }
    }
    /// <summary>
    /// 矢に当たった時のエフェクトやSE、アニメーションを再生する
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (isHit || !collision.gameObject.CompareTag(Tags.Arrow)) return;

        isHit = true;
        canMove = false;

        // パーティクル再生
        ParticleSystem newParticle = Instantiate(particle);
        Vector3 particlePosition = transform.position + Vector3.up * 1f;
        newParticle.transform.position = particlePosition;

        // アニメーション切り替え
        anim.SetBool(Tags.Die, true);

        audioSource.PlayOneShot(hitSound); // ヒット時のサウンド再生
        newParticle.Play();

        StartCoroutine(ReturnParticleToPool(newParticle));
    }
    /// <summary>
    /// エフェクトのプール管理
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
    /// 敵をプールに戻す際にリセットしてから戻す
    /// </summary>
    private void ResetMonster()
    {

        isHit = false;
        canMove = true;

        // Rigidbody の状態をリセット
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
