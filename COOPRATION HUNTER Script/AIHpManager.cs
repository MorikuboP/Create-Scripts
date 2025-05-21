using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AIHpManager : MonoBehaviour
{
    [SerializeField] private float aiHp = 5f;            // AIのHP
    [SerializeField] private Text hpText;                // HPを表示するUIテキスト
    [SerializeField] private Renderer _renderer;
    private bool isInvincible = false;                  // 無敵状態のフラグ
    private float blinkCycle = 0.4f;                             //点滅周期
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
        // HPが0になったらオブジェクトを非アクティブ化し、GameOverSceneをロード
        if (aiHp <= 0)
        {
            StartCoroutine(LoadGameOverScene()); // シーン遷移を開始
        }

    }
    /// <summary>
    /// Enemyにぶつかった時の処理
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        // "Enemy"タグのオブジェクトと衝突した場合
        if (collision.gameObject.CompareTag(Tags.Enemy) && !isInvincible)
        {
            aiHp--; // HPを減らす
            UpdateHpUI();
            StartCoroutine(ActivateInvincibility()); // 無敵状態を開始
        }
    }
    /// <summary>
    /// HPのUIのアップデート
    /// </summary>
    private void UpdateHpUI()
    {
        hpText.text = aiHp.ToString(); // HPをUIに表示
    }
    /// <summary>
    /// シーン遷移の処理
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadGameOverScene()
    {
        //スローモーションにする
        Time.timeScale = 0.2f;
        //１秒間待つ
        yield return new WaitForSecondsRealtime(1f);
        //1時間を元に戻す
        Time.timeScale = 1f;
        //シーンをロード
        SceneManager.LoadScene(Tags.GameOver, LoadSceneMode.Single);
    }

    /// <summary>
    /// 無敵時間とキャラの点滅処理
    /// </summary>
    /// <returns></returns>
    private IEnumerator ActivateInvincibility()
    {
        isInvincible = true; // 無敵状態をオン

        Renderer[] renderers = GetComponentsInChildren<Renderer>(); // 自分と子オブジェクトのRendererを取得

        float timer = 0f; // タイマーをリセット

        while (timer < blinkTime) // 5秒間点滅
        {
            timer += Time.deltaTime;

            // すべてのレンダラーを一括で変更
            foreach (Renderer rend in renderers)
            {
                rend.enabled = Mathf.PingPong(timer, blinkCycle) >= blinkCycle * Center;
            }

            yield return null; // 次のフレームまで待つ
        }

        // 最後に全てを表示状態にする
        foreach (Renderer rend in renderers)
        {
            rend.enabled = true;
        }

        isInvincible = false; // 無敵状態をオフ
    }
}
