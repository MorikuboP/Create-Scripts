using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerEnemyCaught : MonoBehaviour
{
 
    public static class Tags
    {
        public const string Enemy = "enemy";
    }
    /// <summary>
    /// �v���C���[���G�[�W�F���g��߂܂������̏���
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag(Tags.Enemy))
        {
            SceneManager.LoadScene("ClearScene");
        }
    }
}
