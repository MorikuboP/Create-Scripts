using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathCheck : MonoBehaviour
{
    [SerializeField] private int _enemyHP;  //敵のHPを設定
    private Rigidbody2D _rigidbody;         //リジットボディを取得
    private EnemyMove _enemyMove;           //EnemyMoveを取得

    public static class Tags
    {
        public const string Attack = "Attack";
    }

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _enemyMove = GetComponent<EnemyMove>();
    }

    // Update is called once per frame
    void Update()
    {
        //敵の移動情報をリセットし、リセットフラグを立てる
        if(_enemyHP <= 0)
        {
            _enemyMove.CorrentPoint = 0;
            _enemyMove.IsResetFlag = true;
        }
    }
    /// <summary>
    /// 敵のHPを減らす
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter2D(Collision2D other)
    {
        //敵HPを減らす
        if(other.gameObject.CompareTag(Tags.Attack))
        {
            _enemyHP--;
        }
    }
}
