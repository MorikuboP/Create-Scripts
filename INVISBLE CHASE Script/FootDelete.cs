using System.Collections;
using UnityEngine;

public class FootDelete : MonoBehaviour
{
    private float fadeDuration = 5.0f; // �t�F�[�h�ɂ����鎞�ԁi�b�j
    private int step = 10;            // �t�F�[�h�̃X�e�b�v��
    private ObjectPool _objectPool;   // �v�[���̎Q�Ƃ�ێ�
    private MeshRenderer _meshRenderer; // �L���b�V���p
    /// <summary>
    /// �v�[�����Q��
    /// </summary>
    /// <param name="pool"></param>
    public void Initialize(ObjectPool pool)
    {
        // �v�[���Q�Ƃ�ݒ�
        _objectPool = pool; 
    }
    /// <summary>
    /// ���b�V�������_���[���擾����
    /// </summary>
    void OnEnable()
    {
        // MeshRenderer ���L���b�V��
        _meshRenderer = GetComponent<MeshRenderer>();

        if (_meshRenderer == null)
        {
            Debug.LogError("MeshRenderer��������܂���I");
            return;
        }

        // �t�F�[�h�A�E�g���J�n
        StartCoroutine(Disappearing());
    }
    /// <summary>
    /// ���Ԍo�߂ő��Ղ𓧖��ɂ��Ă���
    /// </summary>
    /// <returns></returns>
    IEnumerator Disappearing()
    {
        float stepDuration = fadeDuration / step; // �e�X�e�b�v�̑ҋ@����
        Material[] materials = _meshRenderer.materials; // �}�e���A�����擾

        for (int i = 0; i < step; i++)
        {
            float alpha = 1 - (1.0f * i / step);
            foreach (var mat in materials)
            {
                Color color = mat.color;
                color.a = alpha;
                mat.color = color; // �A���t�@�l���X�V
            }

            yield return new WaitForSeconds(stepDuration);
        }

        // �t�F�[�h������Ƀv�[���֖߂�
        if (_objectPool != null)
        {
            _objectPool.ReturnObject(gameObject);
        }
    }
}