using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;                //�e�̎��
    [SerializeField] private int _poolSize;                           //�v�[���̃T�C�Y

    private Queue<GameObject> _bulletPool = new Queue<GameObject>();  //�e�̃��X�g

    /// <summary>
    /// �e�̃v�[���𐶐�����
    /// </summary>
    private void Awake()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            GameObject _bullet = Instantiate(_bulletPrefab);
            _bullet.SetActive(false);
            _bulletPool.Enqueue(_bullet);
        }
    }
    /// <summary>
    /// �e���v�[�������o��
    /// </summary>
    /// <returns></returns>
    public GameObject GetBullet()
    {
        if(_bulletPool.Count > 0)
        {
            //  �v�[��������o��
            GameObject _bullet = _bulletPool.Dequeue();
            _bullet.SetActive(true);
            return _bullet;
        }
        else
        {
            GameObject _bullet = Instantiate(_bulletPrefab);
            return _bullet;
        }
    }
    /// <summary>
    /// �e���v�[���ɖ߂�
    /// </summary>
    /// <param name="_bullet"></param>
    public void ReturnBullet(GameObject _bullet)
    {
        _bullet.SetActive(false);
        //�v�[���ɖ߂�
        _bulletPool.Enqueue(_bullet);
    }
}
