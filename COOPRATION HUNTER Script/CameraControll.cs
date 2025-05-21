using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControll : MonoBehaviour
{
    [SerializeField] private GameObject player; // プレイヤーオブジェクト
    [SerializeField] private float rotationSpeed = 5.0f; // カメラの回転スピードを調整する倍率

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
        // プレイヤーの移動に追従
        currentPos = player.transform.position;
        diff = currentPos - pastPos;
        transform.position = Vector3.Lerp(transform.position, transform.position + diff, 3.0f);
        pastPos = currentPos;

        // マウス入力の取得
        float mouseX = Input.GetAxis(Tags.MouseInput) * rotationSpeed; // 回転スピード倍率を適用
    

        // 水平方向の回転 (Y軸回り)
        if (Mathf.Abs(mouseX) > 0.01f)
        {
            transform.RotateAround(player.transform.position, Vector3.up, mouseX);
        }

       
    }
}

