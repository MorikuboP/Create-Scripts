using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �G��ǐՂ��A�S�[�������Ɍ������Ă���G�����ʂ���N���X�B
/// �w�肵���S�[���I�u�W�F�N�g�̈ʒu����ɁA
/// �g���K�[�ɓ������G���S�[���Ɍ������Ă��邩�ǂ����𔻒肷��B
/// </summary>
public class EnemyTracker : MonoBehaviour
{
    [SerializeField] private GameObject _goalPrefab;                   // �S�[���n�_�̃v���n�u
    [SerializeField] private bool _changeAxis = false;                 // false = X�����, true = Y�����

    private TowerAttack _towerAttack;
    private List<GameObject> _allEnemies = new List<GameObject>();     // ���o���ꂽ�S�Ă̓G
    private List<GameObject> _targetEnemies = new List<GameObject>();  // �S�[�������Ɍ������Ă���G
    private Vector2Int _goalPosition;                                  // �S�[���̍��W
    private Vector2Int _myPosition;                                    // ���̃I�u�W�F�N�g�̍��W
    private int _goalDirection;                                        // �S�[���ւ̕����i1 or -1�j

    /// <summary>
    /// ���݃g���b�L���O���Ă���S�Ă̓G���X�g���擾�B
    /// </summary>
    public List<GameObject> GetAllEnemies => _allEnemies;

    /// <summary>
    /// �S�[���Ɍ������Ă���G�̃��X�g���擾�B
    /// </summary>
    public List<GameObject> GetTargetEnemies => _targetEnemies;

    /// <summary>
    /// �Q�[�����œG�����ʂ���^�O���Ǘ�����N���X�B
    /// </summary>
    public static class Tags
    {
        public static string Enemy = "Enemy";
    }

    /// <summary>
    /// �����������B�S�[���̈ʒu�Ǝ��g�̈ʒu���擾���A
    /// �S�[���ւ̕������v�Z����B
    /// </summary>
    private void Start()
    {
        _towerAttack = GetComponent<TowerAttack>();

        Transform goalTransform = _goalPrefab.transform;
        _goalPosition = new Vector2Int(Mathf.FloorToInt(goalTransform.position.x), Mathf.FloorToInt(goalTransform.position.y));
        _myPosition = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));

        // �S�[�����ǂ̕����ɂ��邩������iX�� or Y���j
        _goalDirection = _changeAxis
            ? (_goalPosition.y > _myPosition.y ? 1 : -1) // Y���̕�������
            : (_goalPosition.x > _myPosition.x ? 1 : -1); // X���̕�������

    }

    /// <summary>
    /// �g���K�[�ɓG���������Ƃ��ɌĂ΂��B
    /// �G�����X�g�ɒǉ����A�S�[�������Ɍ������Ă��邩����B
    /// </summary>
    /// <param name="other">�g���K�[�ɓ������I�u�W�F�N�g</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Tags.Enemy) && !_allEnemies.Contains(other.gameObject))
        {
            _allEnemies.Add(other.gameObject);
            CheckEnemyDirection(other.gameObject);
        }
    }

    /// <summary>
    /// �g���K�[����G���o���Ƃ��ɌĂ΂��B
    /// �G�����X�g����폜�B
    /// </summary>
    /// <param name="other">�g���K�[����o���I�u�W�F�N�g</param>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(Tags.Enemy))
        {
            _allEnemies.Remove(other.gameObject);
            _targetEnemies.Remove(other.gameObject);
        }
    }

    /// <summary>
    /// ���t���[���A�g���b�L���O���Ă���G��
    /// �S�[�������Ɍ������Ă��邩���`�F�b�N�B
    /// </summary>
    private void Update()
    {
        for (int i = _allEnemies.Count - 1; i >= 0; i--)
        {
            CheckEnemyDirection(_allEnemies[i]);
        }
    }

    /// <summary>
    /// �G���S�[�������Ɍ������Ă��邩�𔻒�B
    /// </summary>
    /// <param name="enemy">�`�F�b�N����G��GameObject</param>
    private void CheckEnemyDirection(GameObject enemy)
    {
        // �G���j������Ă����珈�����I��
        if (enemy == null)
        {
            return;
        }


        Vector2Int enemyPosition = new Vector2Int(Mathf.FloorToInt(enemy.transform.position.x), Mathf.FloorToInt(enemy.transform.position.y));

        // �G���S�[�������ɂ��邩�ǂ����𔻒�
        bool isInGoalDirection = _changeAxis
            ? (enemyPosition.y - _myPosition.y) * _goalDirection > 0 // Y���Ŕ���
            : (enemyPosition.x - _myPosition.x) * _goalDirection > 0; // X���Ŕ���

        if (isInGoalDirection)
        {
            // �܂����X�g�ɓ����Ă��Ȃ���Βǉ�
            if (!_targetEnemies.Contains(enemy))
            {
                _targetEnemies.Add(enemy);
                _towerAttack.SelectNewTarget();
            }
        }
        else
        {
            // �S�[�������ɂ��Ȃ��Ȃ����ꍇ�̓��X�g����폜
            _targetEnemies.Remove(enemy);
            _towerAttack.SelectNewTarget();
        }
    }
}
