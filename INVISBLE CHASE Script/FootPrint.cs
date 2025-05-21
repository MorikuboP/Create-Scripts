using UnityEngine;

public class FootPrint : MonoBehaviour
{�@
    [SerializeField]private ObjectPool _objectPool; �@�@�@�@�@�@// �I�u�W�F�N�g�v�[��
    private float _footprintInterval = 0.5f; �@�@�@�@�@�@// ���Ղ̐����Ԋu
    private float _footprintDuration = 5f; �@�@�@�@�@�@ // ���Ղ𐶐����鎞��
    private float _elapsedTime = 0f;         �@�@�@�@�@�@// ���Ղ𐶐����Ă��鎞��
    private float _footprintTimer = 0f;      �@�@�@�@�@�@// ���Ղ̐����^�C�}�[
    private bool _isGeneratingFootprints = false;�@�@�@  // ���Ղ𐶐������ǂ���
    private bool _infinityFootFlag = false;   �@�@�@     // ���Ղ𐶐���������t���O

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

            // �C���N�𓥂񂾂Ƃ���10�b�ԑ�����
            if (!_infinityFootFlag && _elapsedTime >= _footprintDuration)
            {
                _isGeneratingFootprints = false;
            }
        }

        if (_infinityFootFlag)
        {
            // �����t���O�������Ă���ꍇ�A�������p��
            _isGeneratingFootprints = true;
        }
    }

    public bool GetSetFootBool
    {
        get { return _infinityFootFlag; }
        set { _infinityFootFlag = value; }
    }

    // ���Ղ𐶐����鏈��
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

    // ���Ր������J�n����
    public void StartGeneratingFootprints()
    {
        if (!_isGeneratingFootprints)
        {
          
            _isGeneratingFootprints = true;
            // �������Ԃ����Z�b�g
            _elapsedTime = 0f;
            // �^�C�}�[�����Z�b�g
            _footprintTimer = 0f; 
        }
    }

    // ���Ր������~����
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
            // ���Ԃ����Z�b�g
            _elapsedTime = 0f;  
        }
    }
}
