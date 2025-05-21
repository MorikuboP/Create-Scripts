using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowManager : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f; // ��̎���
    [SerializeField] private ScoreCount Scorecount;
    private float arrowSpeed; // ��̑��x
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
        ReturnObjectToPool(gameObject); // ��������}������v�[���ɖ߂�
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
        // ����v�[���ɖ߂�
        arrow.SetActive(false);
        timeAlive = 0f; // �����̃��Z�b�g
    }
}
