using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{

    [SerializeField] private float interval;
    [SerializeField] private float playerArrowSpeed = default; // ��̑��x
   [SerializeField]  private ArrowPool arrowPool; // ��̃v�[���̎Q��
    [SerializeField] Transform shootPoint; // ��𔭎˂���ʒu
    private bool _shotflag = default;
    private float timer;
    private BowSpawn spawn;

    public static class Tags
    {
        public const string ArrowManager = "ArrowPool";
    }

    void Start()
    {
        spawn = GetComponent<BowSpawn>();
        // ArrowManager �Ƃ������O�̃I�u�W�F�N�g��T���Ă��̒��� ArrowPool �R���|�[�l���g���擾
        GameObject arrowManager = GameObject.Find(Tags.ArrowManager);
        if (arrowManager != null)
        {
            arrowPool = arrowManager.GetComponent<ArrowPool>(); // ArrowPool �R���|�[�l���g���擾
        }
        else
        {
            Debug.LogError("ArrowManager �I�u�W�F�N�g��������܂���I");
        }

        if (arrowPool == null)
        {
            Debug.LogError("ArrowPool is not assigned!");
        }
        _shotflag = true;
        timer = 0f;
      
    }

    // Update is called once per frame
    void Update()
    {
        // ����ˉ\�ŁA�E�N���b�N�������ꂽ�Ƃ�
        if (_shotflag && Input.GetMouseButton(0))
        {
            GameObject arrow = arrowPool.GetArrow(); // ����v�[������擾

            if (arrow != null)
            {
                // ��̈ʒu�Ɖ�]��ݒ�
                arrow.transform.position = shootPoint.position;

                // Z��������90�x��]
                Quaternion rotation = shootPoint.rotation * Quaternion.Euler(0, 0, 90);
                arrow.transform.rotation = rotation;

                // ��̓������J�n
                arrow.GetComponent<ArrowManager>().Launch(playerArrowSpeed);

                // ��𔭎˂�����t���O��false��
                _shotflag = false;

                // ���ˌ�̃^�C�}�[�X�V
                timer = 0f;
            }
        }


        if (!_shotflag)
        {
            timer += Time.deltaTime; // ���Ԃ����Z

            if (timer >= interval) // �C���^�[�o���𒴂����甭�ˉ\��
            {
                _shotflag = true; // ���ˏ���OK
            }
        }
    }
}
