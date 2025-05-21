using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AIHpManager : MonoBehaviour
{
    [SerializeField] private float aiHp = 5f;            // AI��HP
    [SerializeField] private Text hpText;                // HP��\������UI�e�L�X�g
    [SerializeField] private Renderer _renderer;
    private bool isInvincible = false;                  // ���G��Ԃ̃t���O
    private float blinkCycle = 0.4f;                             //�_�Ŏ���
    private const float blinkTime = 5;
    private const float Center = 0.2f;
   

    public static class Tags
    {
        public const string Enemy = "Enemy";
        public const string GameOver = "GameOverScene";
    }

    public float AiHp
    {
        get { return aiHp; }
        set { aiHp = value; }
    }
    void Update()
    {
        // HP��0�ɂȂ�����I�u�W�F�N�g���A�N�e�B�u�����AGameOverScene�����[�h
        if (aiHp <= 0)
        {
            StartCoroutine(LoadGameOverScene()); // �V�[���J�ڂ��J�n
        }

    }
    /// <summary>
    /// Enemy�ɂԂ��������̏���
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        // "Enemy"�^�O�̃I�u�W�F�N�g�ƏՓ˂����ꍇ
        if (collision.gameObject.CompareTag(Tags.Enemy) && !isInvincible)
        {
            aiHp--; // HP�����炷
            UpdateHpUI();
            StartCoroutine(ActivateInvincibility()); // ���G��Ԃ��J�n
        }
    }
    /// <summary>
    /// HP��UI�̃A�b�v�f�[�g
    /// </summary>
    private void UpdateHpUI()
    {
        hpText.text = aiHp.ToString(); // HP��UI�ɕ\��
    }
    /// <summary>
    /// �V�[���J�ڂ̏���
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadGameOverScene()
    {
        //�X���[���[�V�����ɂ���
        Time.timeScale = 0.2f;
        //�P�b�ԑ҂�
        yield return new WaitForSecondsRealtime(1f);
        //1���Ԃ����ɖ߂�
        Time.timeScale = 1f;
        //�V�[�������[�h
        SceneManager.LoadScene(Tags.GameOver, LoadSceneMode.Single);
    }

    /// <summary>
    /// ���G���ԂƃL�����̓_�ŏ���
    /// </summary>
    /// <returns></returns>
    private IEnumerator ActivateInvincibility()
    {
        isInvincible = true; // ���G��Ԃ��I��

        Renderer[] renderers = GetComponentsInChildren<Renderer>(); // �����Ǝq�I�u�W�F�N�g��Renderer���擾

        float timer = 0f; // �^�C�}�[�����Z�b�g

        while (timer < blinkTime) // 5�b�ԓ_��
        {
            timer += Time.deltaTime;

            // ���ׂẴ����_���[���ꊇ�ŕύX
            foreach (Renderer rend in renderers)
            {
                rend.enabled = Mathf.PingPong(timer, blinkCycle) >= blinkCycle * Center;
            }

            yield return null; // ���̃t���[���܂ő҂�
        }

        // �Ō�ɑS�Ă�\����Ԃɂ���
        foreach (Renderer rend in renderers)
        {
            rend.enabled = true;
        }

        isInvincible = false; // ���G��Ԃ��I�t
    }
}
