using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGoalCheck : MonoBehaviour
{
    private EnemyMove _enemyMove;     //EnemyMoveを取得
    private Rigidbody2D _rigidBody;   //リジットボディの取得

    public static class Tags
    {
        public const string Goal = "Goal";
    }

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _enemyMove = GetComponent<EnemyMove>();
    }
    /// <summary>
    /// 敵の情報をリセットする
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        //敵の移動情報をリセットし、リセットフラグを立てる
        if (other.CompareTag(Tags.Goal))
        {
           
            _enemyMove.CorrentPoint = 0;
            _enemyMove.IsResetFlag = true;
        }
    }
}
