using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAttackPower : MonoBehaviour
{
    [SerializeField] private int _BulletDamage;@//’e‚ÌUŒ‚—Í

    public static class Tags
    {
        public static string Enemy = "Enemy";
    }

    /// <summary>
    /// ’e‚Éİ’è‚µ‚½UŒ‚—Í‚ğ‚±‚Ì’e‚ª“–‚½‚Á‚½“G‚É—^‚¦‚é
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
