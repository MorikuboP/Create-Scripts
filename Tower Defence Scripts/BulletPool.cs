using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;                //弾の種類
    [SerializeField] private int _poolSize;                           //プールのサイズ

    private Queue<GameObject> _bulletPool = new Queue<GameObject>();  //弾のリスト

    /// <summary>
    /// 弾のプールを生成する
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
    /// 弾をプールを取り出す
    /// </summary>
    /// <returns></returns>
    public GameObject GetBullet()
    {
        if(_bulletPool.Count > 0)
        {
            //  プールから取り出す
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
    /// 弾をプールに戻す
    /// </summary>
    /// <param name="_bullet"></param>
    public void ReturnBullet(GameObject _bullet)
    {
        _bullet.SetActive(false);
        //プールに戻す
        _bulletPool.Enqueue(_bullet);
    }
}
