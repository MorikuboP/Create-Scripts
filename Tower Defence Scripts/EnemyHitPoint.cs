using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class EnemyHitPoint : MonoBehaviour
{
    [SerializeField] private int _EnemyHP;         //敵のHPを設定する
    [SerializeField] private Slider _hpSlider;     //敵HPのスライダーの設定
    [SerializeField] private string _enemyName;    //敵の名前の設定
    [SerializeField] private EnemyPool _enemyPool; //敵のぷーるを取得
    [SerializeField] private bool _isBoss = false; // ボスかどうかのフラグ
    private int _currentEnemyHP;                   //現在の敵のHP
    private EnemyTracker _enemyTracker;            //EnemyTrackerの取得

    // ボスが倒れたことを通知するイベント
    public static event Action _onBossDefeated;

    private void Start()
    {
        _currentEnemyHP = _EnemyHP;
        _hpSlider.maxValue = _EnemyHP;
        _hpSlider.value = _EnemyHP;

        _enemyTracker = FindObjectOfType<EnemyTracker>();
    }

    private void Update()
    {
        //スライダーの位置を固定する
        if (_hpSlider != null)
        {
            Vector3 _screenPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1, 0));
            _hpSlider.transform.position = _screenPos;
        }
    }
    /// <summary>
    /// 敵のHPを減らし、スライダーに反映させる
    /// </summary>
    /// <param name="_BulletDamage"></param>
    public void TakeDamage(int _BulletDamage)
    {
        _currentEnemyHP -= _BulletDamage;
        _hpSlider.value = _currentEnemyHP;

        //HPが０になったら敵がやられた時の動きに入る
        if (_currentEnemyHP <= 0)
        {
            DestroyEnemy();
        }
    }
    /// <summary>
    /// やられたあとの処理
    /// </summary>
    private void DestroyEnemy()
    {
        //敵をリストからなくす
        if (_enemyTracker != null)
        {
            _enemyTracker.GetAllEnemies.Remove(gameObject);
            _enemyTracker.GetTargetEnemies.Remove(gameObject);
        }

        // ボスだった場合、イベントを発行
        if (_isBoss)
        {
            _onBossDefeated?.Invoke(); // ボス撃破イベントを発生
        }


        // オブジェクトをプールに戻す
        EnemyPool.Instance.ReturnEnemy(_enemyName, gameObject);
    }

}
