using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitAreaEnemy : MonoBehaviour
{ 
    [SerializeField] GameObject hittarget;

    private MonsterMoveAI move;

    private GameObject target;  // �v���C���[�Ȃǂ̃^�[�Q�b�g

    public static class Tags
    {
        public const string AI = "AI";
    }

    private void Start()
    {
        move = GetComponent<MonsterMoveAI>();   // ObjectMove �R���|�[�l���g���擾
    }

    void Update()
    {
        if (target != null)
        {
            // ObjectMove�X�N���v�g�̃^�[�Q�b�g���X�V
            target = move.Target;
                    
        }
        else
        {
            Debug.Log("�^�[�Q�b�g��null�ł��B");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // �q�b�g�G���A�ɐG�ꂽ�v���C���[���^�[�Q�b�g�ɐݒ�
        if (other.CompareTag(Tags.AI))
        {
            target = other.gameObject;  // �v���C���[���^�[�Q�b�g�Ƃ��Đݒ�
            Debug.Log("�^�[�Q�b�g�ݒ�: " + target.name);  // �^�[�Q�b�g�ݒ莞�Ɋm�F
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.AI))
        {
            target = null;  // �v���C���[���G���A����o����^�[�Q�b�g��null�ɂ���
        }

    }
}
