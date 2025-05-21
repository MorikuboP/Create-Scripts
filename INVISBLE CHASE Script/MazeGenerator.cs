using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class MazeGenerator : MonoBehaviour
{
    #region Fields
    [SerializeField] private int _flootWidth = default;         //���H�̕�
    [SerializeField] private int _flootHeight = default;        //���H�̍���
    [SerializeField] private int _inkCount = default;           //�G�[�W�F���g�̐�����
    [SerializeField] private GameObject _flootPrefab;           //���̃v���n�u
    [SerializeField] private GameObject _wallPrefab;            //�ǂ̃v���n�u
    [SerializeField] private GameObject _inkPrefab;             //�C���N�̃v���n�u
    [SerializeField] private GameObject enemy;�@�@�@            //�G�[�W�F���g�̃v���n�u
    [SerializeField] private GameObject player;�@�@�@�@�@�@�@   //�v���C���[�̃v���n�u
    [SerializeField] private EnemyNormalMove _normalMove;       //�G�l�~�[�̒ʏ�ړ��X�N���v�g
    private NavMeshSurface _navMeshSurface;                     // �i�r���b�V���T�[�t�F�X��ǉ�
    private int[,] _mazeDate;                                   //���H�̃f�[�^
    private const float _heightAdjustment = 0.7f;               //��������ۂ̍�������
    private const int _minDistance = 10;                         // �v���C���[�ƃG�[�W�F���g���ێ����ׂ��ŏ�����
    private const int _walkblePath = 1;                         //���s�\�ȓ�
    private const int _wallPath = 0;                            //���s�s�ȕ�
    private const int _oddNumber = 2;                           //�������`�F�b�N���邽�߂̒l
    private const int _checkWallCount = 3;                      //���͂S�}�X�̕ǂ̌��̋��e��
    private List<Vector2Int> _pathList = new List<Vector2Int>();//�������ꂽ���̃��X�g
    private List<Vector3> _walkableCells = new List<Vector3>(); // ���s�\�ȃZ���̃��X�g
    private List<Vector3> _inkCells = new List<Vector3>();      //�C���N�̂���}�X�̃��X�g
    #endregion

    #region  Property
    /// <summary>
    /// ���s�\�Z���̃��X�g
    /// </summary>
    public List<Vector3> WalkableCells
    {
        get { return _walkableCells; }
    }
    /// <summary>
    /// �C���N���������ꂽ�Z���̃��X�g
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

        //MeshSurface�̎擾
        _navMeshSurface = GetComponent<NavMeshSurface>();
        //Cell�̃|�W�V�����̐ݒ�
        _mazeDate = new int[_flootWidth, _flootHeight];
        //���H�̏��̐���
        GeneratorMaze();
        //�s���~�܂�����炷
        RemoveDeadEnd();
        //�v���n�u�𐶐�
        InstatiateMaze();
        // �C���N�������_���ɔz�u
        CreateInkRandom(_inkCount);
        // NavMesh���x�C�N�i�o�H�T���f�[�^�쐬�j
        BakeNavMesh();

        // �G��EnemyCount�̔z�u
        PlaceEnemies();
        // �v���C���[��z�u
        PlacePlayer();


    }
    /// <summary>
    /// �[���D��T����p���Ė��H�𐶐�����
    /// </summary>
    private void GeneratorMaze()
    {
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        Vector2Int start = Vector2Int.one;
        _mazeDate[start.x, start.y] = _walkblePath;
        stack.Push(start);
        _pathList.Add(start);

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        // �O�g��ǂɂ���
        for (int x = 0; x < _flootWidth; x++)
        {
            _mazeDate[x, 0] = _wallPath ; // ���̕�
            _mazeDate[x, _flootHeight - 1] = _wallPath; // ��̕�
        }
        for (int y = 0; y < _flootHeight; y++)
        {
            _mazeDate[0, y] = _wallPath; // ���̕�
            _mazeDate[_flootWidth - 1, y] = _wallPath; // �E�̕�
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
    /// �s���~�܂�����炷���߂̏���
    /// </summary>
    private void RemoveDeadEnd()
    {
        bool updated;  // ���[�v���񂷂��߂̃t���O

        do
        {
            updated = false;
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            for (int x = 1; x < _flootWidth - 1; x++)
            {
                for (int y = 1; y < _flootHeight - 1; y++)
                {
                    if (_mazeDate[x, y] == _walkblePath) // �ʘH�������`�F�b�N
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

                        // 2�����ȏオ�ǂȂ�A�J�ʂ�����i�]����3��������2�����ɕύX�j
                        if (wallCount >= _checkWallCount)
                        {
                            _mazeDate[lastWall.x, lastWall.y] = _walkblePath;
                            updated = true; // �ύX�����������Ƃ��L�^
                        }
                    }
                }
            }
        } while (updated); // �ύX���Ȃ��Ȃ�܂ŌJ��Ԃ�
    }


    /// <summary>
    /// �O�����J�x�ɐݒ肵�Ȃ���ǃv���n�u�Ə�
    /// </summary>
    private void InstatiateMaze()
    {
        for (int x = 0; x < _flootWidth; x++)
        {
            for (int y = 0; y < _flootHeight; y++)
            {
                Vector3 position = new Vector3(x, -1, y);

                // �O���͕K���ǂɂ���
                // x == 0 || x == _flootWidth - 1 || y == 0 || y == _flootHeight - 1 �̏������C��
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
    /// �����_���ɃC���N�𐶐�
    /// </summary>
    /// <param name="InkCounts">�C���N�̐�����</param>
    void CreateInkRandom(int InkCounts)
    {
        // �C���N�̐������z�u
        for (int i = 0; i < InkCounts; i++)
        {
            // �z�u�ł���ꏊ���Ȃ��ꍇ�̓��[�v�I��
            if (_walkableCells.Count == 0) break;

            int randomIndex = Random.Range(0, _walkableCells.Count);
            // �����_���Ȉʒu���擾
            Vector3 inkPosition = _walkableCells[randomIndex];
            // �����𒲐�
            inkPosition.y += _heightAdjustment;
            // �C���N�𐶐�
            Instantiate(_inkPrefab, inkPosition, Quaternion.Euler(90, 0, 0));
            // �z�u�����ʒu�����X�g�ɒǉ�
            _inkCells.Add(inkPosition);
            // �g�����Z�������X�g����폜
            _walkableCells.RemoveAt(randomIndex);
        }
    }

    /// <summary>
    /// �G���w�肵�����������s�\�Z���ɔz�u
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

            // �v���C���[�Ƃ̋�����minDistance�����Ȃ�A�V�����ʒu��T��
            while (Vector3.Distance(enemyPosition, player.transform.position) < _minDistance)
            {
                randomIndex = Random.Range(0, _walkableCells.Count);
                enemyPosition = _walkableCells[randomIndex];
            }

            // �G�̔z�u
            enemy.transform.position = enemyPosition + Vector3.up * _heightAdjustment;
            // �z�u�ς݂̃Z�������X�g����폜
            _walkableCells.RemoveAt(randomIndex);
        }

    }

    /// <summary>
    /// �v���C���[�������_���ȕ��s�\�Z���ɔz�u
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

            // �v���C���[���z�u���ꂽ�ꏊ��ێ����A�G�Əd�Ȃ�Ȃ��悤�ɂ���
            player.transform.position = playerPosition + Vector3.up * _heightAdjustment;
            _walkableCells.RemoveAt(randomIndex);
        }
    }
    /// <summary>
    /// �ʒu�����H�͈͓̔������肷��
    /// </summary>
    /// <param name="pos">���̈ʒu</param>
    /// <returns></returns>
    bool IsInside(Vector2Int pos)
    {
        // X���W���t���A�͈͓̔����ǂ������m�F
        bool insideX = pos.x > 0 && pos.x < _flootWidth - 1;

        // Y���W���t���A�͈͓̔����ǂ������m�F
        bool insideY = pos.y > 0 && pos.y < _flootHeight - 1;

        // X���W��Y���W�������͈͓��ł���΁A�t���A�͈͓̔��Ɉʒu���Ă���
        if (insideX && insideY)
        {
            // �͈͓��Ɉʒu���Ă���ꍇ��true��Ԃ�
            return true; 
        }

        // �͈͊O�̏ꍇ��false��Ԃ�
        return false;  
    }

    /// <summary>
    /// NavMesh���x�C�N���Čo�H�T���f�[�^�𐶐�
    /// </summary>
    void BakeNavMesh()
    {
        if (_navMeshSurface != null)
        {
            _navMeshSurface.BuildNavMesh();
        }
    }


}
