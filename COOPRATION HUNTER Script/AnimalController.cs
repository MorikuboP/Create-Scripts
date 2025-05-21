using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : MonoBehaviour
{
    private CreateObjects objectPool; // CreateObjects�̃C���X�^���X��ێ�
    public float _speed;               // �����̈ړ����x
    private Vector3 spawnPoint;        // �����|�C���g
    public bool _shotdirection = true; // ���˕���
    private GameObject createposition; // �����ʒu

    void Start()
    {
        objectPool = transform.parent.GetComponent<CreateObjects>();
        gameObject.SetActive(false); // ������ԂŔ�A�N�e�B�u�ɂ���
        _shotdirection = false; // ���˕�����������
    }

    public void move(Vector3 _pos)
    {
        createposition.transform.position = _pos; // �����ʒu���X�V
        gameObject.SetActive(true); // �������A�N�e�B�u�ɂ���

        // �ړ�����
        if (_shotdirection == false)
        {
            transform.position += transform.right * _speed * Time.deltaTime; // �E�����Ɉړ�
        }
        else if (_shotdirection == true)
        {
            transform.position += -transform.right * _speed * Time.deltaTime; // �������Ɉړ�
        }
    }

    // �����|�C���g��ݒ肷�郁�\�b�h
    public void SetSpawnPoint(Vector3 point)
    {
        spawnPoint = point;
    }

    // �����|�C���g���擾���郁�\�b�h
    public Vector3 GetSpawnPoint()
    {
        return spawnPoint;
    }

    // createposition��ݒ肷�郁�\�b�h
    public void SetCreatePosition(GameObject position)
    {
        createposition = position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Delete"))
        {
            objectPool.Collect(this); // ��ʊO�����̃g���K�[�ɂԂ�������Collect
        }
    }
}
