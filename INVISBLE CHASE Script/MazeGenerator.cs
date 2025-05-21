using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class MazeGenerator : MonoBehaviour
{
    #region Fields
    [SerializeField] private int _flootWidth = default;         //迷路の幅
    [SerializeField] private int _flootHeight = default;        //迷路の高さ
    [SerializeField] private int _inkCount = default;           //エージェントの生成数
    [SerializeField] private GameObject _flootPrefab;           //床のプレハブ
    [SerializeField] private GameObject _wallPrefab;            //壁のプレハブ
    [SerializeField] private GameObject _inkPrefab;             //インクのプレハブ
    [SerializeField] private GameObject enemy;　　　            //エージェントのプレハブ
    [SerializeField] private GameObject player;　　　　　　　   //プレイヤーのプレハブ
    [SerializeField] private EnemyNormalMove _normalMove;       //エネミーの通常移動スクリプト
    private NavMeshSurface _navMeshSurface;                     // ナビメッシュサーフェスを追加
    private int[,] _mazeDate;                                   //迷路のデータ
    private const float _heightAdjustment = 0.7f;               //生成する際の高さ調節
    private const int _minDistance = 10;                         // プレイヤーとエージェントが保持すべき最小距離
    private const int _walkblePath = 1;                         //歩行可能な道
    private const int _wallPath = 0;                            //歩行不可な壁
    private const int _oddNumber = 2;                           //偶数かチェックするための値
    private const int _checkWallCount = 3;                      //周囲４マスの壁の個数の許容個数
    private List<Vector2Int> _pathList = new List<Vector2Int>();//生成された道のリスト
    private List<Vector3> _walkableCells = new List<Vector3>(); // 歩行可能なセルのリスト
    private List<Vector3> _inkCells = new List<Vector3>();      //インクのあるマスのリスト
    #endregion

    #region  Property
    /// <summary>
    /// 歩行可能セルのリスト
    /// </summary>
    public List<Vector3> WalkableCells
    {
        get { return _walkableCells; }
    }
    /// <summary>
    /// インクが生成されたセルのリスト
    /// </summary>
    public List<Vector3> InkCells
    {
        get { return _inkCells; }
    }
    #endregion

    void Start()
    {

        if(_flootWidth % _oddNumber == 0)
        {
            _flootWidth++;
        }

        if (_flootHeight % _oddNumber == 0)
        {
            _flootHeight++;
        }

        //MeshSurfaceの取得
        _navMeshSurface = GetComponent<NavMeshSurface>();
        //Cellのポジションの設定
        _mazeDate = new int[_flootWidth, _flootHeight];
        //迷路の情報の生成
        GeneratorMaze();
        //行き止まりを減らす
        RemoveDeadEnd();
        //プレハブを生成
        InstatiateMaze();
        // インクをランダムに配置
        CreateInkRandom(_inkCount);
        // NavMeshをベイク（経路探索データ作成）
        BakeNavMesh();

        // 敵をEnemyCount体配置
        PlaceEnemies();
        // プレイヤーを配置
        PlacePlayer();


    }
    /// <summary>
    /// 深さ優先探索を用いて迷路を生成する
    /// </summary>
    private void GeneratorMaze()
    {
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        Vector2Int start = Vector2Int.one;
        _mazeDate[start.x, start.y] = _walkblePath;
        stack.Push(start);
        _pathList.Add(start);

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        // 外枠を壁にする
        for (int x = 0; x < _flootWidth; x++)
        {
            _mazeDate[x, 0] = _wallPath ; // 下の壁
            _mazeDate[x, _flootHeight - 1] = _wallPath; // 上の壁
        }
        for (int y = 0; y < _flootHeight; y++)
        {
            _mazeDate[0, y] = _wallPath; // 左の壁
            _mazeDate[_flootWidth - 1, y] = _wallPath; // 右の壁
        }


        while (stack.Count > 0)
        {
            Vector2Int current = stack.Peek();
            List<Vector2Int> neighbors = new List<Vector2Int>();

            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = current + dir * 2;
                if (IsInside(next) && _mazeDate[next.x, next.y] == _wallPath)
                {
                    neighbors.Add(next);
                }
            }

            if (neighbors.Count > 0)
            {
                Vector2Int dig = neighbors[Random.Range(0, neighbors.Count)];
                _mazeDate[dig.x, dig.y] = _walkblePath;
                _mazeDate[(dig.x + current.x) / 2, (dig.y + current.y) / 2] = _walkblePath;
                _pathList.Add(dig);
                stack.Push(dig);
            }
            else
            {
                stack.Pop();
            }
        }
    }
    /// <summary>
    /// 行き止まりを減らすための処理
    /// </summary>
    private void RemoveDeadEnd()
    {
        bool updated;  // ループを回すためのフラグ

        do
        {
            updated = false;
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            for (int x = 1; x < _flootWidth - 1; x++)
            {
                for (int y = 1; y < _flootHeight - 1; y++)
                {
                    if (_mazeDate[x, y] == _walkblePath) // 通路部分をチェック
                    {
                        int wallCount = 0;
                        Vector2Int lastWall = Vector2Int.zero;

                        foreach (Vector2Int dir in directions)
                        {
                            Vector2Int neighbor = new Vector2Int(x, y) + dir;
                            if (_mazeDate[neighbor.x, neighbor.y] == _wallPath)
                            {
                                wallCount++;
                                lastWall = neighbor;
                            }
                        }

                        // 2方向以上が壁なら、開通させる（従来の3方向から2方向に変更）
                        if (wallCount >= _checkWallCount)
                        {
                            _mazeDate[lastWall.x, lastWall.y] = _walkblePath;
                            updated = true; // 変更があったことを記録
                        }
                    }
                }
            }
        } while (updated); // 変更がなくなるまで繰り返す
    }


    /// <summary>
    /// 外周をカベに設定しながら壁プレハブと床
    /// </summary>
    private void InstatiateMaze()
    {
        for (int x = 0; x < _flootWidth; x++)
        {
            for (int y = 0; y < _flootHeight; y++)
            {
                Vector3 position = new Vector3(x, -1, y);

                // 外周は必ず壁にする
                // x == 0 || x == _flootWidth - 1 || y == 0 || y == _flootHeight - 1 の条件を修正
                if (x == 0 || x == _flootWidth - 1 || y == 0 || y == _flootHeight - 1||_mazeDate[x,y]==_wallPath)
                {
                    Instantiate(_wallPrefab, new Vector3(x, 0, y), Quaternion.identity);
                }
                else
                {
                    Instantiate(_flootPrefab, position, Quaternion.identity);
                    _walkableCells.Add(position);
                }
            }
        }

        _normalMove.GridCheack = true;
    }

    /// <summary>
    /// ランダムにインクを生成
    /// </summary>
    /// <param name="InkCounts">インクの生成数</param>
    void CreateInkRandom(int InkCounts)
    {
        // インクの数だけ配置
        for (int i = 0; i < InkCounts; i++)
        {
            // 配置できる場所がない場合はループ終了
            if (_walkableCells.Count == 0) break;

            int randomIndex = Random.Range(0, _walkableCells.Count);
            // ランダムな位置を取得
            Vector3 inkPosition = _walkableCells[randomIndex];
            // 高さを調整
            inkPosition.y += _heightAdjustment;
            // インクを生成
            Instantiate(_inkPrefab, inkPosition, Quaternion.Euler(90, 0, 0));
            // 配置した位置をリストに追加
            _inkCells.Add(inkPosition);
            // 使ったセルをリストから削除
            _walkableCells.RemoveAt(randomIndex);
        }
    }

    /// <summary>
    /// 敵を指定した数だけ歩行可能セルに配置
    /// </summary>
    void PlaceEnemies()
    {
        if(_walkableCells.Count == 0)
        {
            return;
        }

        if (_walkableCells.Count > 0)
        {
            int randomIndex = Random.Range(0, _walkableCells.Count);
            Vector3 enemyPosition = _walkableCells[randomIndex];

            // プレイヤーとの距離がminDistance未満なら、新しい位置を探す
            while (Vector3.Distance(enemyPosition, player.transform.position) < _minDistance)
            {
                randomIndex = Random.Range(0, _walkableCells.Count);
                enemyPosition = _walkableCells[randomIndex];
            }

            // 敵の配置
            enemy.transform.position = enemyPosition + Vector3.up * _heightAdjustment;
            // 配置済みのセルをリストから削除
            _walkableCells.RemoveAt(randomIndex);
        }

    }

    /// <summary>
    /// プレイヤーをランダムな歩行可能セルに配置
    /// </summary>
    void PlacePlayer()
    {
        if (_walkableCells.Count == 0)
        {
            return;
        }

        if (_walkableCells.Count > 0)
        {
            int randomIndex = Random.Range(0, _walkableCells.Count);
            Vector3 playerPosition = _walkableCells[randomIndex];

            // プレイヤーが配置された場所を保持し、敵と重ならないようにする
            player.transform.position = playerPosition + Vector3.up * _heightAdjustment;
            _walkableCells.RemoveAt(randomIndex);
        }
    }
    /// <summary>
    /// 位置が迷路の範囲内か判定する
    /// </summary>
    /// <param name="pos">今の位置</param>
    /// <returns></returns>
    bool IsInside(Vector2Int pos)
    {
        // X座標がフロアの範囲内かどうかを確認
        bool insideX = pos.x > 0 && pos.x < _flootWidth - 1;

        // Y座標がフロアの範囲内かどうかを確認
        bool insideY = pos.y > 0 && pos.y < _flootHeight - 1;

        // X座標とY座標両方が範囲内であれば、フロアの範囲内に位置している
        if (insideX && insideY)
        {
            // 範囲内に位置している場合はtrueを返す
            return true; 
        }

        // 範囲外の場合はfalseを返す
        return false;  
    }

    /// <summary>
    /// NavMeshをベイクして経路探索データを生成
    /// </summary>
    void BakeNavMesh()
    {
        if (_navMeshSurface != null)
        {
            _navMeshSurface.BuildNavMesh();
        }
    }


}
