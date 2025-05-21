using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterMoveAI : MonoBehaviour
{
    #region Fields
    [SerializeField] private GameObject _target;　　　　　　　　// 追尾対象（AIプレイヤーのオブジェクト）
    [SerializeField] private float _moveSpeed = 2f;　　　　   　// 敵の移動速度
    [SerializeField] private ObjectPool _objectPoolManager;　   // オブジェクトプールマネージャー
    [SerializeField] private ParticleSystem _particle;　        // ヒット時に再生するパーティクルエフェクト
    [SerializeField] private AudioClip _hitSound;　　           // ヒット時のサウンド
    private AudioSource _audioSource;　　                       // オーディオソース
    private Animator _animator;　                               // アニメーション制御用
    public bool CanMove { get; private set; } = true;　         // 追尾可能フラグ（trueなら移動）
    private bool _isHit = false;　　                            // 矢に当たったかどうかのフラグ
    private float _initialMoveSpeed;　　                        // 初期の移動速度を保存
    private const float _slowSpeed = 2f;                         //ターゲットの体力が少なくなった時の手加減用速度
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

        // _target が null ならエラーメッセージを出す
        if (_target == null)
        {
            Debug.LogError("ObjectMove: _target が設定されていません！", this);
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
        if (_target == null) return; // _target が null の場合は処理しない

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
