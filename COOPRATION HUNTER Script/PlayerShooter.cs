using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{

    [SerializeField] private float interval;
    [SerializeField] private float playerArrowSpeed = default; // 矢の速度
   [SerializeField]  private ArrowPool arrowPool; // 矢のプールの参照
    [SerializeField] Transform shootPoint; // 矢を発射する位置
    private bool _shotflag = default;
    private float timer;
    private BowSpawn spawn;

    public static class Tags
    {
        public const string ArrowManager = "ArrowPool";
    }

    void Start()
    {
        spawn = GetComponent<BowSpawn>();
        // ArrowManager という名前のオブジェクトを探してその中の ArrowPool コンポーネントを取得
        GameObject arrowManager = GameObject.Find(Tags.ArrowManager);
        if (arrowManager != null)
        {
            arrowPool = arrowManager.GetComponent<ArrowPool>(); // ArrowPool コンポーネントを取得
        }
        else
        {
            Debug.LogError("ArrowManager オブジェクトが見つかりません！");
        }

        if (arrowPool == null)
        {
            Debug.LogError("ArrowPool is not assigned!");
        }
        _shotflag = true;
        timer = 0f;
      
    }

    // Update is called once per frame
    void Update()
    {
        // 矢が発射可能で、右クリックが押されたとき
        if (_shotflag && Input.GetMouseButton(0))
        {
            GameObject arrow = arrowPool.GetArrow(); // 矢をプールから取得

            if (arrow != null)
            {
                // 矢の位置と回転を設定
                arrow.transform.position = shootPoint.position;

                // Z軸方向に90度回転
                Quaternion rotation = shootPoint.rotation * Quaternion.Euler(0, 0, 90);
                arrow.transform.rotation = rotation;

                // 矢の動きを開始
                arrow.GetComponent<ArrowManager>().Launch(playerArrowSpeed);

                // 矢を発射したらフラグをfalseに
                _shotflag = false;

                // 発射後のタイマー更新
                timer = 0f;
            }
        }


        if (!_shotflag)
        {
            timer += Time.deltaTime; // 時間を加算

            if (timer >= interval) // インターバルを超えたら発射可能に
            {
                _shotflag = true; // 発射準備OK
            }
        }
    }
}
