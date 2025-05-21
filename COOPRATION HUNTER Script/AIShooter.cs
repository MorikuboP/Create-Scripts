using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIShooter : MonoBehaviour
{
    #region Fields
    private List<Transform> targets = new List<Transform>();   // ���X�g�ɓ����^�[�Q�b�g�I�u�W�F�N�g
    private float rotationSpeed = 5f;          // �̂̉�]�X�s�[�h
    private float minSpawnTime = 1f;           // �ŏ���������
    private float maxSpawnTime = 5f;           // �ő吶������
    private float _spawnTimer;
    private float timer;
    private Transform currentTarget = null;    // �ł��߂��^�[�Q�b�g��ێ�����t�B�[���h
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
        // �^�[�Q�b�g�����Ȃ��ꍇ�͏������Ȃ�
        if (targets.Count == 0) return;
        // ��ԋ߂��^�[�Q�b�g���擾
        currentTarget = GetClosestTarget(); 

        if (currentTarget != null)
        {
            // �^�[�Q�b�g�̌��݈ʒu�̕����������iX,Z�݂̂��g�p��Y�͌Œ�j
            Vector3 direction = currentTarget.position - transform.position;
            // Y�����Œ肷��
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
    ///���G�͈͓��̓������G�����X�g�ɓ����
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Enemy))
        {
            targets.Add(other.transform);
        }
    }

    /// <summary>
    /// ���G�͈͓�����o���G�����X�g����폜����
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.Enemy))
        {
            targets.Remove(other.transform);

            // �O�ꂽ�^�[�Q�b�g�����݂̃^�[�Q�b�g�������ꍇ�AcurrentTarget�����Z�b�g
            if (other.transform == currentTarget)
            {
                currentTarget = null;
            }
        }
    }

   /// <summary>
   /// ��ԋ������߂��G�����b�N�I������
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
    /// �����_���Ȏ��ԂœG�ɖ�����
    /// </summary>
    private void ShootIfReady()
    {
        if (currentTarget == null) return;

        // �^�[�Q�b�g�̌��݈ʒu�Ɍ������Ė�𔭎�
        spawn.ShootArrow(currentTarget.position);
        _isReadyToShot = false;
        _spawnTimer = Random.Range(minSpawnTime, maxSpawnTime);
        timer = 0;
    }
}
