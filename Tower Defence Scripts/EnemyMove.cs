using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    [SerializeField] private Transform[] _wayPoints;  //進む道のりを設定する
    [SerializeField] private float _moveSpeed;        //移動速度の設定
    private int _correntPoint = 0;                    //現在の目的地の設定
    private Rigidbody2D _rb;                          //リジットボディの取得
    private Animator _animator;                       //アニメーションを取得
    private bool isReset = false;                     //リセットフラグ
    
    private bool isSideWalkR;                          // アニメーションのフラグ
    private bool isSideWalkL;
    private bool isFrontWalk;
    private bool isBackWalk;
   
    private Vector2 lastDirection;                    // 現在の方向を記録する
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
            Debug.LogError("Animatorが見つかりません！", this);
        }
    }

    void Update()
    {
        if (_wayPoints.Length == 0)
        {
            return;
        }

        // TransformのpositionをVector2型に変換して計算
        Transform target = _wayPoints[_correntPoint];
        Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
        _rb.velocity = direction * _moveSpeed;

        // 方向が変わったらアニメーションを更新
        if (direction != lastDirection)
        {
            UpdateAnimation(direction);
            // 方向を更新
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
    /// アニメーションを更新する
    /// </summary>
    /// <param name="direction"></param>
    private void UpdateAnimation(Vector2 direction)
    {
        // 全アニメーションをオフにする
        _animator.SetBool(Tags.IsSideL, false);
        _animator.SetBool(Tags.IsSideR, false);
        _animator.SetBool(Tags.IsFront, false);
        _animator.SetBool(Tags.IsBack, false);

        // 進行方向に応じて適切なアニメーションをオンにする
        if (direction.x < _value) // 左
        {
            _animator.SetBool(Tags.IsSideL, true);
        }
        else if(direction.x > _value)//右
        {
            _animator.SetBool(Tags.IsSideR, true);
        }
        else if (direction.y > _value) // 上
        {
            _animator.SetBool(Tags.IsFront, true);
        }
        else if (direction.y < _value) // 下
        {
            _animator.SetBool(Tags.IsBack, true);
        }
    }
}
