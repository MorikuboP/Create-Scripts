using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPool : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab; // ��̃v���n�u
    [SerializeField] private int poolSize = 10; // �v�[���̃T�C�Y
    private List<GameObject> arrowPool; // ��̃v�[��

    void Start()
    {
        // �v�[���̏�����
        arrowPool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab);
            arrow.SetActive(false); // ������ԂŔ�A�N�e�B�u�ɂ���
            arrowPool.Add(arrow);
        }
    }

    public GameObject GetArrow()
    {
        // �v�[���������擾
        foreach (GameObject arrow in arrowPool)
        {
            if (!arrow.activeInHierarchy)
            {
                //arrow.SetActive(true);
                return arrow;
            }
        }

        // �v�[������̏ꍇ�� null ��Ԃ�
        return null;
    }

    public void ReturnArrow(GameObject arrow)
    {
        // ����v�[���ɖ߂�
        arrow.SetActive(false);
    }
}
