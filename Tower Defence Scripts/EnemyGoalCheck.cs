using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGoalCheck : MonoBehaviour
{
    private EnemyMove _enemyMove;     //EnemyMove���擾
    private Rigidbody2D _rigidBody;   //���W�b�g�{�f�B�̎擾

    public static class Tags
    {
        public const string Goal = "Goal";
    }

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _enemyMove = GetComponent<EnemyMove>();
    }
    /// <summary>
    /// �G�̏������Z�b�g����
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        //�G�̈ړ��������Z�b�g���A���Z�b�g�t���O�𗧂Ă�
        if (other.CompareTag(Tags.Goal))
        {
           
            _enemyMove.CorrentPoint = 0;
            _enemyMove.IsResetFlag = true;
        }
    }
}
