using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIShooter : MonoBehaviour
{
    #region Fields
    private List<Transform> targets = new List<Transform>();   // リストに入れるターゲットオブジェクト
    private float rotationSpeed = 5f;          // 体の回転スピード
    private float minSpawnTime = 1f;           // 最小生成時間
    private float maxSpawnTime = 5f;           // 最大生成時間
    private float _spawnTimer;
    private float timer;
    private Transform currentTarget = null;    // 最も近いターゲットを保持するフィールド
    private bool _isReadyToShot = true;
    private BowSpawn spawn;
    #endregion

    public static class Tags
    {
        public const string Enemy = "Enemy";
    }

    private void Start()
    {
        spawn = GetComponent<BowSpawn>();
    }

    void Update()
    {
        // ターゲットがいない場合は処理しない
        if (targets.Count == 0) return;
        // 一番近いターゲットを取得
        currentTarget = GetClosestTarget(); 

        if (currentTarget != null)
        {
            // ターゲットの現在位置の方向を向く（X,Zのみを使用しYは固定）
            Vector3 direction = currentTarget.position - transform.position;
            // Y軸を固定する
            direction.y = 0; 

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            if (_isReadyToShot)
            {
                ShootIfReady();
            }
        }

        if (timer >= _spawnTimer)
        {
            _isReadyToShot = true;
            timer = 0;
        }

        timer += Time.deltaTime;
    }

    /// <summary>
    ///索敵範囲内の入った敵をリストに入れる
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Enemy))
        {
            targets.Add(other.transform);
        }
    }

    /// <summary>
    /// 索敵範囲内から出た敵をリストから削除する
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.Enemy))
        {
            targets.Remove(other.transform);

            // 外れたターゲットが現在のターゲットだった場合、currentTargetをリセット
            if (other.transform == currentTarget)
            {
                currentTarget = null;
            }
        }
    }

   /// <summary>
   /// 一番距離が近い敵をロックオンする
   /// </summary>
    private Transform GetClosestTarget()
    {
        if (targets.Count == 0) return null;

        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform target in targets)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = target;
            }
        }

        return closestTarget;
    }

    /// <summary>
    /// ランダムな時間で敵に矢を放つ
    /// </summary>
    private void ShootIfReady()
    {
        if (currentTarget == null) return;

        // ターゲットの現在位置に向かって矢を発射
        spawn.ShootArrow(currentTarget.position);
        _isReadyToShot = false;
        _spawnTimer = Random.Range(minSpawnTime, maxSpawnTime);
        timer = 0;
    }
}
