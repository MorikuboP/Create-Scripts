using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class EnemyHitPoint : MonoBehaviour
{
    [SerializeField] private int _EnemyHP;         //�G��HP��ݒ肷��
    [SerializeField] private Slider _hpSlider;     //�GHP�̃X���C�_�[�̐ݒ�
    [SerializeField] private string _enemyName;    //�G�̖��O�̐ݒ�
    [SerializeField] private EnemyPool _enemyPool; //�G�̂Ձ[����擾
    [SerializeField] private bool _isBoss = false; // �{�X���ǂ����̃t���O
    private int _currentEnemyHP;                   //���݂̓G��HP
    private EnemyTracker _enemyTracker;            //EnemyTracker�̎擾

    // �{�X���|�ꂽ���Ƃ�ʒm����C�x���g
    public static event Action _onBossDefeated;

    private void Start()
    {
        _currentEnemyHP = _EnemyHP;
        _hpSlider.maxValue = _EnemyHP;
        _hpSlider.value = _EnemyHP;

        _enemyTracker = FindObjectOfType<EnemyTracker>();
    }

    private void Update()
    {
        //�X���C�_�[�̈ʒu���Œ肷��
        if (_hpSlider != null)
        {
            Vector3 _screenPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1, 0));
            _hpSlider.transform.position = _screenPos;
        }
    }
    /// <summary>
    /// �G��HP�����炵�A�X���C�_�[�ɔ��f������
    /// </summary>
    /// <param name="_BulletDamage"></param>
    public void TakeDamage(int _BulletDamage)
    {
        _currentEnemyHP -= _BulletDamage;
        _hpSlider.value = _currentEnemyHP;

        //HP���O�ɂȂ�����G�����ꂽ���̓����ɓ���
        if (_currentEnemyHP <= 0)
        {
            DestroyEnemy();
        }
    }
    /// <summary>
    /// ���ꂽ���Ƃ̏���
    /// </summary>
    private void DestroyEnemy()
    {
        //�G�����X�g����Ȃ���
        if (_enemyTracker != null)
        {
            _enemyTracker.GetAllEnemies.Remove(gameObject);
            _enemyTracker.GetTargetEnemies.Remove(gameObject);
        }

        // �{�X�������ꍇ�A�C�x���g�𔭍s
        if (_isBoss)
        {
            _onBossDefeated?.Invoke(); // �{�X���j�C�x���g�𔭐�
        }


        // �I�u�W�F�N�g���v�[���ɖ߂�
        EnemyPool.Instance.ReturnEnemy(_enemyName, gameObject);
    }

}
