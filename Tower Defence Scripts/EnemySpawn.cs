using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;

/// <summary>
/// 各ウェーブで出現する敵の情報を管理するクラス
/// </summary>
[System.Serializable]
public class WaveData
{
    /// <summary>
    /// ウェーブ内で出現する敵のリスト
    /// </summary>
    public List<EnemySpawnInfo> _enemyList;

    /// <summary>
    /// このウェーブでは敵を生成せず、待機するかどうか
    /// </summary>
    public bool _waitInsteadOfSpawn = false;
}

/// <summary>
/// 各敵の情報（名前と遅延時間）を格納するクラス
/// </summary>
[System.Serializable]
public class EnemySpawnInfo
{
    /// <summary>
    /// 敵の名前（EnemyPool から取得するキー）
    /// </summary>
    public string _enemyName;

    /// <summary>
    /// 敵を出現させるまでの遅延時間（秒）
    /// </summary>
    public float _delayTime;
}

/// <summary>
/// 敵のスポーンを管理するクラス
/// </summary>
public class EnemySpawn : MonoBehaviour
{
    /// <summary>
    /// 各スポーンポイントごとのウェーブデータを管理するクラス
    /// </summary>
    [System.Serializable]
    public class SpawnPointWaves
    {
        /// <summary>
        /// 敵を出現させるスポーン地点
        /// </summary>
        public Transform _spawnPoint;

        /// <summary>
        /// このスポーンポイントでの全ウェーブのデータ
        /// </summary>
        public List<WaveData> _wavesdatas;
    }

    [SerializeField] private List<SpawnPointWaves> _spawnPointsData; //すべてのスポーンポイントのデータ
    [SerializeField] private float _waveInterval = 3f;               //各ウェーブの間隔（秒）
    [SerializeField] private EnemyPool _enemyPool;                   //敵のオブジェクトプールへの参照
    private bool isSpawning = true;                                  //現在のスポーン状態（false にすると敵の出現を停止）
    private const int _waitTime = 1000;                              //遅延させる時間

    public EnemyPool Pool
    {
        get { return _enemyPool; }
        set { _enemyPool = value; }
    }

    /// <summary>
    /// 初期処理（ゲーム開始時に敵のウェーブを開始）
    /// </summary>
    private async void Start()
    {
        await SpawnWavesAsync();
    }

    /// <summary>
    /// 指定のスポーンポイントでウェーブを非同期的に処理
    /// </summary>
    private async Task SpawnWaveAtPointAsync(SpawnPointWaves spawnData, int waveIndex)
    {
        if (!isSpawning)
        {
            return;
        }

        // 指定されたウェーブが存在しない場合は処理を終了
        if (waveIndex >= spawnData._wavesdatas.Count)
        {
            return;
        }

        WaveData wave = spawnData._wavesdatas[waveIndex];

        // 敵を生成せず、ウェーブ間隔を待機する場合
        if (wave._waitInsteadOfSpawn)
        {
            await Task.Delay((int)(_waveInterval * _waitTime));
            return;
        }

        List<Task> enemyTasks = new List<Task>();
        float waveDelay = 0f;

        // 各敵をリストの上から順番にスポーン
        foreach (EnemySpawnInfo enemyInfo in wave._enemyList)
        {
            enemyTasks.Add(SpawnEnemyAsync(enemyInfo, spawnData._spawnPoint, waveDelay));
            waveDelay += enemyInfo._delayTime;
        }

        await Task.WhenAll(enemyTasks);
    }

    /// <summary>
    /// すべてのウェーブの敵を順番にスポーンする
    /// </summary>
    private async Task SpawnWavesAsync()
    {
        int maxWaveCount = _spawnPointsData.Max(sp => sp._wavesdatas.Count);

        for (int waveIndex = 0; waveIndex < maxWaveCount; waveIndex++)
        {
            if (!isSpawning)
            {
                break;
            }
            //並列実行をするためにリストを生成する
            List<Task> waveTasks = new List<Task>();

            foreach (SpawnPointWaves spawnData in _spawnPointsData)
            {
                waveTasks.Add(SpawnWaveAtPointAsync(spawnData, waveIndex));
            }
            //全てのスポーンポイントでのウェーブ処理が終了するのを待つ
            await Task.WhenAll(waveTasks);

            if (waveIndex < maxWaveCount - 1)
            {
                await Task.Delay((int)(_waveInterval * _waitTime));
            }
        }
    }

    /// <summary>
    /// 指定された敵を一定時間後にスポーンする非同期メソッド
    /// </summary>
    private async Task SpawnEnemyAsync(EnemySpawnInfo enemyInfo, Transform spawnPoint, float delay)
    {
        if (!isSpawning)
        {
            return;
        }

        // 指定時間だけ待機
        await Task.Delay((int)(delay * _waitTime));

        if (!isSpawning)
        {
            return;
        }

        // 敵をプールから取得
        GameObject enemy = _enemyPool.GetEnemy(enemyInfo._enemyName, spawnPoint.position);

        if (enemy != null)
        {
            enemy.transform.position = spawnPoint.position;
            enemy.SetActive(true);
        }
    }

    /// <summary>
    /// 敵のスポーンを停止する
    /// </summary>
    public void StopSpawning()
    {
        isSpawning = false;
    }
}
