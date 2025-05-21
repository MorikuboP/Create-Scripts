using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathCheck : MonoBehaviour
{
    [SerializeField] private int _enemyHP;  //�G��HP��ݒ�
    private Rigidbody2D _rigidbody;         //���W�b�g�{�f�B���擾
    private EnemyMove _enemyMove;           //EnemyMove���擾

    public static class Tags
    {
        public const string Attack = "Attack";
    }

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _enemyMove = GetComponent<EnemyMove>();
    }

    // Update is called once per frame
    void Update()
    {
        //�G�̈ړ��������Z�b�g���A���Z�b�g�t���O�𗧂Ă�
        if(_enemyHP <= 0)
        {
            _enemyMove.CorrentPoint = 0;
            _enemyMove.IsResetFlag = true;
        }
    }
    /// <summary>
    /// �G��HP�����炷
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter2D(Collision2D other)
    {
        //�GHP�����炷
        if(other.gameObject.CompareTag(Tags.Attack))
        {
            _enemyHP--;
        }
    }
}
