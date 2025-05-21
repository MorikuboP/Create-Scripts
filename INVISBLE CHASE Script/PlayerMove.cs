using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4f;
    private bool _isMove = false;
    private Animator anime;
    private Rigidbody _rigidbody;
    private Vector3 Velocity;
    #region property
    public bool IsMove
    {
        get { return _isMove; }
        set { _isMove = value; }
    }

    public Vector3 PlayerSpeed
    {
        get { return Velocity; }
        set { Velocity = value; }
    }
    #endregion

    public static class Tags
    {
        public const string Run = "Run";
        public const string Hori = "Horizontal";
        public const string Ver = "Vertical";
    }
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }



    void Start()
    {
        anime = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();

        // Rigidbody の初期設定
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }


    /// <summary>
    /// プレイヤーの移動挙動
    /// </summary>
    void FixedUpdate()
    {
        if (_isMove)
        {
            float moveX = Input.GetAxis(Tags.Hori);
            float moveZ = Input.GetAxis(Tags.Ver);

            Vector3 movement = new Vector3(-moveZ, 0, moveX);

            if (movement.magnitude > 0.3f)
            {
                anime.SetBool(Tags.Run, true);
            }
            else
            {
                anime.SetBool(Tags.Run, false);
            }

            if (movement.magnitude > 1)
            {
                movement.Normalize();
            }

            // Rigidbody の velocity を使って移動
            _rigidbody.velocity = new Vector3(movement.x * moveSpeed, _rigidbody.velocity.y, movement.z * moveSpeed);

            // 移動ベクトルがゼロでない場合のみ回転させる
            if (movement != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(movement);
                _rigidbody.MoveRotation(Quaternion.Slerp(_rigidbody.rotation, targetRotation, Time.fixedDeltaTime * 10f));
            }
        }
        else
        {
            // 移動していないときは停止
            _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
            anime.SetBool(Tags.Run, false);
        }
    }

}
