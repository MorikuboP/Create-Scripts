using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab; // ��������I�u�W�F�N�g�̃v���n�u
    [SerializeField] private int poolSize = 10; // �v�[���̏����T�C�Y

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        // �w�肵���������I�u�W�F�N�g���v�[�����Ă���
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    /// <summary>
    /// �I�u�W�F�N�g���擾
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
    /// �v�[����p�ӂ���
    /// </summary>
    /// <param name="count"></param>
    public void Initialize(int count)
    {
        // �w�肳�ꂽ�������I�u�W�F�N�g���v�[�����Ă���
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    /// <summary>
    /// �I�u�W�F�N�g���v�[���ɖ߂�
    /// </summary>
    /// <param name="obj"></param>
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}