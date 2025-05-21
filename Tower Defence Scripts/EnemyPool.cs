using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵のオブジェクトプールを管理するクラス。
/// 事前に一定数の敵オブジェクトを生成し、
/// 必要なときにプールから取り出し、使用後に戻すことで、
/// パフォーマンスを最適化する。
/// </summary>
public class EnemyPool : MonoBehaviour
{
    /// <summary>
    /// 敵の種類を定義するクラス。
    /// </summary>
    [System.Serializable]
    public class EnemyType
    {
        public string _enemyName;          // 敵の名前（識別用）
        public GameObject _enemyPrefab;    // 生成する敵のプレハブ
        public int _poolSize;              // プールするオブジェクトの数
    }

    /// <summary>
    /// シングルトンインスタンス。
    /// このクラスの唯一のインスタンスを提供する。
    /// </summary>
    public static EnemyPool Instance { get; private set; }

    [SerializeField] private List<EnemyType> _enemyTypes; // 敵の種類リスト

    private List<GameObject> _activeEnemies = new List<GameObject>(); // アクティブな敵リスト
    private Dictionary<string, Queue<GameObject>> _enemyPools = new Dictionary<string, Queue<GameObject>>(); // 敵のオブジェクトプール

    /// <summary>
    /// シングルトンのセットアップとオブジェクトプールの初期化。
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // シーンを跨いでオブジェクトを保持
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            // 既にインスタンスが存在する場合は破棄
            Destroy(gameObject); 
            return;
        }

        // プールの初期化
        InitializePools(); 
    }

    /// <summary>
    /// 各敵タイプごとにプールを作成し、オブジェクトを事前生成。
    /// </summary>
    private void InitializePools()
    {
        foreach (EnemyType type in _enemyTypes)
        {
            // キューを作成
            Queue<GameObject> objectPool = new Queue<GameObject>(); 

            // 各敵タイプごとに専用の親オブジェクトを作成
            GameObject parent = new GameObject(type._enemyName + "_Pool");
            parent.transform.SetParent(this.transform);

            for (int i = 0; i < type._poolSize; i++)
            {
                // プレハブを生成
                GameObject obj = Instantiate(type._enemyPrefab, parent.transform);
                // 初期状態は非アクティブ
                obj.SetActive(false); 
                // キューに追加
                objectPool.Enqueue(obj);
            }

            // 作成したキューを辞書に登録
            _enemyPools[type._enemyName] = objectPool; 
        }
    }

    /// <summary>
    /// 敵をプールから取得し、指定位置に配置。
    /// </summary>
    /// <param name="_enemyName">取得する敵の名前</param>
    /// <param name="position">出現位置</param>
    /// <returns>取得した敵のGameObject</returns>
    public GameObject GetEnemy(string _enemyName, Vector2 position)
    {
        if (_enemyPools.ContainsKey(_enemyName) && _enemyPools[_enemyName].Count > 0)
        {
            // プールから取り出す
            GameObject _enemy = _enemyPools[_enemyName].Dequeue();
            // 指定位置に配置
            _enemy.transform.position = position;
            // アクティブ化
            _enemy.SetActive(true);
            // アクティブリストに追加
            _activeEnemies.Add(_enemy);
            return _enemy;
        }
        else
        {
            Debug.LogWarning($"敵 '{_enemyName}' のプールが空です！");
            return null;
        }
    }

    /// <summary>
    /// 使用済みの敵をプールに戻す。
    /// </summary>
    /// <param name="_enemyName">戻す敵の名前</param>
    /// <param name="_enemy">戻す敵のGameObject</param>
    public void ReturnEnemy(string _enemyName, GameObject _enemy)
    {
        if (_enemy == null)
        {
            Debug.LogWarning($"ReturnEnemy: '{_enemyName}' は既に破棄されています。");
            return;
        }

        // 非アクティブ化
        _enemy.SetActive(false);
        // アクティブリストから削除
        _activeEnemies.Remove(_enemy); 

        if (_enemyPools.ContainsKey(_enemyName))
        {
            // プールに戻す
            _enemyPools[_enemyName].Enqueue(_enemy);
        }
    }

    /// <summary>
    /// 全てのアクティブな敵を非アクティブ化し、リストをクリア。
    /// </summary>
    public void DespawnAllEnemies()
    {
        for (int i = 0; i < _activeEnemies.Count; i++)
        {
            // 全て非アクティブ化
            _activeEnemies[i].SetActive(false); 
        }
        // リストをクリア
        _activeEnemies.Clear();
    }
}
