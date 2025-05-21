using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;

/// <summary>
/// �e�E�F�[�u�ŏo������G�̏����Ǘ�����N���X
/// </summary>
[System.Serializable]
public class WaveData
{
    /// <summary>
    /// �E�F�[�u���ŏo������G�̃��X�g
    /// </summary>
    public List<EnemySpawnInfo> _enemyList;

    /// <summary>
    /// ���̃E�F�[�u�ł͓G�𐶐������A�ҋ@���邩�ǂ���
    /// </summary>
    public bool _waitInsteadOfSpawn = false;
}

/// <summary>
/// �e�G�̏��i���O�ƒx�����ԁj���i�[����N���X
/// </summary>
[System.Serializable]
public class EnemySpawnInfo
{
    /// <summary>
    /// �G�̖��O�iEnemyPool ����擾����L�[�j
    /// </summary>
    public string _enemyName;

    /// <summary>
    /// �G���o��������܂ł̒x�����ԁi�b�j
    /// </summary>
    public float _delayTime;
}

/// <summary>
/// �G�̃X�|�[�����Ǘ�����N���X
/// </summary>
public class EnemySpawn : MonoBehaviour
{
    /// <summary>
    /// �e�X�|�[���|�C���g���Ƃ̃E�F�[�u�f�[�^���Ǘ�����N���X
    /// </summary>
    [System.Serializable]
    public class SpawnPointWaves
    {
        /// <summary>
        /// �G���o��������X�|�[���n�_
        /// </summary>
        public Transform _spawnPoint;

        /// <summary>
        /// ���̃X�|�[���|�C���g�ł̑S�E�F�[�u�̃f�[�^
        /// </summary>
        public List<WaveData> _wavesdatas;
    }

    [SerializeField] private List<SpawnPointWaves> _spawnPointsData; //���ׂẴX�|�[���|�C���g�̃f�[�^
    [SerializeField] private float _waveInterval = 3f;               //�e�E�F�[�u�̊Ԋu�i�b�j
    [SerializeField] private EnemyPool _enemyPool;                   //�G�̃I�u�W�F�N�g�v�[���ւ̎Q��
    private bool isSpawning = true;                                  //���݂̃X�|�[����ԁifalse �ɂ���ƓG�̏o�����~�j
    private const int _waitTime = 1000;                              //�x�������鎞��

    public EnemyPool Pool
    {
        get { return _enemyPool; }
        set { _enemyPool = value; }
    }

    /// <summary>
    /// ���������i�Q�[���J�n���ɓG�̃E�F�[�u���J�n�j
    /// </summary>
    private async void Start()
    {
        await SpawnWavesAsync();
    }

    /// <summary>
    /// �w��̃X�|�[���|�C���g�ŃE�F�[�u��񓯊��I�ɏ���
    /// </summary>
    private async Task SpawnWaveAtPointAsync(SpawnPointWaves spawnData, int waveIndex)
    {
        if (!isSpawning)
        {
            return;
        }

        // �w�肳�ꂽ�E�F�[�u�����݂��Ȃ��ꍇ�͏������I��
        if (waveIndex >= spawnData._wavesdatas.Count)
        {
            return;
        }

        WaveData wave = spawnData._wavesdatas[waveIndex];

        // �G�𐶐������A�E�F�[�u�Ԋu��ҋ@����ꍇ
        if (wave._waitInsteadOfSpawn)
        {
            await Task.Delay((int)(_waveInterval * _waitTime));
            return;
        }

        List<Task> enemyTasks = new List<Task>();
        float waveDelay = 0f;

        // �e�G�����X�g�̏ォ�珇�ԂɃX�|�[��
        foreach (EnemySpawnInfo enemyInfo in wave._enemyList)
        {
            enemyTasks.Add(SpawnEnemyAsync(enemyInfo, spawnData._spawnPoint, waveDelay));
            waveDelay += enemyInfo._delayTime;
        }

        await Task.WhenAll(enemyTasks);
    }

    /// <summary>
    /// ���ׂẴE�F�[�u�̓G�����ԂɃX�|�[������
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
            //������s�����邽�߂Ƀ��X�g�𐶐�����
            List<Task> waveTasks = new List<Task>();

            foreach (SpawnPointWaves spawnData in _spawnPointsData)
            {
                waveTasks.Add(SpawnWaveAtPointAsync(spawnData, waveIndex));
            }
            //�S�ẴX�|�[���|�C���g�ł̃E�F�[�u�������I������̂�҂�
            await Task.WhenAll(waveTasks);

            if (waveIndex < maxWaveCount - 1)
            {
                await Task.Delay((int)(_waveInterval * _waitTime));
            }
        }
    }

    /// <summary>
    /// �w�肳�ꂽ�G����莞�Ԍ�ɃX�|�[������񓯊����\�b�h
    /// </summary>
    private async Task SpawnEnemyAsync(EnemySpawnInfo enemyInfo, Transform spawnPoint, float delay)
    {
        if (!isSpawning)
        {
            return;
        }

        // �w�莞�Ԃ����ҋ@
        await Task.Delay((int)(delay * _waitTime));

        if (!isSpawning)
        {
            return;
        }

        // �G���v�[������擾
        GameObject enemy = _enemyPool.GetEnemy(enemyInfo._enemyName, spawnPoint.position);

        if (enemy != null)
        {
            enemy.transform.position = spawnPoint.position;
            enemy.SetActive(true);
        }
    }

    /// <summary>
    /// �G�̃X�|�[�����~����
    /// </summary>
    public void StopSpawning()
    {
        isSpawning = false;
    }
}
