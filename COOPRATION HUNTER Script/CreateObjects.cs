using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateObjects : MonoBehaviour
{
    [SerializeField] List<AnimalController> animals;  // ������AnimalController�������X�g
    [SerializeField] int _maxCount;                   // �ő吶����
    [SerializeField] List<Transform> createPoints;    // �����̐����|�C���g
    Queue<AnimalController> animalQueue;              // �������Ǘ�����L���[

    private void Awake()
    {
        animalQueue = new Queue<AnimalController>();

        for (int i = 0; i < _maxCount; i++)
        {
            // �����_���ɐ����|�C���g��I��
            Transform point = createPoints[Random.Range(0, createPoints.Count)];

            // �����_����AnimalController��I��
            AnimalController selectedAnimal = animals[Random.Range(0, animals.Count)];

            // AnimalController�𐶐�
            AnimalController createAnimal = Instantiate(selectedAnimal, point.position, Quaternion.identity, transform);

            // ���������������L���[�ɒǉ�
            createAnimal.SetSpawnPoint(point.position); // �����|�C���g��ݒ�
            animalQueue.Enqueue(createAnimal);
        }
    }

    public AnimalController Launch(Vector3 _pos)
    {

        // �L���[���瓮�����擾
        AnimalController createAnimal = animalQueue.Dequeue();

        // �������A�N�e�B�u�ɂ��A�w��ʒu�Ɉړ�
        createAnimal.gameObject.SetActive(true);
        createAnimal.move(_pos);


        return createAnimal;
    }

    public void Collect(AnimalController _animal)
    {
        // �����_���Ȑ����|�C���g�ɖ߂�
        Transform point = createPoints[Random.Range(0, createPoints.Count)];
        _animal.gameObject.transform.position = point.position;

        _animal.gameObject.SetActive(false); // �������A�N�e�B�u�ɂ���
        animalQueue.Enqueue(_animal); // �L���[�ɖ߂�

    }
}
