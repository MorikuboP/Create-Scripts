using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowSpawn : MonoBehaviour
{
    [SerializeField] private ArrowPool arrowPool;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Transform enemyTarget;
    private float arrowSpeed = 40f; // 矢の速度
    private GameObject arrow;
 

    // 予測位置を受け取るメソッド
    public void ShootArrow(Vector3 targetPosition)
    {
        GameObject arrow = arrowPool.GetArrow(); // 矢をプールから取得

        if (arrow != null)
        {
            // 矢の位置と回転を設定
            arrow.transform.position = shootPoint.position;

            // Z軸方向に90度回転
            Quaternion rotation = shootPoint.rotation * Quaternion.Euler(0, 0, 90);
            arrow.transform.rotation = rotation;

            arrow.SetActive(true);

            // 矢の動きを開始
            arrow.GetComponent<ArrowManager>().Launch(arrowSpeed);
        }
    }

    // 予測位置を計算するメソッド
    public Vector3 PredictTargetPosition(Transform target)
    {
        Rigidbody targetRb = target.GetComponent<Rigidbody>();
        Vector3 targetVelocity = targetRb ? targetRb.velocity : Vector3.zero;
        float predictionTime = 0.5f;
        return target.position + targetVelocity * predictionTime;
    }
}
