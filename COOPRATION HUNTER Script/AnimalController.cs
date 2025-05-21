using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : MonoBehaviour
{
    private CreateObjects objectPool; // CreateObjectsのインスタンスを保持
    public float _speed;               // 動物の移動速度
    private Vector3 spawnPoint;        // 生成ポイント
    public bool _shotdirection = true; // 発射方向
    private GameObject createposition; // 生成位置

    void Start()
    {
        objectPool = transform.parent.GetComponent<CreateObjects>();
        gameObject.SetActive(false); // 初期状態で非アクティブにする
        _shotdirection = false; // 発射方向を初期化
    }

    public void move(Vector3 _pos)
    {
        createposition.transform.position = _pos; // 生成位置を更新
        gameObject.SetActive(true); // 動物をアクティブにする

        // 移動処理
        if (_shotdirection == false)
        {
            transform.position += transform.right * _speed * Time.deltaTime; // 右方向に移動
        }
        else if (_shotdirection == true)
        {
            transform.position += -transform.right * _speed * Time.deltaTime; // 左方向に移動
        }
    }

    // 生成ポイントを設定するメソッド
    public void SetSpawnPoint(Vector3 point)
    {
        spawnPoint = point;
    }

    // 生成ポイントを取得するメソッド
    public Vector3 GetSpawnPoint()
    {
        return spawnPoint;
    }

    // createpositionを設定するメソッド
    public void SetCreatePosition(GameObject position)
    {
        createposition = position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Delete"))
        {
            objectPool.Collect(this); // 画面外や特定のトリガーにぶつかったらCollect
        }
    }
}
