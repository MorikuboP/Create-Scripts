using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellComponent : MonoBehaviour
{
    private bool isWall; // 通行可能かどうか
    private int gridX;
    private int gridZ;
    private string cellType = "Default"; // セルの種類


    public void InitializeCell(int x, int z, bool isWalkable, string type)
    {
        gridX = x;
        gridZ = z;
        isWall = !isWalkable; // 壁の場合はtrueに設定
        cellType = type;

        UpdateColor();
    }

    private void UpdateColor()
    {
        Renderer renderer = GetComponent<Renderer>();

        // isWallに応じた色を設定
        renderer.material.color = isWall ? Color.black : Color.gray;
    }
}

