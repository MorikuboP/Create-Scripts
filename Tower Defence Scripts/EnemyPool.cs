using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �G�̃I�u�W�F�N�g�v�[�����Ǘ�����N���X�B
/// ���O�Ɉ�萔�̓G�I�u�W�F�N�g�𐶐����A
/// �K�v�ȂƂ��Ƀv�[��������o���A�g�p��ɖ߂����ƂŁA
/// �p�t�H�[�}���X���œK������B
/// </summary>
public class EnemyPool : MonoBehaviour
{
    /// <summary>
    /// �G�̎�ނ��`����N���X�B
    /// </summary>
    [System.Serializable]
    public class EnemyType
    {
        public string _enemyName;          // �G�̖��O�i���ʗp�j
        public GameObject _enemyPrefab;    // ��������G�̃v���n�u
        public int _poolSize;              // �v�[������I�u�W�F�N�g�̐�
    }

    /// <summary>
    /// �V���O���g���C���X�^���X�B
    /// ���̃N���X�̗B��̃C���X�^���X��񋟂���B
    /// </summary>
    public static EnemyPool Instance { get; private set; }

    [SerializeField] private List<EnemyType> _enemyTypes; // �G�̎�ރ��X�g

    private List<GameObject> _activeEnemies = new List<GameObject>(); // �A�N�e�B�u�ȓG���X�g
    private Dictionary<string, Queue<GameObject>> _enemyPools = new Dictionary<string, Queue<GameObject>>(); // �G�̃I�u�W�F�N�g�v�[��

    /// <summary>
    /// �V���O���g���̃Z�b�g�A�b�v�ƃI�u�W�F�N�g�v�[���̏������B
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // �V�[�����ׂ��ŃI�u�W�F�N�g��ێ�
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            // ���ɃC���X�^���X�����݂���ꍇ�͔j��
            Destroy(gameObject); 
            return;
        }

        // �v�[���̏�����
        InitializePools(); 
    }

    /// <summary>
    /// �e�G�^�C�v���ƂɃv�[�����쐬���A�I�u�W�F�N�g�����O�����B
    /// </summary>
    private void InitializePools()
    {
        foreach (EnemyType type in _enemyTypes)
        {
            // �L���[���쐬
            Queue<GameObject> objectPool = new Queue<GameObject>(); 

            // �e�G�^�C�v���Ƃɐ�p�̐e�I�u�W�F�N�g���쐬
            GameObject parent = new GameObject(type._enemyName + "_Pool");
            parent.transform.SetParent(this.transform);

            for (int i = 0; i < type._poolSize; i++)
            {
                // �v���n�u�𐶐�
                GameObject obj = Instantiate(type._enemyPrefab, parent.transform);
                // ������Ԃ͔�A�N�e�B�u
                obj.SetActive(false); 
                // �L���[�ɒǉ�
                objectPool.Enqueue(obj);
            }

            // �쐬�����L���[�������ɓo�^
            _enemyPools[type._enemyName] = objectPool; 
        }
    }

    /// <summary>
    /// �G���v�[������擾���A�w��ʒu�ɔz�u�B
    /// </summary>
    /// <param name="_enemyName">�擾����G�̖��O</param>
    /// <param name="position">�o���ʒu</param>
    /// <returns>�擾�����G��GameObject</returns>
    public GameObject GetEnemy(string _enemyName, Vector2 position)
    {
        if (_enemyPools.ContainsKey(_enemyName) && _enemyPools[_enemyName].Count > 0)
        {
            // �v�[��������o��
            GameObject _enemy = _enemyPools[_enemyName].Dequeue();
            // �w��ʒu�ɔz�u
            _enemy.transform.position = position;
            // �A�N�e�B�u��
            _enemy.SetActive(true);
            // �A�N�e�B�u���X�g�ɒǉ�
            _activeEnemies.Add(_enemy);
            return _enemy;
        }
        else
        {
            Debug.LogWarning($"�G '{_enemyName}' �̃v�[������ł��I");
            return null;
        }
    }

    /// <summary>
    /// �g�p�ς݂̓G���v�[���ɖ߂��B
    /// </summary>
    /// <param name="_enemyName">�߂��G�̖��O</param>
    /// <param name="_enemy">�߂��G��GameObject</param>
    public void ReturnEnemy(string _enemyName, GameObject _enemy)
    {
        if (_enemy == null)
        {
            Debug.LogWarning($"ReturnEnemy: '{_enemyName}' �͊��ɔj������Ă��܂��B");
            return;
        }

        // ��A�N�e�B�u��
        _enemy.SetActive(false);
        // �A�N�e�B�u���X�g����폜
        _activeEnemies.Remove(_enemy); 

        if (_enemyPools.ContainsKey(_enemyName))
        {
            // �v�[���ɖ߂�
            _enemyPools[_enemyName].Enqueue(_enemy);
        }
    }

    /// <summary>
    /// �S�ẴA�N�e�B�u�ȓG���A�N�e�B�u�����A���X�g���N���A�B
    /// </summary>
    public void DespawnAllEnemies()
    {
        for (int i = 0; i < _activeEnemies.Count; i++)
        {
            // �S�Ĕ�A�N�e�B�u��
            _activeEnemies[i].SetActive(false); 
        }
        // ���X�g���N���A
        _activeEnemies.Clear();
    }
}
