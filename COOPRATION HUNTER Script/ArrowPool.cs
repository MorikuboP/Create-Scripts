using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPool : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab; // 矢のプレハブ
    [SerializeField] private int poolSize = 10; // プールのサイズ
    private List<GameObject> arrowPool; // 矢のプール

    void Start()
    {
        // プールの初期化
        arrowPool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab);
            arrow.SetActive(false); // 初期状態で非アクティブにする
            arrowPool.Add(arrow);
        }
    }

    public GameObject GetArrow()
    {
        // プールから矢を取得
        foreach (GameObject arrow in arrowPool)
        {
            if (!arrow.activeInHierarchy)
            {
                //arrow.SetActive(true);
                return arrow;
            }
        }

        // プールが空の場合は null を返す
        return null;
    }

    public void ReturnArrow(GameObject arrow)
    {
        // 矢をプールに戻す
        arrow.SetActive(false);
    }
}
