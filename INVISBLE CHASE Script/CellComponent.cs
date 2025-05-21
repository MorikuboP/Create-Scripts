using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellComponent : MonoBehaviour
{
    private bool isWall; // �ʍs�\���ǂ���
    private int gridX;
    private int gridZ;
    private string cellType = "Default"; // �Z���̎��


    public void InitializeCell(int x, int z, bool isWalkable, string type)
    {
        gridX = x;
        gridZ = z;
        isWall = !isWalkable; // �ǂ̏ꍇ��true�ɐݒ�
        cellType = type;

        UpdateColor();
    }

    private void UpdateColor()
    {
        Renderer renderer = GetComponent<Renderer>();

        // isWall�ɉ������F��ݒ�
        renderer.material.color = isWall ? Color.black : Color.gray;
    }
}

