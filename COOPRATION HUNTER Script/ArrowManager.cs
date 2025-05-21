using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowManager : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f; // 矢の寿命
    [SerializeField] private ScoreCount Scorecount;
    private float arrowSpeed; // 矢の速度
    private float timeAlive = 0f;

    public static class Tags
    {
        public const string Enemy = "Enemy";
    }

    public void Launch(float arrowSpeed)
    {
        this.arrowSpeed = arrowSpeed;
        gameObject.SetActive(true);
        StartCoroutine(MoveArrow());
    }

    private System.Collections.IEnumerator MoveArrow()
    {
        while (timeAlive < lifetime)
        {
            transform.Translate(Vector3.up * arrowSpeed * Time.deltaTime);
            timeAlive += Time.deltaTime;
            yield return null;
        }
        ReturnObjectToPool(gameObject); // 矢が寿命を迎えたらプールに戻す
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(Tags.Enemy))
        {
            Scorecount.ScoreAdd();
            ReturnObjectToPool(gameObject);
        }
    }
    public void ReturnObjectToPool(GameObject arrow)
    {
        // 矢をプールに戻す
        arrow.SetActive(false);
        timeAlive = 0f; // 寿命のリセット
    }
}
