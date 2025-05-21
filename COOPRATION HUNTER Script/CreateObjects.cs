using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateObjects : MonoBehaviour
{
    [SerializeField] List<AnimalController> animals;  // 複数のAnimalControllerを持つリスト
    [SerializeField] int _maxCount;                   // 最大生成数
    [SerializeField] List<Transform> createPoints;    // 複数の生成ポイント
    Queue<AnimalController> animalQueue;              // 動物を管理するキュー

    private void Awake()
    {
        animalQueue = new Queue<AnimalController>();

        for (int i = 0; i < _maxCount; i++)
        {
            // ランダムに生成ポイントを選択
            Transform point = createPoints[Random.Range(0, createPoints.Count)];

            // ランダムにAnimalControllerを選択
            AnimalController selectedAnimal = animals[Random.Range(0, animals.Count)];

            // AnimalControllerを生成
            AnimalController createAnimal = Instantiate(selectedAnimal, point.position, Quaternion.identity, transform);

            // 生成した動物をキューに追加
            createAnimal.SetSpawnPoint(point.position); // 生成ポイントを設定
            animalQueue.Enqueue(createAnimal);
        }
    }

    public AnimalController Launch(Vector3 _pos)
    {

        // キューから動物を取得
        AnimalController createAnimal = animalQueue.Dequeue();

        // 動物をアクティブにし、指定位置に移動
        createAnimal.gameObject.SetActive(true);
        createAnimal.move(_pos);


        return createAnimal;
    }

    public void Collect(AnimalController _animal)
    {
        // ランダムな生成ポイントに戻す
        Transform point = createPoints[Random.Range(0, createPoints.Count)];
        _animal.gameObject.transform.position = point.position;

        _animal.gameObject.SetActive(false); // 動物を非アクティブにする
        animalQueue.Enqueue(_animal); // キューに戻す

    }
}
