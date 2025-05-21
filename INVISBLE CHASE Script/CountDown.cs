using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CountDown : MonoBehaviour
{
    #region Fields
    [SerializeField] private float _initialCountDownTime = 3f;   // 最初のカウントダウン時間
    [SerializeField] private float _mainCountDownTime = 30f;     // ゲームの制限時間
    [SerializeField] private Text _initialCountText;             // 最初のカウントダウン表示用UI
    [SerializeField] private Text _mainCountText;                // メインカウントダウン表示用UI
    [SerializeField] private EnemyMoveUpdater _updater;
    [SerializeField] private EnemyNormalMove _normalMove;
    [SerializeField] private FootPrint _foot;                    // 足跡のシステム（？）を制御するスクリプト
    [SerializeField] private PlayerMove _player;                 // プレイヤーの移動を制御するスクリプト
    private float LimitTime = 10f;                               // 敵が手加減を開始する時間（制限時間が10秒以下になったら）
    private bool isInitialCountDownFinished = false;             // 最初のカウントダウンが終了したかのフラグ
    private float _previousInitialTime = -1f;                    // 最初のカウントダウンの前回の表示時間（最適化用）
    private float _previousMainTime = -1f;                       // メインカウントダウンの前回の表示時間（最適化用）
    #endregion

    private void Start()
    {
        // 初期カウントダウンとメインカウントダウンのUIを設定
        UpdateInitialCountText(_initialCountDownTime);
        UpdateMainCountText(_mainCountDownTime);
    }

    private void Update()
    {
        if (!isInitialCountDownFinished)
        {
            // 最初のカウントダウンを実行
            InitialCountDown();
        }
        else
        {
            // メインのカウントダウンを実行
            MainCountDown();
        }
    }
    /// <summary>
    /// 最初の３カウント
    /// ３カウント後、エージェント、プレイヤーを行動できるようにする
    /// </summary>
    private void InitialCountDown()
    {
        _initialCountDownTime -= Time.deltaTime; // 時間を減らす

        if (_initialCountDownTime <= 0)
        {
            // カウントダウン終了時の処理
            isInitialCountDownFinished = true;
            _initialCountText.text = ""; // UIを非表示にする

            // プレイヤーと敵の移動を有効にする
            _normalMove.IsMove = true;
            _player.IsMove = true;
            return;
        }

        // カウントダウンUIを更新
        UpdateInitialCountText(_initialCountDownTime);
    }
    /// <summary>
    /// 制限時間カウント
    /// LimitTime以下になったら手加減モードを発動する
    /// </summary>
    private void MainCountDown()
    {
        // 時間を減らす
        _mainCountDownTime -= Time.deltaTime;

        // 制限時間がLimitTime以下になったら敵の行動を変更
        if (_mainCountDownTime <= LimitTime)
        {
            // 敵を手加減モードにする
            _updater.LaxityMode();
            // 足跡の設定を変更
            _foot.GetSetFootBool = true;
        }

        // 制限時間が0になったらゲームオーバーシーンへ遷移
        if (_mainCountDownTime <= 0)
        {
            SceneManager.LoadScene("GameOverScene");
            return;
        }

        // カウントダウンUIを更新
        UpdateMainCountText(_mainCountDownTime);
    }

    /// <summary>
    /// 最初のカウントダウンUIの更新処理
    /// </summary>
    /// <param name="time"></param>
    private void UpdateInitialCountText(float time)
    {
        // 小数点切り上げ
        int newTime = Mathf.CeilToInt(time);
        // 前回と値が変わったときだけ更新（無駄な処理を減らす）
        if (newTime != _previousInitialTime) 
        {
            _initialCountText.text = newTime.ToString();
            _previousInitialTime = newTime;
        }
    }

    /// <summary>
    /// メインカウントダウンUIの更新処理
    /// </summary>
    /// <param name="time"></param>
    private void UpdateMainCountText(float time)
    {
        // 小数点切り上げ
        int newTime = Mathf.CeilToInt(time);
        // 前回と値が変わったときだけ更新（無駄な処理を減らす）
        if (newTime != _previousMainTime) 
        {
            _mainCountText.text = newTime.ToString();
            _previousMainTime = newTime;
        }
    }
}
