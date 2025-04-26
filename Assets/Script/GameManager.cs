using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

// ����ҁ@�S���C�q�f�L   �{�c���s


// �O������ݒ�\�ȃX�e�[�W�̃f�[�^�N���X
[System.Serializable]
public class StageData
{
    public List<GameObject> magObjSphere = new List<GameObject>();              // ���̂̎��̓I�u�W�F�N�g
    public List<GameObject> magObjSplit1 = new List<GameObject>();              // �������̂̍����̎��̓I�u�W�F�N�g
    public List<GameObject> magObjSplit2 = new List<GameObject>();              // �������̂̉E���̎��̓I�u�W�F�N�g
    public List<GameObject> magObjConnecter = new List<GameObject>();           // �������̂�ڑ����鎥�̓I�u�W�F�N�g
    public List<DetectArea> detectAreas = new List<DetectArea>();               // �N���A����I�u�W�F�N�g

    private List<SphereMagnetism> sphereMagCS = new List<SphereMagnetism>();    // ���̂̎��̓X�N���v�g
    private List<HCubeMagnetism> hCubeMagCS = new List<HCubeMagnetism>();       // �������̂̎��̓X�N���v�g
    private List<CubeMagnetism> cubeMagCS = new List<CubeMagnetism>();          // �R�l�N�^�[�̎��̓X�N���v�g
                                                
    private List<MoveSphere> moveSphereCS = new List<MoveSphere>();             // ���̂̓���X�N���v�g
    private List<MoveHCubeL> moveHCubeLCS = new List<MoveHCubeL>();             // �������̂̍����̓���X�N���v�g
    private List<MoveHCubeR> moveHCubeRCS = new List<MoveHCubeR>();             // �������̂̉E���̓���X�N���v�g
                                                
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

    private float clearTimer = 0.0f;          // �ڑ����ꑱ���Ă���b��
    private float clearTime = 4.0f;           // �ڑ�����Ă���b�������̕b���𒴂���ƃN���A�Ƃ݂Ȃ�
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

        //// �X�e�[�W1�̊e���̓X�N���v�g��L����
        //ActiveMagObjects(0);

        // �S�X�e�[�W�̈ړ��ł��鎥�̓I�u�W�F�N�g�̒T��
        for (int i = 0; i < stageDatas.Count; i++)
        {
            SearchCanMoveMagObj(i);
            Debug.Log(i);
            Debug.Log(stageDatas.Count);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        // ----- �ړ��ł��鎥�̓I�u�W�F�N�g�̒T�� ----- //
        SearchCanMoveMagObj(curStage);

        // ----- �X�e�[�W�̃N���A���� ----- //
        int connectCount = 0;  // �q�����Ă��锻��G���A

        // ���݂̃X�e�[�W�̔���G���A���q�����Ă��邩�𒲂ׂ�
        for (int i = 0; i < stageDatas[curStage].detectAreas.Count; i++)
        {
            // �q�����Ă��鎞�A���̐����J�E���g
            if (stageDatas[curStage].detectAreas[i].GetIsConnectFg())
            {
                connectCount++;

                // ���ׂĂ̔���G���A���q�����Ă��鎞�A�b���ɂ���ăN���A�t���O��ύX
                if (connectCount == stageDatas[curStage].detectAreas.Count)
                {
                    clearTimer += Time.deltaTime;   // �q�����Ă���b�����v��

                    // �N���A�Ƃ݂Ȃ��b���𒴂�����N���A
                    if (clearTimer > clearTime) 
                    {
                        stageDatas[curStage].SetClearFg(true);
                        clearTimer = 0.0f;   // �^�C�}�[���Z�b�g
                    }   
                }
            }
        }

        // ���݂̃X�e�[�W���N���A�������A���̃X�e�[�W������Ύ��̃X�e�[�W�ɐi��
        if (stageDatas[curStage].GetClearFg() && curStage + 1 < stageDatas.Count)
        {
            //DisableMagObjectCS(curStage);                 // ���̃X�e�[�W�̎��̓I�u�W�F�N�g��L����
            stageDatas[curStage].SetClearFg(false);   // ������X�V���Ȃ��悤�Ƀt���O��܂�
            curStage++;
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

    // ����G���A�I�u�W�F�N�g�̏�Ԃ����m
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

    // �w�肵���X�e�[�W�̈ړ��ł��鎥�̓I�u�W�F�N�g��T�����A�X�N���v�g�̗L����Ԃ�ύX
    private void SearchCanMoveMagObj(int _index)
    {
        int cnt = 0;    // ���[�v��
        // ���̂̎��̓I�u�W�F�N�g
        foreach (GameObject magObj in stageDatas[_index].magObjSphere)
        {
            if (magObj != null)
            {
                // �v���C���[L�̎��͔͈͂ɓ����Ă��鎥�̓I�u�W�F�N�g�̃X�N���v�g�̂ݗL����
                if (GetDistancePlayerMagToMagObj(curStage, 1, cnt, magObj) < stageDatas[_index].GetSphereMagCS()[cnt].GetMagnetismRange())
                {
                    Debug.Log(_index + 1 + ": Sphere �L����");
                    stageDatas[_index].GetSphereMagCS()[cnt].enabled = true;
                    stageDatas[_index].GetMoveSphereCS()[cnt].enabled = true;
                }
                // �����Ă��Ȃ����͖�����
                else
                {
                    Debug.Log(_index + 1 + ": Sphere ������");
                    stageDatas[_index].GetSphereMagCS()[cnt].enabled = false;
                    stageDatas[_index].GetMoveSphereCS()[cnt].enabled = false;
                }

                // �v���C���[R�̎��͔͈͂ɓ����Ă��鎥�̓I�u�W�F�N�g�̃X�N���v�g�̂ݗL����
                if (GetDistancePlayerMagToMagObj(curStage, 2, cnt, magObj) < stageDatas[_index].GetSphereMagCS()[cnt].GetMagnetismRange())
                {
                    Debug.Log(_index + 1 + ": Sphere �L����");
                    stageDatas[_index].GetSphereMagCS()[cnt].enabled = true;
                    stageDatas[_index].GetMoveSphereCS()[cnt].enabled = true;
                }
                // �����Ă��Ȃ����͖�����
                else
                {
                    Debug.Log(_index + 1 + ": Sphere ������");
                    stageDatas[_index].GetSphereMagCS()[cnt].enabled = false;
                    stageDatas[_index].GetMoveSphereCS()[cnt].enabled = false;
                }
                cnt++;
            }
        }

        cnt = 0;
        // �������̂̍����̎��̓I�u�W�F�N�g
        foreach (GameObject magObj in stageDatas[_index].magObjSplit1)
        {
            if (magObj != null)
            {
                // �v���C���[L�̎��͔͈͂ɓ����Ă��鎥�̓I�u�W�F�N�g�̃X�N���v�g�̂ݗL����
                if (GetDistancePlayerMagToMagObj(curStage, 1, cnt, magObj) < stageDatas[_index].GetHCubeMagCS()[cnt].GetMagnetismRange())
                {
                    Debug.Log(_index + 1 + ": Split1 �L����");
                    stageDatas[_index].GetHCubeMagCS()[cnt].enabled = true;
                    stageDatas[_index].GetMoveHCubeLCS()[cnt].enabled = true;
                }
                // �����Ă��Ȃ����͖�����
                else
                {
                    Debug.Log(_index + 1 + ": Split1 ������");
                    stageDatas[_index].GetHCubeMagCS()[cnt].enabled = false;
                    stageDatas[_index].GetMoveHCubeLCS()[cnt].enabled = false;
                }

                // �v���C���[R�̎��͔͈͂ɓ����Ă��鎥�̓I�u�W�F�N�g�̃X�N���v�g�̂ݗL����
                if (GetDistancePlayerMagToMagObj(curStage, 2, cnt, magObj) < stageDatas[_index].GetHCubeMagCS()[cnt].GetMagnetismRange())
                {
                    Debug.Log(_index + 1 + ": Split1 �L����");
                    stageDatas[_index].GetHCubeMagCS()[cnt].enabled = true;
                    stageDatas[_index].GetMoveHCubeLCS()[cnt].enabled = true;
                }
                // �����Ă��Ȃ����͖�����
                else
                {
                    Debug.Log(_index + 1 + ": Split1 ������");
                    stageDatas[_index].GetHCubeMagCS()[cnt].enabled = false;
                    stageDatas[_index].GetMoveHCubeLCS()[cnt].enabled = false;
                }
                cnt++;
            }
        }

        cnt = 0;
        // �������̂̉E���̎��̓I�u�W�F�N�g
        foreach (GameObject magObj in stageDatas[_index].magObjSplit2)
        {
            if (magObj != null)
            {
                // �v���C���[L�̎��͔͈͂ɓ����Ă��鎥�̓I�u�W�F�N�g�̃X�N���v�g�̂ݗL����
                if (GetDistancePlayerMagToMagObj(curStage, 1, cnt, magObj) < stageDatas[_index].GetHCubeMagCS()[cnt].GetMagnetismRange())
                {
                    Debug.Log(_index + 1 + ": Split2 �L����");
                    stageDatas[_index].GetHCubeMagCS()[cnt].enabled = true;
                    stageDatas[_index].GetMoveHCubeRCS()[cnt].enabled = true;
                }
                // �����Ă��Ȃ����͖�����
                else
                {
                    Debug.Log(_index + 1 + ": Split2 ������");
                    stageDatas[_index].GetHCubeMagCS()[cnt].enabled = false;
                    stageDatas[_index].GetMoveHCubeRCS()[cnt].enabled = false;
                }

                // �v���C���[R�̎��͔͈͂ɓ����Ă��鎥�̓I�u�W�F�N�g�̃X�N���v�g�̂ݗL����
                if (GetDistancePlayerMagToMagObj(curStage, 2, cnt, magObj) < stageDatas[_index].GetHCubeMagCS()[cnt].GetMagnetismRange())
                {
                    Debug.Log(_index + 1 + ": Split2 �L����");
                    stageDatas[_index].GetHCubeMagCS()[cnt].enabled = true;
                    stageDatas[_index].GetMoveHCubeRCS()[cnt].enabled = true;
                }
                // �����Ă��Ȃ����͖�����
                else
                {
                    Debug.Log(_index + 1 + ": Split2 ������");
                    stageDatas[_index].GetHCubeMagCS()[cnt].enabled = false;
                    stageDatas[_index].GetMoveHCubeRCS()[cnt].enabled = false;
                }
                cnt++;
            }
        }

        cnt = 0;
        // �������̂�ڑ����鎥�̓I�u�W�F�N�g
        foreach (GameObject magObj in stageDatas[_index].magObjConnecter)
        {
            if (magObj != null)
            {
                // �v���C���[L�̎��͔͈͂ɓ����Ă��鎥�̓I�u�W�F�N�g�̃X�N���v�g�̂ݗL����
                if (GetDistancePlayerMagToMagObj(curStage, 1, cnt, magObj) < stageDatas[_index].GetCubeMagCS()[cnt].GetMagnetismRange())
                {
                    Debug.Log(_index + 1 + ": Connecter �L����");
                    stageDatas[_index].GetCubeMagCS()[cnt].enabled = true;
                }
                // �����Ă��Ȃ����͖�����
                else
                {
                    Debug.Log(_index + 1 + ": Connecter ������");
                    stageDatas[_index].GetCubeMagCS()[cnt].enabled = false;
                }

                // �v���C���[R�̎��͔͈͂ɓ����Ă��鎥�̓I�u�W�F�N�g�̃X�N���v�g�̂ݗL����
                if (GetDistancePlayerMagToMagObj(curStage, 2, cnt, magObj) < stageDatas[_index].GetCubeMagCS()[cnt].GetMagnetismRange())
                {
                    Debug.Log(_index + 1 + ": Connecter �L����");
                    stageDatas[_index].GetCubeMagCS()[cnt].enabled = true;
                }
                // �����Ă��Ȃ����͖�����
                else
                {
                    Debug.Log(_index + 1 + ": Connecter ������");
                    stageDatas[_index].GetCubeMagCS()[cnt].enabled = false;
                }
            }
        }
    }

    // �w�肵���X�e�[�W�̎��̓I�u�W�F�N�g�̃X�N���v�g�𖳌���
    private void DisableMagObjectCS(int _stageIndex)
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
    }

    // �v���C���[�̎��΂��玥�̓I�u�W�F�N�g�܂ł̋������擾
    private float GetDistancePlayerMagToMagObj(int _index, int _playerMagNumber, int _magObjNumber, GameObject _magObj)
    {
        Vector3 playerMagPos = (_playerMagNumber == 1) ? magnet1.transform.position : magnet2.transform.position;   // �v���C���[�̎��΂̈ʒu
        float playerMagArea = (_playerMagNumber == 1) ? magnetism1.magnetismRange : magnetism2.magnetismRange;      // �v���C���[�̎��΂̎��͔͈�

        Vector3 magObjPos = _magObj.transform.position;     // ���̓I�u�W�F�N�g�̈ʒu

        // ��ޕʂ̎��̓I�u�W�F�N�g�̎��͔͈͂��擾
        float magObjArea = 0.0f;
        switch (_magObj.name)
        {
            case "MagObj_Sphere":
                magObjArea = stageDatas[_index].GetSphereMagCS()[_magObjNumber].GetMagnetismRange();     // ���̂̎��̓I�u�W�F�N�g�̎��͔͈�
                break;
            case "MagObj_split1":
                magObjArea = stageDatas[_index].GetHCubeMagCS()[_magObjNumber].GetMagnetismRange();      // �������̂̍����̎��̓I�u�W�F�N�g�̎��͔͈�
                break;
            case "MagObj_split2":
                magObjArea = stageDatas[_index].GetHCubeMagCS()[_magObjNumber].GetMagnetismRange();      // �������̂̉E���̎��̓I�u�W�F�N�g�̎��͔͈�
                break;
            case "Connecter":
                magObjArea = stageDatas[_index].GetCubeMagCS()[_magObjNumber].GetMagnetismRange();       // �������̂�ڑ����鎥�̓I�u�W�F�N�g�̎��͔͈�
                break;
        }

        float centerDistance = Vector3.Distance(playerMagPos, magObjPos);   // ���S�ʒu�̋���

        float magnetismAreaDistance = Mathf.Max(0.0f, centerDistance - (playerMagArea + magObjArea));   // ���͔͈͂̋���
        return magnetismAreaDistance;
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
