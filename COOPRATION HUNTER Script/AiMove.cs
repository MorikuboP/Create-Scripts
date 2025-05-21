using System.Collections.Generic;
using UnityEngine;

public class AiMove : MonoBehaviour
{
   
    [SerializeField] private float _moveSpeed = 2f;               // AIの移動速度
    [SerializeField] private float _areaRadius = 5f;              // 行動範囲の半径
    [SerializeField] private float _minLimitX, _maxLimitX;        // 移動可能範囲のX軸最小・最大値
    [SerializeField] private float _minLimitZ, _maxLimitZ;        // 移動可能範囲のZ軸最小・最大値
    [SerializeField] private Transform _player;                   // プレイヤーの位置情報
    private float _stopPosition = 0.5f;                           // 目的地に近づいたと判断する値
    private float _rotationSpeed = default;                       // 回転速度
    private bool _hasEnemyInRange;                                // 敵が範囲内にいるかどうかのフラグ
    private Vector3 _targetPosition;                              // 目的地の座標 
    private Animator _animator;                                   // アニメーション制御用のコンポーネント
    private HashSet<Transform> _targetsEnemy = new HashSet<Transform>();   // 検知したターゲット（敵）のリスト

    public static class Tags
    {
        public const string Enemy = "Enemy";
        public const string Walk = "Walk";
    }

    /// <summary>
    /// Animatorコンポーネントを取得
    /// </summary>
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // 目的地に十分近づいた場合、新しい目的地を決定する
        if ((transform.position - _targetPosition).sqrMagnitude < _stopPosition * _stopPosition)
        {
            // 敵が範囲内にいる場合はプレイヤーの位置を基準に、新しい移動先を設定
            _targetPosition = _hasEnemyInRange ? GetClampedPosition(_player.position, _areaRadius, 1f)
                                             : GetClampedPosition(transform.position, _areaRadius, 5f);
            SetWalkAnimation(false);
        }
        else
        {
            // 目的地に向かって移動
            MoveToTarget();
            SetWalkAnimation(true);
        }
    }

    /// <summary>
    /// 移動可能範囲を考慮し、新しいランダムな目的地を決定する
    /// </summary>
    /// <param name="basePosition"></param>
    /// <param name="radius"></param>
    /// <param name="minDistance"></param>
    /// <returns></returns>
    private Vector3 GetClampedPosition(Vector3 basePosition, float radius, float minDistance)
    {
        Vector3 randomOffset = Vector3.zero;
        float distance = 0f;

        while (distance < minDistance)
        {
            // 0〜360度のランダムな角度を取得
            float randomAngle = Random.Range(0f, 360f);
            // 最小距離から指定半径内のランダムな距離を決定
            distance = Random.Range(minDistance, radius);
            // ランダムな方向にオフセットを計算
            randomOffset = new Vector3(Mathf.Cos(randomAngle) * distance, 0, Mathf.Sin(randomAngle) * distance);
        }

        // 設定された範囲内にクランプ（制限）する
        Vector3 target = basePosition + randomOffset;
        target.x = Mathf.Clamp(target.x, _minLimitX, _maxLimitX);
        target.z = Mathf.Clamp(target.z, _minLimitZ, _maxLimitZ);
        return target;
    }

    /// <summary>
    ///目的地に向かって移動
    /// </summary>
    private void MoveToTarget()
    {
        // 目的地への方向を正規化
        Vector3 direction = (_targetPosition - transform.position).normalized;
        // 移動処理
        transform.position += direction * _moveSpeed * Time.deltaTime;

        // 進行方向にキャラクターを回転（水平のみ）
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        targetRotation.x = targetRotation.z = 0; // 水平方向の回転のみ許可
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
    }

    // アニメーションの設定
    private void SetWalkAnimation(bool isWalking)
    {
        _animator.SetBool(Tags.Walk, isWalking);
    }

    /// <summary>
    /// 動物の範囲に入った場合、ターゲットリストに追加し、敵がいる状態にする
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Enemy))
        {
            _targetsEnemy.Add(other.transform);
            _hasEnemyInRange = true;
        }
    }

    /// <summary>
    /// 動物の範囲から出た場合、ターゲットリストから削除し、リストが空なら敵がいない状態にする
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.Enemy))
        {
            _targetsEnemy.Remove(other.transform);
            _hasEnemyInRange = _targetsEnemy.Count > 0;
        }
    }
}