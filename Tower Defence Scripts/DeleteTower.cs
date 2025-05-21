using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine;

public class DeleteTower : MonoBehaviour
{
    [SerializeField] Tilemap _targetTileMap;    //�I���ł���^�C���̎��
    private HashSet<Vector3Int> _placedTiles;   //�^���[���ݒu����Ă���}�X
    private List<GameObject> _placedObjects;    //�ݒu����Ă���^���[
    private CreateTower _createTower;           //CreateTower�̎擾
    private const float _judgmentRange = 0.3f;  //����͈͂̒���

    public static class Tags
    {
        public const string PlacedTower = "PlacedTower";
    }

    void Start()
    {
        _createTower = GetComponent<CreateTower>();
        if(_createTower != null)
        {
            _placedTiles = _createTower.GetPlacedTiles();
            _placedObjects = _createTower.GetPlacedObject();
        }
    }

    void Update()
    {
        //�E�N���b�N����ƃ^���[�̍폜�̏������s��
        if(Input.GetMouseButtonDown(1))
        {
            Vector3Int cellPosition = GetMouseTilePosition();
            if(_placedTiles != null && _placedTiles.Contains(cellPosition))
            {
                RemoveObject(cellPosition);
            }
        }
    }
    /// <summary>
    /// �}�X�ɒu���Ă���^���[���폜����
    /// </summary>
    /// <param name="cellPosition">�^���[���ݒu����Ă���ʒu</param>
    private void RemoveObject(Vector3Int cellPosition)
    {
        Vector3 worldPosition = _targetTileMap.GetCellCenterWorld(cellPosition);

        //worldPosition�𒆐S��_judgementRange�̃T�C�Y�̉~�����A���̉~���̃R���C�_�[���擾
        Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPosition, _judgmentRange);
        Collider2D closestCollider = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D collider in colliders)
        {
            // �^�O����v����I�u�W�F�N�g�̂ݏ���
            if (collider.CompareTag(Tags.PlacedTower))
            {
                float distance = Vector2.Distance(worldPosition, collider.bounds.center);
                // �ł��߂��I�u�W�F�N�g��T��
                if (distance < closestDistance)  
                {
                    closestDistance = distance;
                    closestCollider = collider;
                }
            }
        }

        // �폜�Ώۂ����������ꍇ�̂ݍ폜
        if (closestCollider != null) 
        {
            _placedObjects.Remove(closestCollider.gameObject);
            Destroy(closestCollider.gameObject);
            _placedTiles.Remove(cellPosition);
            _createTower.Reinstallable(cellPosition);
        }
    }
    /// <summary>
    /// �}�E�X�J�[�\���̈ʒu�����
    /// </summary>
    /// <returns></returns>
    Vector3Int GetMouseTilePosition()
    {
        Vector3 mouseworldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseworldPos.z = 0;
        return _targetTileMap.WorldToCell(mouseworldPos);
    }
}
