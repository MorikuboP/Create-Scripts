using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // 移動スピード倍率
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float minLimitX, maxLimitX, minLimitZ, maxLimitZ;

    private Animator animator; // Animator コンポーネント
    private Vector3 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>(); // Animator の取得
    }

    private void Update()
    {
        HandleInput(); // 入力処理
        UpdateRotation(); // カメラに向き続ける
        UpdateAnimation(); // アニメーション制御
    }

    private void FixedUpdate()
    {
        Move(); // Rigidbody による移動
    }

    private void HandleInput()
    {
        // カメラの前方向と右方向を取得
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 cameraRight = Vector3.Scale(Camera.main.transform.right, new Vector3(1, 0, 1)).normalized;

        // 入力に基づいて移動方向を計算
        moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) moveDirection += cameraForward;
        if (Input.GetKey(KeyCode.A)) moveDirection -= cameraRight;
        if (Input.GetKey(KeyCode.S)) moveDirection -= cameraForward;
        if (Input.GetKey(KeyCode.D)) moveDirection += cameraRight;

        moveDirection = moveDirection.normalized * moveSpeed; // スピード倍率を適用
    }

    private void UpdateRotation()
    {
        // プレイヤーの向きをカメラの向きに合わせる
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        if (cameraForward.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
            transform.rotation = targetRotation; // カメラ方向を向き続ける
        }
    }

    private void Move()
    {
        if (moveDirection.magnitude > 0)
        {
            rb.velocity = moveDirection; // 移動処理
        }
        else
        {
            rb.velocity = Vector3.zero; // 停止時の速度をゼロに設定
        }

        // 移動範囲を制限
        rb.position = new Vector3(
            Mathf.Clamp(rb.position.x, minLimitX, maxLimitX),
            rb.position.y,
            Mathf.Clamp(rb.position.z, minLimitZ, maxLimitZ)
        );
    }

    private void UpdateAnimation()
    {
        // 移動中かどうかを判定してアニメーションを制御
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

