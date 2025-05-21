using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectPrefabs;       // ��������I�u�W�F�N�g�̃v���n�u���X�g
    [SerializeField] private List<float> spawnProbabilities;       // �����m�����X�g
    [SerializeField] private List<Transform> spawnPoints;          // �����|�C���g�̃��X�g
    [SerializeField] private int poolSize = 10;                    // �v�[������I�u�W�F�N�g�̐�
    [SerializeField] private int maxCreateCount;�@�@�@�@�@ //�ő吶����
    private float minSpawnTime = 1f;              // �ŏ���������
    private float maxSpawnTime = 5f;              // �ő吶������
    private int minSpawnCount = 1;                // �ŏ�������
    private int maxSpawnCount = 3;                // �ő吶����
    private List<GameObject> objectPool = new List<GameObject>();
    private HashSet<Transform> usedSpawnPoints = new HashSet<Transform>();
    private int activeAnimals = 0;
    private int createCount;


    void Start()
    {
        // �v�[���̏�����
        InitializeObjectPool();

        // �����_�����ԂŐ������J�n
        StartCoroutine(SpawnObjectsRandomIntervals());

        createCount = 0;
    }

    /// <summary>
    ///  �I�u�W�F�N�g�v�[���̏�����
    /// </summary>
    void InitializeObjectPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject prefabToInstantiate = SelectRandomPrefab();
            GameObject obj = Instantiate(prefabToInstantiate);
            obj.SetActive(false);  // ������Ԃł͔�A�N�e�B�u
            objectPool.Add(obj);
        }
    }

    /// <summary>
    /// �����_���ȊԊu�ŃI�u�W�F�N�g�𐶐�����
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnObjectsRandomIntervals()
    {
        while (createCount < maxCreateCount)
        {
            float randomInterval = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(randomInterval);

            if (activeAnimals < 5)
            {
                int randomSpawnCount = Random.Range(minSpawnCount, maxSpawnCount + 1);
                SpawnRandomObjects(randomSpawnCount);
            }
        }
    }

    /// <summary>
    /// �����_���ɃI�u�W�F�N�g���w�萔�擾���ă����_���Ȑ����|�C���g�ɔz�u
    /// </summary>
    /// <param name="spawnCount"></param>
    public void SpawnRandomObjects(int spawnCount)
    {
        int maxSpawn = Mathf.Min(spawnCount, 5 - activeAnimals);
        // �g�p�ς݃|�C���g�����Z�b�g
        usedSpawnPoints.Clear();
        List<Transform> availableSpawnPoints = new List<Transform>(spawnPoints);

        for (int i = 0; i < maxSpawn; i++)
        {
            if (createCount >= maxCreateCount || availableSpawnPoints.Count == 0)
                break;

            GameObject selectedObject = GetPooledObject();
            Transform spawnPoint = GetRandomUnusedSpawnPoint(ref availableSpawnPoints);

            if (selectedObject != null && spawnPoint != null)
            {
                selectedObject.transform.position = spawnPoint.position;
                selectedObject.transform.rotation = spawnPoint.rotation;
                selectedObject.SetActive(true);
                activeAnimals++;
                createCount++;
                // �g�p�ς݃��X�g�ɒǉ�
                usedSpawnPoints.Add(spawnPoint);
            }
        }
    }
    /// <summary>
    /// �g�p�ς݂łȂ������|�C���g�������_���Ɏ擾
    /// </summary>
    /// <returns></returns>
    private Transform GetRandomUnusedSpawnPoint(ref List<Transform> availableSpawnPoints)
    {
        if (availableSpawnPoints.Count == 0)
            return null;

        int index = Random.Range(0, availableSpawnPoints.Count);
        Transform selected = availableSpawnPoints[index];
        // �I�񂾃X�|�[���|�C���g���폜�i������Ȃ��悤�Ɂj
        availableSpawnPoints.RemoveAt(index);

        return selected;
    }

    /// <summary>
    /// �v�[�����疢�g�p�̃v���n�u�����o��
    /// </summary>
    /// <returns></returns>
    GameObject GetPooledObject()
    {
        foreach (GameObject obj in objectPool)
        {
            if (!obj.activeInHierarchy)
                return obj;
        }
        return null;
    }

    /// <summary>
    /// �v�[���ɖ߂�
    /// </summary>
    /// <param name="obj"></param>
    public void ReturnObjectToPool(GameObject obj)
    {
        obj.SetActive(false);
        activeAnimals--;
    }

    /// <summary>
    /// �v���n�u��ݒ肳�ꂽ�m���őI������
    /// </summary>
    /// <returns></returns>
    GameObject SelectRandomPrefab()
    {
        float totalProbability = 0f;
        foreach (float prob in spawnProbabilities)
        {
            totalProbability += prob;
        }

        float randomPoint = Random.Range(0f, totalProbability);
        float cumulativeProbability = 0f;

        for (int i = 0; i < objectPrefabs.Count; i++)
        {
            cumulativeProbability += spawnProbabilities[i];
            if (randomPoint <= cumulativeProbability)
            {
                return objectPrefabs[i];
            }
        }

        return objectPrefabs[0];
    }

}
