using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// ����ҁ@�S���C�q�f�L

public class GameManager : MonoBehaviour
{
    public DetectArea[] detectAreas;    // ���ׂĂ̔���G���A��o�^
    private int totalConnected = 0;       // �ڑ����ꂽ����G���A�̐�

    public PressurePlates01[] pressurePlates; // ���ׂĂ̊�����o�^
    private int totalPressed = 0; // ������Ă��銴���̐�

    public string resultSceneName = "Result"; // �J�ڐ�̃V�[������Inspector�Őݒ�

    private bool gameClearFg = false;         // �Q�[���N���A�������ǂ���
    private bool gameOverFg = false;          // �Q�[���I�[�o�[�������ǂ���

    private float changeSceneTime = 1.0f;     // ���b��Ƀ��U���g�V�[���ɑJ�ڂ��邩

    // Start is called before the first frame update
    void Start()
    {
        foreach (PressurePlates01 plate in pressurePlates)
        {
            plate.OnPressurePlateChanged += OnPlateStateChanged; // �C�x���g�o�^
        }

        foreach (DetectArea detectArea in detectAreas)
        {
            detectArea.OnDetectAreaChanged += OnDetectionStateChanged; // �C�x���g�o�^
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if(gameOverFg)
        {
            Debug.Log("���΂��������܂����I�Q�[���I�[�o�[�IResult��ʂɈڂ�܂�");
            // �����ɃQ�[���I�[�o�[����������

            // changeSceneTime�b��Ƀ��U���g�V�[���ɑJ��
            Invoke("MoveResultScene", changeSceneTime);    
        }
    }

    void OnPlateStateChanged(bool isPressed)
    {
        totalPressed += isPressed ? 1 : -1; // ������Ă��鐔�𑝌�

        if (totalPressed == pressurePlates.Length) // ���ׂĂ������ꂽ�ꍇ
        {
            gameClearFg = true; // �Q�[���N���A
            Debug.Log("�S�Ă̊�����������Ă��܂��I�Q�[���N���A�IResult��ʂɈڂ�܂�");
            // �����ɃQ�[���N���A����������
            SceneManager.LoadScene(resultSceneName);
        }
    }

    void OnDetectionStateChanged(bool isConnected)
    {
        totalConnected += isConnected ? 1 : -1; // �ڑ�����Ă��鐔�𑝌�
        Debug.Log(totalConnected);

        if (totalConnected == detectAreas.Length)
        {
            gameClearFg = true; // �Q�[���N���A
            Debug.Log("�S�Ẳ�H���ڑ�����Ă��܂��I�Q�[���N���A�IResult��ʂɈڂ�܂�");
            // �����ɃQ�[���N���A����������

            // changeSceneTime�b��Ƀ��U���g�V�[���ɑJ��
            Invoke("MoveResultScene", changeSceneTime);
        }
    }

    // ���U���g�V�[���ɑJ��
    private void MoveResultScene()
    {
        SceneManager.LoadScene(resultSceneName);
    }

    // �Q�[���N���A�t���O���擾
    public bool GetGameClearFg()
    {
        return gameClearFg;
    }

    // �Q�[���I�[�o�[�t���O���擾
    public bool GetGameOverFg()
    {
        return gameOverFg;
    }

    // �Q�[���N���A�t���O���Z�b�g
    public void SetGameClearFg(bool _gameClearFg)
    {
        gameClearFg = _gameClearFg;
    }

    // �Q�[���I�[�o�[�t���O���Z�b�g
    public void SetGameOverFg(bool _gameOverFg)
    {
        gameOverFg = _gameOverFg;
    }
}
