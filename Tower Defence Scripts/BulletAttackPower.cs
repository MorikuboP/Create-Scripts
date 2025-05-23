using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAttackPower : MonoBehaviour
{
    [SerializeField] private int _BulletDamage;　//弾の攻撃力

    public static class Tags
    {
        public static string Enemy = "Enemy";
    }

    /// <summary>
    /// 弾に設定した攻撃力をこの弾が当たった敵に与える
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.Enemy))
        {
            if (collision.gameObject.TryGetComponent<EnemyHitPoint>(out EnemyHitPoint enemyHP))
            {
                enemyHP.TakeDamage(_BulletDamage);
            }

        }
    }
}
