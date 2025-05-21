using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControll : MonoBehaviour
{
    [SerializeField] private GameObject player; // �v���C���[�I�u�W�F�N�g
    [SerializeField] private float rotationSpeed = 5.0f; // �J�����̉�]�X�s�[�h�𒲐�����{��

    private Vector3 currentPos;
    private Vector3 pastPos;
    private Vector3 diff;

    public static class Tags
    {
        public const string MouseInput = "Mouse X";
    }

    // Start is called before the first frame update
    void Start()
    {
        pastPos = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // �v���C���[�̈ړ��ɒǏ]
        currentPos = player.transform.position;
        diff = currentPos - pastPos;
        transform.position = Vector3.Lerp(transform.position, transform.position + diff, 3.0f);
        pastPos = currentPos;

        // �}�E�X���͂̎擾
        float mouseX = Input.GetAxis(Tags.MouseInput) * rotationSpeed; // ��]�X�s�[�h�{����K�p
    

        // ���������̉�] (Y�����)
        if (Mathf.Abs(mouseX) > 0.01f)
        {
            transform.RotateAround(player.transform.position, Vector3.up, mouseX);
        }

       
    }
}

