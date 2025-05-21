using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // �ړ��X�s�[�h�{��
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float minLimitX, maxLimitX, minLimitZ, maxLimitZ;

    private Animator animator; // Animator �R���|�[�l���g
    private Vector3 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>(); // Animator �̎擾
    }

    private void Update()
    {
        HandleInput(); // ���͏���
        UpdateRotation(); // �J�����Ɍ���������
        UpdateAnimation(); // �A�j���[�V��������
    }

    private void FixedUpdate()
    {
        Move(); // Rigidbody �ɂ��ړ�
    }

    private void HandleInput()
    {
        // �J�����̑O�����ƉE�������擾
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 cameraRight = Vector3.Scale(Camera.main.transform.right, new Vector3(1, 0, 1)).normalized;

        // ���͂Ɋ�Â��Ĉړ��������v�Z
        moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) moveDirection += cameraForward;
        if (Input.GetKey(KeyCode.A)) moveDirection -= cameraRight;
        if (Input.GetKey(KeyCode.S)) moveDirection -= cameraForward;
        if (Input.GetKey(KeyCode.D)) moveDirection += cameraRight;

        moveDirection = moveDirection.normalized * moveSpeed; // �X�s�[�h�{����K�p
    }

    private void UpdateRotation()
    {
        // �v���C���[�̌������J�����̌����ɍ��킹��
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        if (cameraForward.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
            transform.rotation = targetRotation; // �J��������������������
        }
    }

    private void Move()
    {
        if (moveDirection.magnitude > 0)
        {
            rb.velocity = moveDirection; // �ړ�����
        }
        else
        {
            rb.velocity = Vector3.zero; // ��~���̑��x���[���ɐݒ�
        }

        // �ړ��͈͂𐧌�
        rb.position = new Vector3(
            Mathf.Clamp(rb.position.x, minLimitX, maxLimitX),
            rb.position.y,
            Mathf.Clamp(rb.position.z, minLimitZ, maxLimitZ)
        );
    }

    private void UpdateAnimation()
    {
        // �ړ������ǂ����𔻒肵�ăA�j���[�V�����𐧌�
        SetWalkAnimation(moveDirection.magnitude > 0.1f);
    }

    private void SetWalkAnimation(bool isWalking)
    {
        if (animator != null)
        {
            animator.SetBool("Walk", isWalking);
        }
    }
}

