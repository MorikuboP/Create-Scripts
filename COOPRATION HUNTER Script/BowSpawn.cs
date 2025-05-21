using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowSpawn : MonoBehaviour
{
    [SerializeField] private ArrowPool arrowPool;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Transform enemyTarget;
    private float arrowSpeed = 40f; // ��̑��x
    private GameObject arrow;
 

    // �\���ʒu���󂯎�郁�\�b�h
    public void ShootArrow(Vector3 targetPosition)
    {
        GameObject arrow = arrowPool.GetArrow(); // ����v�[������擾

        if (arrow != null)
        {
            // ��̈ʒu�Ɖ�]��ݒ�
            arrow.transform.position = shootPoint.position;

            // Z��������90�x��]
            Quaternion rotation = shootPoint.rotation * Quaternion.Euler(0, 0, 90);
            arrow.transform.rotation = rotation;

            arrow.SetActive(true);

            // ��̓������J�n
            arrow.GetComponent<ArrowManager>().Launch(arrowSpeed);
        }
    }

    // �\���ʒu���v�Z���郁�\�b�h
    public Vector3 PredictTargetPosition(Transform target)
    {
        Rigidbody targetRb = target.GetComponent<Rigidbody>();
        Vector3 targetVelocity = targetRb ? targetRb.velocity : Vector3.zero;
        float predictionTime = 0.5f;
        return target.position + targetVelocity * predictionTime;
    }
}
