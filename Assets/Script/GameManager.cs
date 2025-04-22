using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// ����ҁ@�S���C�q�f�L   �{�c���s

// �O������ݒ�\�ȃX�e�[�W�̃f�[�^
[System.Serializable]
public class StageData
{
    public List<GameObject> magObjects;     // ���̓I�u�W�F�N�g
    public List<DetectArea> detectAreas;    // �N���A����I�u�W�F�N�g
    public bool clearFg = false;            // �N���A�t���O
}

public class GameManager : MonoBehaviour
{
    [Header("�e�X�e�[�W�̎��̓I�u�W�F�N�g�i�ŏ��ɃX�e�[�W����ݒ�j")]
    public List<StageData> stageDatas;

    [Header("�v���C���[�̎���")]
    public GameObject magnet1;
    public GameObject magnet2;

    public DetectArea[] detectAreas;    // ���ׂĂ̔���G���A��o�^
    private int totalConnected = 0;     // �ڑ����ꂽ����G���A�̐�

    public string resultSceneName = "Result";       // �J�ڐ�̃V�[������Inspector�Őݒ�
    public string gameOverSceneName = "GameOver";   // �J�ڐ�̃V�[������Inspector�Őݒ�

    private bool gameClearFg = false;         // �Q�[���N���A�������ǂ���
    private bool gameOverFg = false;          // �Q�[���I�[�o�[�������ǂ���

    private float changeSceneTime = 1.0f;     // ���b��Ƀ��U���g�V�[���ɑJ�ڂ��邩

    private Magnetism magnetism1 = null;      // �v���C���[L�̃}�O�l�e�B�Y��
    private Magnetism magnetism2 = null;      // �v���C���[R�̃}�O�l�e�B�Y��

    private int curStage = 0;                 // ���݂̃X�e�[�W��

    public PressurePlates01[] pressurePlates; // ���ׂĂ̊�����o�^
    private int totalPressed = 0; // ������Ă��銴���̐�


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

        // ���̓X�N���v�g���擾
        magnetism1 = magnet1.GetComponent<Magnetism>();
        magnetism2 = magnet2.GetComponent<Magnetism>();

        // �X�e�[�W1�̎��̓I�u�W�F�N�g��L����
        ActiveMagObjects(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        int connectCount = 0;  // �q�����Ă��锻��G���A

        // ���݂̃X�e�[�W�̔���G���A���q�����Ă��邩�𒲂ׂ�
        for (int i = 0; i < stageDatas[curStage].detectAreas.Count; i++)
        {
            // �q�����Ă��鎞�A���̐����J�E���g
            if (stageDatas[curStage].detectAreas[i].GetIsConnectFg())
            {
                connectCount++;

                // ���ׂĂ̔���G���A���q�����Ă��鎞�A�N���A�t���O�𗧂Ă�
                if (connectCount == stageDatas[curStage].detectAreas.Count)
                {
                    stageDatas[curStage].clearFg = true;
                }
            }
        }

        // ���݂̃X�e�[�W���N���A�������A���̃X�e�[�W�ɐi��
        if (stageDatas[curStage].clearFg )
        {
            curStage++;
            ActiveMagObjects(curStage); // ���̃X�e�[�W�̎��̓I�u�W�F�N�g��L����
            stageDatas[curStage - 1].clearFg = false;   // ������X�V���Ȃ��悤�Ƀt���O��܂�
        }

        // �v���C���[�̎��΂ɉ����������������܂��́A���͔͈͊O�ɏo����
        if ((magnetism1.isSnapping || magnetism2.isSnapping) || 
            (!magnetism1.inMagnetismArea || !magnetism2.inMagnetismArea))
        {
            Debug.Log("���΂��������܂����I�Q�[���I�[�o�[�IResult��ʂɈڂ�܂�");
            // �����ɃQ�[���I�[�o�[����������

            // changeSceneTime�b��ɃQ�[���I�[�o�[�V�[���ɑJ��
            Invoke("MoveGameOverScene", changeSceneTime);    
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

            // changeSceneTime�b��Ƀ��U���g�V�[���ɑJ��
            Invoke("MoveResultScene", changeSceneTime);
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

    // �w�肵���X�e�[�W�̎��̓I�u�W�F�N�g��L����
    private void ActiveMagObjects(int _stageIndex)
    {
        // �S�X�e�[�W�̎��̓I�u�W�F�N�g�𖳌���
        for (int i = 0; i < stageDatas.Count; i++)
        {
            foreach (GameObject obj in stageDatas[i].magObjects)
            {
                if (obj != null) { obj.SetActive(false); }
            }
        }

        // �w�肳�ꂽ�X�e�[�W�̃I�u�W�F�N�g��L����
        if(_stageIndex >= 0 &&  _stageIndex < stageDatas.Count)
        {
            foreach (GameObject obj in stageDatas[_stageIndex].magObjects)
            {
                if (obj != null) { obj.SetActive(true); }
            }
        }
    }


    // ���U���g�V�[���ɑJ��
    private void MoveResultScene()
    {
        SceneManager.LoadScene(resultSceneName);
    }

    // �Q�[���I�[�o�[�V�[���ɑJ��
    private void MoveGameOverScene()
    {
        SceneManager.LoadScene(gameOverSceneName);
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
