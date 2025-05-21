using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab; // 生成するオブジェクトのプレハブ
    [SerializeField] private int poolSize = 10; // プールの初期サイズ

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        // 指定した数だけオブジェクトをプールしておく
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    /// <summary>
    /// オブジェクトを取得
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public GameObject GetObject(Vector3 position, Quaternion rotation)
    {
        GameObject obj;
        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
            obj.SetActive(true);
        }
        else
        {
            obj = Instantiate(prefab);
        }

        obj.transform.position = position;
        obj.transform.rotation = rotation;
        return obj;
    }
    /// <summary>
    /// プールを用意する
    /// </summary>
    /// <param name="count"></param>
    public void Initialize(int count)
    {
        // 指定された数だけオブジェクトをプールしておく
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    /// <summary>
    /// オブジェクトをプールに戻す
    /// </summary>
    /// <param name="obj"></param>
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}