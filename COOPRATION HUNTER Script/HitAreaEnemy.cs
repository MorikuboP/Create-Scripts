using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitAreaEnemy : MonoBehaviour
{ 
    [SerializeField] GameObject hittarget;

    private MonsterMoveAI move;

    private GameObject target;  // プレイヤーなどのターゲット

    public static class Tags
    {
        public const string AI = "AI";
    }

    private void Start()
    {
        move = GetComponent<MonsterMoveAI>();   // ObjectMove コンポーネントを取得
    }

    void Update()
    {
        if (target != null)
        {
            // ObjectMoveスクリプトのターゲットを更新
            target = move.Target;
                    
        }
        else
        {
            Debug.Log("ターゲットがnullです。");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // ヒットエリアに触れたプレイヤーをターゲットに設定
        if (other.CompareTag(Tags.AI))
        {
            target = other.gameObject;  // プレイヤーをターゲットとして設定
            Debug.Log("ターゲット設定: " + target.name);  // ターゲット設定時に確認
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.AI))
        {
            target = null;  // プレイヤーがエリアから出たらターゲットをnullにする
        }

    }
}
