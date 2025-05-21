using UnityEngine;

public class FootPrint : MonoBehaviour
{　
    [SerializeField]private ObjectPool _objectPool; 　　　　　　// オブジェクトプール
    private float _footprintInterval = 0.5f; 　　　　　　// 足跡の生成間隔
    private float _footprintDuration = 5f; 　　　　　　 // 足跡を生成する時間
    private float _elapsedTime = 0f;         　　　　　　// 足跡を生成している時間
    private float _footprintTimer = 0f;      　　　　　　// 足跡の生成タイマー
    private bool _isGeneratingFootprints = false;　　　  // 足跡を生成中かどうか
    private bool _infinityFootFlag = false;   　　　     // 足跡を生成し続けるフラグ

    public static class Tags
    {
        public const string Ink = "ink";
    }

    private void Update()
    {
        if (_isGeneratingFootprints)
        {
            _elapsedTime += Time.deltaTime;
            _footprintTimer += Time.deltaTime;

            if (_footprintTimer >= _footprintInterval)
            {
                _footprintTimer = 0f;
                GenerateFootprint();
            }

            // インクを踏んだときは10秒間続ける
            if (!_infinityFootFlag && _elapsedTime >= _footprintDuration)
            {
                _isGeneratingFootprints = false;
            }
        }

        if (_infinityFootFlag)
        {
            // 無限フラグが立っている場合、生成を継続
            _isGeneratingFootprints = true;
        }
    }

    public bool GetSetFootBool
    {
        get { return _infinityFootFlag; }
        set { _infinityFootFlag = value; }
    }

    // 足跡を生成する処理
    private void GenerateFootprint()
    {
        GameObject footPrint = _objectPool.GetObject(transform.position, transform.rotation);
        if (footPrint != null) if (footPrint != null)
        {
            footPrint.transform.position = transform.position;
            footPrint.transform.rotation = transform.rotation;
            footPrint.SetActive(true);
        }
       
    }

    // 足跡生成を開始する
    public void StartGeneratingFootprints()
    {
        if (!_isGeneratingFootprints)
        {
          
            _isGeneratingFootprints = true;
            // 生成時間をリセット
            _elapsedTime = 0f;
            // タイマーもリセット
            _footprintTimer = 0f; 
        }
    }

    // 足跡生成を停止する
    public void StopGeneratingFootprints()
    {
        _isGeneratingFootprints = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Ink))
        {
            StartGeneratingFootprints();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.Ink))
        {
            _isGeneratingFootprints = true;
            // 時間をリセット
            _elapsedTime = 0f;  
        }
    }
}
