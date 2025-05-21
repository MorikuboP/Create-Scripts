using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CountDown : MonoBehaviour
{
    #region Fields
    [SerializeField] private float _initialCountDownTime = 3f;   // �ŏ��̃J�E���g�_�E������
    [SerializeField] private float _mainCountDownTime = 30f;     // �Q�[���̐�������
    [SerializeField] private Text _initialCountText;             // �ŏ��̃J�E���g�_�E���\���pUI
    [SerializeField] private Text _mainCountText;                // ���C���J�E���g�_�E���\���pUI
    [SerializeField] private EnemyMoveUpdater _updater;
    [SerializeField] private EnemyNormalMove _normalMove;
    [SerializeField] private FootPrint _foot;                    // ���Ղ̃V�X�e���i�H�j�𐧌䂷��X�N���v�g
    [SerializeField] private PlayerMove _player;                 // �v���C���[�̈ړ��𐧌䂷��X�N���v�g
    private float LimitTime = 10f;                               // �G����������J�n���鎞�ԁi�������Ԃ�10�b�ȉ��ɂȂ�����j
    private bool isInitialCountDownFinished = false;             // �ŏ��̃J�E���g�_�E�����I���������̃t���O
    private float _previousInitialTime = -1f;                    // �ŏ��̃J�E���g�_�E���̑O��̕\�����ԁi�œK���p�j
    private float _previousMainTime = -1f;                       // ���C���J�E���g�_�E���̑O��̕\�����ԁi�œK���p�j
    #endregion

    private void Start()
    {
        // �����J�E���g�_�E���ƃ��C���J�E���g�_�E����UI��ݒ�
        UpdateInitialCountText(_initialCountDownTime);
        UpdateMainCountText(_mainCountDownTime);
    }

    private void Update()
    {
        if (!isInitialCountDownFinished)
        {
            // �ŏ��̃J�E���g�_�E�������s
            InitialCountDown();
        }
        else
        {
            // ���C���̃J�E���g�_�E�������s
            MainCountDown();
        }
    }
    /// <summary>
    /// �ŏ��̂R�J�E���g
    /// �R�J�E���g��A�G�[�W�F���g�A�v���C���[���s���ł���悤�ɂ���
    /// </summary>
    private void InitialCountDown()
    {
        _initialCountDownTime -= Time.deltaTime; // ���Ԃ����炷

        if (_initialCountDownTime <= 0)
        {
            // �J�E���g�_�E���I�����̏���
            isInitialCountDownFinished = true;
            _initialCountText.text = ""; // UI���\���ɂ���

            // �v���C���[�ƓG�̈ړ���L���ɂ���
            _normalMove.IsMove = true;
            _player.IsMove = true;
            return;
        }

        // �J�E���g�_�E��UI���X�V
        UpdateInitialCountText(_initialCountDownTime);
    }
    /// <summary>
    /// �������ԃJ�E���g
    /// LimitTime�ȉ��ɂȂ������������[�h�𔭓�����
    /// </summary>
    private void MainCountDown()
    {
        // ���Ԃ����炷
        _mainCountDownTime -= Time.deltaTime;

        // �������Ԃ�LimitTime�ȉ��ɂȂ�����G�̍s����ύX
        if (_mainCountDownTime <= LimitTime)
        {
            // �G����������[�h�ɂ���
            _updater.LaxityMode();
            // ���Ղ̐ݒ��ύX
            _foot.GetSetFootBool = true;
        }

        // �������Ԃ�0�ɂȂ�����Q�[���I�[�o�[�V�[���֑J��
        if (_mainCountDownTime <= 0)
        {
            SceneManager.LoadScene("GameOverScene");
            return;
        }

        // �J�E���g�_�E��UI���X�V
        UpdateMainCountText(_mainCountDownTime);
    }

    /// <summary>
    /// �ŏ��̃J�E���g�_�E��UI�̍X�V����
    /// </summary>
    /// <param name="time"></param>
    private void UpdateInitialCountText(float time)
    {
        // �����_�؂�グ
        int newTime = Mathf.CeilToInt(time);
        // �O��ƒl���ς�����Ƃ������X�V�i���ʂȏ��������炷�j
        if (newTime != _previousInitialTime) 
        {
            _initialCountText.text = newTime.ToString();
            _previousInitialTime = newTime;
        }
    }

    /// <summary>
    /// ���C���J�E���g�_�E��UI�̍X�V����
    /// </summary>
    /// <param name="time"></param>
    private void UpdateMainCountText(float time)
    {
        // �����_�؂�グ
        int newTime = Mathf.CeilToInt(time);
        // �O��ƒl���ς�����Ƃ������X�V�i���ʂȏ��������炷�j
        if (newTime != _previousMainTime) 
        {
            _mainCountText.text = newTime.ToString();
            _previousMainTime = newTime;
        }
    }
}
