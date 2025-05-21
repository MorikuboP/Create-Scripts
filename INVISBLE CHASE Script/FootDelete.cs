using System.Collections;
using UnityEngine;

public class FootDelete : MonoBehaviour
{
    private float fadeDuration = 5.0f; // フェードにかかる時間（秒）
    private int step = 10;            // フェードのステップ数
    private ObjectPool _objectPool;   // プールの参照を保持
    private MeshRenderer _meshRenderer; // キャッシュ用
    /// <summary>
    /// プールを参照
    /// </summary>
    /// <param name="pool"></param>
    public void Initialize(ObjectPool pool)
    {
        // プール参照を設定
        _objectPool = pool; 
    }
    /// <summary>
    /// メッシュレンダラーを取得する
    /// </summary>
    void OnEnable()
    {
        // MeshRenderer をキャッシュ
        _meshRenderer = GetComponent<MeshRenderer>();

        if (_meshRenderer == null)
        {
            Debug.LogError("MeshRendererが見つかりません！");
            return;
        }

        // フェードアウトを開始
        StartCoroutine(Disappearing());
    }
    /// <summary>
    /// 時間経過で足跡を透明にしていく
    /// </summary>
    /// <returns></returns>
    IEnumerator Disappearing()
    {
        float stepDuration = fadeDuration / step; // 各ステップの待機時間
        Material[] materials = _meshRenderer.materials; // マテリアルを取得

        for (int i = 0; i < step; i++)
        {
            float alpha = 1 - (1.0f * i / step);
            foreach (var mat in materials)
            {
                Color color = mat.color;
                color.a = alpha;
                mat.color = color; // アルファ値を更新
            }

            yield return new WaitForSeconds(stepDuration);
        }

        // フェード完了後にプールへ戻す
        if (_objectPool != null)
        {
            _objectPool.ReturnObject(gameObject);
        }
    }
}