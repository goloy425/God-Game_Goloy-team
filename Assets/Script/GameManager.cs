using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// ����ҁ@�S���C�q�f�L   �{�c���s


// �O������ݒ�\�ȃX�e�[�W�̃f�[�^�N���X
[System.Serializable]
public class StageData
{
    public List<GameObject> magObjSphere;       // ���̂̎��̓I�u�W�F�N�g
    public List<GameObject> magObjSplit1;       // �������̂̍����̎��̓I�u�W�F�N�g
    public List<GameObject> magObjSplit2;       // �������̂̉E���̎��̓I�u�W�F�N�g
    public List<GameObject> magObjConnecter;    // �������̂�ڑ����鎥�̓I�u�W�F�N�g
    public List<DetectArea> detectAreas;        // �N���A����I�u�W�F�N�g

    private List<SphereMagnetism> sphereMagCS;  // ���̂̎��̓X�N���v�g
    private List<HCubeMagnetism> hCubeMagCS;    // �������̂̎��̓X�N���v�g
    private List<CubeMagnetism> cubeMagCS;      // �R�l�N�^�[�̎��̓X�N���v�g
                                                
    private List<MoveSphere> moveSphereCS;      // ���̂̓���X�N���v�g
    private List<MoveHCubeL> moveHCubeLCS;      // �������̂̍����̓���X�N���v�g
    private List<MoveHCubeR> moveHCubeRCS;      // �������̂̉E���̓���X�N���v�g
                                                
    private bool clearFg = false;               // �N���A�t���O


    public bool GetClearFg() { return clearFg; }                    // �N���A�t���O�̃Q�b�^�[
    public void SetClearFg(bool _clearFg) { clearFg = _clearFg; }   // �N���A�t���O�̃Z�b�^�[
    public List<SphereMagnetism> GetSphereMagCS() { return sphereMagCS; }   // ���̂̎��̓X�N���v�g���X�g�̃Q�b�^�[
    public List<HCubeMagnetism> GetHCubeMagCS() { return hCubeMagCS; }      // ���̂̎��̓X�N���v�g���X�g�̃Q�b�^�[
    public List<CubeMagnetism> GetCubeMagCS() { return cubeMagCS; }         // ���̂̎��̓X�N���v�g���X�g�̃Q�b�^�[
    public List<MoveSphere> GetMoveSphereCS() { return moveSphereCS; }      // ���̂̎��̓X�N���v�g���X�g�̃Q�b�^�[
    public List<MoveHCubeL> GetMoveHCubeLCS() { return moveHCubeLCS; }      // ���̂̎��̓X�N���v�g���X�g�̃Q�b�^�[
    public List<MoveHCubeR> GetMoveHCubeRCS() { return moveHCubeRCS; }      // ���̂̎��̓X�N���v�g���X�g�̃Q�b�^�[
}


public class GameManager : MonoBehaviour
{
    [Header("�e�X�e�[�W�̎��̓I�u�W�F�N�g�i�ŏ��ɃX�e�[�W����ݒ�j")]
    public List<StageData> stageDatas;

    [Header("�v���C���[�̎���")]
    public GameObject magnet1;
    public GameObject magnet2;

    [Header("�e�X�e�[�W�̃N���A����I�u�W�F�N�g")]
    public DetectArea[] detectAreas;    // ���ׂĂ̔���G���A��o�^
    private int totalConnected = 0;     // �ڑ����ꂽ����G���A�̐�


    [Header("�J�ڐ�̃V�[����")]
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

        // �v���C���[�̎��΂̎��̓X�N���v�g���擾
        magnetism1 = magnet1.GetComponent<Magnetism>();
        magnetism2 = magnet2.GetComponent<Magnetism>();

        // �ݒ肳�ꂽ�S�Ă̎��̓I�u�W�F�N�g�ɃA�^�b�`���ꂽ�e�X�N���v�g�����X�g�ɒǉ�
        AddAllMagCSList();

        // �X�e�[�W1�̊e���̓X�N���v�g��L����
        ActiveMagObjects(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        // ----- �X�e�[�W�̃N���A���� ----- //
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
                    stageDatas[curStage].SetClearFg(true);
                }
            }
        }

        // ���݂̃X�e�[�W���N���A�������A���̃X�e�[�W������Ύ��̃X�e�[�W�ɐi��
        if (stageDatas[curStage].GetClearFg() && curStage + 1 < stageDatas.Count)
        {
            curStage++;
            ActiveMagObjects(curStage); // ���̃X�e�[�W�̎��̓I�u�W�F�N�g��L����
            stageDatas[curStage - 1].SetClearFg(false);   // ������X�V���Ȃ��悤�Ƀt���O��܂�
        }


        // ----- �Q�[���I�[�o�[�̔��菈�� ----- //
        // �v���C���[�̎��΂ɉ���������������
        if (magnetism1.isSnapping || magnetism2.isSnapping)
        {
            Debug.Log("���΂��������܂����I�Q�[���I�[�o�[�IResult��ʂɈڂ�܂�");
            // �����ɃQ�[���I�[�o�[����������

            // changeSceneTime�b��ɃQ�[���I�[�o�[�V�[���ɑJ��
            Invoke("MoveGameOverScene", changeSceneTime);    
        }

        //// �I�u�W�F�N�g�̎��͔͈͊O�ɏo����
        //if (!magnetism1.inObjMagArea || !magnetism2.inObjMagArea)
        //{
        //    Debug.Log("�I�u�W�F�N�g�̎��͔͈͊O�ɏo�܂����I�Q�[���I�[�o�[�IResult��ʂɈڂ�܂�");
        //    // �����ɃQ�[���I�[�o�[����������

        //    // changeSceneTime�b��ɃQ�[���I�[�o�[�V�[���ɑJ��
        //    Invoke("MoveGameOverScene", changeSceneTime);
        //}

        //// �v���C���[�̎��͔͈͊O�ɏo����
        //if (!magnetism1.inPlayerMagArea || !magnetism2.inPlayerMagArea)
        //{
        //    Debug.Log("�v���C���[�̎��͔͈͊O�ɏo�܂����I�Q�[���I�[�o�[�IResult��ʂɈڂ�܂�");
        //    // �����ɃQ�[���I�[�o�[����������

        //    // changeSceneTime�b��ɃQ�[���I�[�o�[�V�[���ɑJ��
        //    Invoke("MoveGameOverScene", changeSceneTime);
        //}

        // ���͔͈͊O�ɏo����
        if (!magnetism1.inMagnetismArea || !magnetism2.inMagnetismArea)
        {
            Debug.Log("���͔͈͊O�ɏo�܂����I�Q�[���I�[�o�[�IResult��ʂɈڂ�܂�");
            // �����ɃQ�[���I�[�o�[����������

            // changeSceneTime�b��ɃQ�[���I�[�o�[�V�[���ɑJ��
            Invoke("MoveGameOverScene", changeSceneTime);
        }
    }

    // GameManager���폜���ꂽ���Ƀ��X�g���̗v�f�����ׂč폜����
    private void OnDestroy()
    {
        //DeleteAllMagCSList();
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


    // �ݒ肳�ꂽ���̓I�u�W�F�N�g�ɃA�^�b�`����Ă���e�X�N���v�g�����X�g�Ɋi�[����
    private void AddAllMagCSList()
    {
        for (int i = 0; i < stageDatas.Count; i++)
        {
            // ���̂̎��̓I�u�W�F�N�g�̊e�X�N���v�g�����X�g�ɒǉ�
            foreach (GameObject magObj in stageDatas[i].magObjSphere)
            {
                if (magObj != null)
                {
                    stageDatas[i].GetSphereMagCS().Add(magObj.GetComponent<SphereMagnetism>());
                    stageDatas[i].GetMoveSphereCS().Add(magObj.GetComponent<MoveSphere>());
                }
            }

            // �������̂̍����̎��̓I�u�W�F�N�g�̊e�X�N���v�g�����X�g�ɒǉ�
            foreach (GameObject magObj in stageDatas[i].magObjSplit1)
            {
                if (magObj != null)
                {
                    stageDatas[i].GetHCubeMagCS().Add(magObj.GetComponent<HCubeMagnetism>());
                    stageDatas[i].GetMoveHCubeLCS().Add(magObj.GetComponent<MoveHCubeL>());
                }
            }

            // �������̂̉E���̎��̓I�u�W�F�N�g�̊e�X�N���v�g�����X�g�ɒǉ�
            foreach (GameObject magObj in stageDatas[i].magObjSplit2)
            {
                if (magObj != null)
                {
                    stageDatas[i].GetHCubeMagCS().Add(magObj.GetComponent<HCubeMagnetism>());
                    stageDatas[i].GetMoveHCubeRCS().Add(magObj.GetComponent<MoveHCubeR>());
                }
            }

            // �������̂�ڑ����鎥�̓I�u�W�F�N�g�̊e�X�N���v�g�����X�g�ɒǉ�
            foreach (GameObject magObj in stageDatas[i].magObjConnecter)
            {
                if (magObj != null)
                {
                    stageDatas[i].GetCubeMagCS().Add(magObj.GetComponent<CubeMagnetism>());
                }
            }
        }
    }

    // ���X�g�Ɋi�[����Ă���e�X�N���v�g��S�č폜����
    private void DeleteAllMagCSList()
    {
        for (int i = 0; i < stageDatas.Count; i++)
        {
            // ���̂̎��̓I�u�W�F�N�g�̊e�X�N���v�g�����X�g����폜
            if (stageDatas[i].magObjSphere != null)
            {
                stageDatas[i].GetSphereMagCS().Clear();
                stageDatas[i].GetMoveSphereCS().Clear();
            }

            // �������̂̍����̎��̓I�u�W�F�N�g�̊e�X�N���v�g�����X�g����폜
            if (stageDatas[i].magObjSplit1 != null)
            {
                stageDatas[i].GetHCubeMagCS().Clear();
                stageDatas[i].GetMoveHCubeLCS().Clear();
            }

            // �������̂̉E���̎��̓I�u�W�F�N�g�̊e�X�N���v�g�����X�g����폜
            if (stageDatas[i].magObjSplit2 != null)
            {
                stageDatas[i].GetHCubeMagCS().Clear();
                stageDatas[i].GetMoveHCubeRCS().Clear();
            }

            // �������̂�ڑ����鎥�̓I�u�W�F�N�g�̊e�X�N���v�g�����X�g����폜
            if (stageDatas[i].magObjConnecter != null)
            {
                stageDatas[i].GetCubeMagCS().Clear();
            }
        }
    }

    // �w�肵���X�e�[�W�̎��̓I�u�W�F�N�g��L����
    private void ActiveMagObjects(int _stageIndex)
    {
        // �S�X�e�[�W�̊e�X�N���v�g�𖳌���
        for (int i = 0; i < stageDatas.Count; i++)
        {
            //foreach (GameObject obj in stageDatas[i].magObjects)
            //{
            //    if (obj != null) { obj.SetActive(false); }
            //}

            // ���̂̎��̓I�u�W�F�N�g
            if (stageDatas[i].magObjSphere != null)
            {
                foreach (SphereMagnetism sphereMag in stageDatas[i].GetSphereMagCS()) { sphereMag.enabled = false; }
                foreach (MoveSphere moveSphere in stageDatas[i].GetMoveSphereCS()) { moveSphere.enabled = false; }
            }

            // �������̂̍����̎��̓I�u�W�F�N�g
            if (stageDatas[i].magObjSplit1 != null)
            {
                foreach (HCubeMagnetism hCubeMag in stageDatas[i].GetHCubeMagCS()) { hCubeMag.enabled = false; }
                foreach (MoveHCubeL moveHCubeL in stageDatas[i].GetMoveHCubeLCS()) { moveHCubeL.enabled = false; }
            }

            // �������̂̉E���̎��̓I�u�W�F�N�g
            if (stageDatas[i].magObjSplit2 != null)
            {
                foreach (HCubeMagnetism hCubeMag in stageDatas[i].GetHCubeMagCS()) { hCubeMag.enabled = false; }
                foreach (MoveHCubeR moveHCubeR in stageDatas[i].GetMoveHCubeRCS()) { moveHCubeR.enabled = false; }
            }

            // �������̂�ڑ����鎥�̓I�u�W�F�N�g
            if (stageDatas[i].magObjConnecter != null)
            {
                foreach (CubeMagnetism CubeMag in stageDatas[i].GetCubeMagCS()) { CubeMag.enabled = false; }
            }
        }

        // �w�肳�ꂽ�X�e�[�W�̊e�X�N���v�g��L����
        if (_stageIndex >= 0 &&  _stageIndex < stageDatas.Count)
        {
            //foreach (GameObject obj in stageDatas[_stageIndex].magObjects)
            //{
            //    if (obj != null) { obj.SetActive(true); }
            //}

            // ���̂̎��̓I�u�W�F�N�g
            if (stageDatas[_stageIndex].magObjSphere != null)
            {
                foreach (SphereMagnetism sphereMag in stageDatas[_stageIndex].GetSphereMagCS()) { sphereMag.enabled = true; }
                foreach (MoveSphere moveSphere in stageDatas[_stageIndex].GetMoveSphereCS()) { moveSphere.enabled = true; }
            }

            // �������̂̍����̎��̓I�u�W�F�N�g
            if (stageDatas[_stageIndex].magObjSplit1 != null)
            {
                foreach (HCubeMagnetism hCubeMag in stageDatas[_stageIndex].GetHCubeMagCS()) { hCubeMag.enabled = true; }
                foreach (MoveHCubeL moveHCubeL in stageDatas[_stageIndex].GetMoveHCubeLCS()) { moveHCubeL.enabled = true; }
            }

            // �������̂̉E���̎��̓I�u�W�F�N�g
            if (stageDatas[_stageIndex].magObjSplit2 != null)
            {
                foreach (HCubeMagnetism hCubeMag in stageDatas[_stageIndex].GetHCubeMagCS()) { hCubeMag.enabled = true; }
                foreach (MoveHCubeR moveHCubeR in stageDatas[_stageIndex].GetMoveHCubeRCS()) { moveHCubeR.enabled = true; }
            }

            // �������̂�ڑ����鎥�̓I�u�W�F�N�g
            if (stageDatas[_stageIndex].magObjConnecter != null)
            {
                foreach (CubeMagnetism CubeMag in stageDatas[_stageIndex].GetCubeMagCS()) { CubeMag.enabled = true; }
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
