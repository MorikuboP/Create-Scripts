using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectPrefabs;       // 生成するオブジェクトのプレハブリスト
    [SerializeField] private List<float> spawnProbabilities;       // 生成確率リスト
    [SerializeField] private List<Transform> spawnPoints;          // 生成ポイントのリスト
    [SerializeField] private int poolSize = 10;                    // プールするオブジェクトの数
    [SerializeField] private int maxCreateCount;　　　　　 //最大生成数
    private float minSpawnTime = 1f;              // 最小生成時間
    private float maxSpawnTime = 5f;              // 最大生成時間
    private int minSpawnCount = 1;                // 最小生成数
    private int maxSpawnCount = 3;                // 最大生成数
    private List<GameObject> objectPool = new List<GameObject>();
    private HashSet<Transform> usedSpawnPoints = new HashSet<Transform>();
    private int activeAnimals = 0;
    private int createCount;


    void Start()
    {
        // プールの初期化
        InitializeObjectPool();

        // ランダム時間で生成を開始
        StartCoroutine(SpawnObjectsRandomIntervals());

        createCount = 0;
    }

    /// <summary>
    ///  オブジェクトプールの初期化
    /// </summary>
    void InitializeObjectPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject prefabToInstantiate = SelectRandomPrefab();
            GameObject obj = Instantiate(prefabToInstantiate);
            obj.SetActive(false);  // 初期状態では非アクティブ
            objectPool.Add(obj);
        }
    }

    /// <summary>
    /// ランダムな間隔でオブジェクトを生成する
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
    /// ランダムにオブジェクトを指定数取得してランダムな生成ポイントに配置
    /// </summary>
    /// <param name="spawnCount"></param>
    public void SpawnRandomObjects(int spawnCount)
    {
        int maxSpawn = Mathf.Min(spawnCount, 5 - activeAnimals);
        // 使用済みポイントをリセット
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
                // 使用済みリストに追加
                usedSpawnPoints.Add(spawnPoint);
            }
        }
    }
    /// <summary>
    /// 使用済みでない生成ポイントをランダムに取得
    /// </summary>
    /// <returns></returns>
    private Transform GetRandomUnusedSpawnPoint(ref List<Transform> availableSpawnPoints)
    {
        if (availableSpawnPoints.Count == 0)
            return null;

        int index = Random.Range(0, availableSpawnPoints.Count);
        Transform selected = availableSpawnPoints[index];
        // 選んだスポーンポイントを削除（次回被らないように）
        availableSpawnPoints.RemoveAt(index);

        return selected;
    }

    /// <summary>
    /// プールから未使用のプレハブを取り出す
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
    /// プールに戻す
    /// </summary>
    /// <param name="obj"></param>
    public void ReturnObjectToPool(GameObject obj)
    {
        obj.SetActive(false);
        activeAnimals--;
    }

    /// <summary>
    /// プレハブを設定された確率で選択する
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
