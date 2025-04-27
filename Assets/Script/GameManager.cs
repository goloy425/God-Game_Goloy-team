using System.Collections;
using System.Collections.Generic;
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
    private List<HCubeMagnetism> split1HCubeMagCS = new List<HCubeMagnetism>(); // �������̂̍����̎��̓X�N���v�g
    private List<HCubeMagnetism> split2HCubeMagCS = new List<HCubeMagnetism>(); // �������̂̉E���̎��̓X�N���v�g
    private List<CubeMagnetism> cubeMagCS = new List<CubeMagnetism>();          // �R�l�N�^�[�̎��̓X�N���v�g

    private List<MoveSphere> moveSphereCS = new List<MoveSphere>();             // ���̂̓���X�N���v�g
    private List<MoveHCubeL> moveHCubeLCS = new List<MoveHCubeL>();             // �������̂̍����̓���X�N���v�g
    private List<MoveHCubeR> moveHCubeRCS = new List<MoveHCubeR>();             // �������̂̉E���̓���X�N���v�g
    private List<SplitCube> splitCubeCS = new List<SplitCube>();                // ����X�N���v�g

    private bool clearFg = false;               // �N���A�t���O

    // �N���A�t���O�̃Q�b�^�[
    public bool GetClearFg() { return clearFg; }

    // �N���A�t���O�̃Z�b�^�[                   
    public void SetClearFg(bool _clearFg) { clearFg = _clearFg; }

    // ���̂̎��̓X�N���v�g���X�g�̃Q�b�^�[
    public List<SphereMagnetism> GetSphereMagCS() { return sphereMagCS; }

    // �������̂̍����̎��̓X�N���v�g���X�g�̃Q�b�^�[
    public List<HCubeMagnetism> GetSplit1HCubeMagCS() { return split1HCubeMagCS; }

    // �������̂̉E���̎��̓X�N���v�g���X�g�̃Q�b�^�[
    public List<HCubeMagnetism> GetSplit2HCubeMagCS() { return split2HCubeMagCS; }

    // �R�l�N�^�[�̎��̓X�N���v�g���X�g�̃Q�b�^�[
    public List<CubeMagnetism> GetCubeMagCS() { return cubeMagCS; }

    // ���̂̓���X�N���v�g���X�g�̃Q�b�^�[
    public List<MoveSphere> GetMoveSphereCS() { return moveSphereCS; }

    // �������̂̍����̓���X�N���v�g���X�g�̃Q�b�^�[
    public List<MoveHCubeL> GetMoveHCubeLCS() { return moveHCubeLCS; }

    // �������̂̉E���̓���X�N���v�g���X�g�̃Q�b�^�[
    public List<MoveHCubeR> GetMoveHCubeRCS() { return moveHCubeRCS; }

    // �������̂̉E���̓���X�N���v�g���X�g�̃Q�b�^�[
    public List<SplitCube> GetSplitCubeCS() { return splitCubeCS; }
}


public class GameManager : MonoBehaviour
{
    [Header("�e�X�e�[�W�̎��̓I�u�W�F�N�g�i�ŏ��ɃX�e�[�W����ݒ�j")]
    public List<StageData> stageData;

    [Header("�v���C���[�̎���")]
    public GameObject magnet1;
    public GameObject magnet2;

    //[Header("�e�X�e�[�W�̃N���A����I�u�W�F�N�g")]
    //public DetectArea[] detectAreas;    // ���ׂĂ̔���G���A��o�^

    [Header("�J�ڐ�̃V�[����")]
    public string resultSceneName = "Result";       // �J�ڐ�̃V�[������Inspector�Őݒ�
    public string gameOverSceneName = "GameOver";   // �J�ڐ�̃V�[������Inspector�Őݒ�

    private bool gameClearFg = false;         // �Q�[���N���A�������ǂ���
    private bool gameOverFg = false;          // �Q�[���I�[�o�[�������ǂ���

    private int totalAreas = 0;               // �ݒ肳�ꂽ����G���A�̐�
    private int totalConnected = 0;           // �ڑ����ꂽ����G���A�̐�
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

        for(int i = 0; i < stageData.Count; i++)
        {
            foreach (DetectArea detectArea in stageData[i].detectAreas)
            {
                detectArea.OnDetectAreaChanged += OnDetectionStateChanged; // �C�x���g�o�^
                totalAreas++;
            }
        }

        // �v���C���[�̎��΂̎��̓X�N���v�g���擾
        magnetism1 = magnet1.GetComponent<Magnetism>();
        magnetism2 = magnet2.GetComponent<Magnetism>();

        // �ݒ肳�ꂽ�S�Ă̎��̓I�u�W�F�N�g�ɃA�^�b�`���ꂽ�e�X�N���v�g�����X�g�ɒǉ�
        AddAllMagCSList();

        Debug.Log("�X�e�[�W�� : " + stageData.Count);
        // �S�X�e�[�W�̈ړ��ł��鎥�̓I�u�W�F�N�g�̒T��
        for (int i = 0; i < stageData.Count; i++)
        {
            Debug.Log("�X�e�[�W" + (i + 1) + "������");
            SearchCanCarryMagObj(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        // ----- �ړ��ł��鎥�̓I�u�W�F�N�g�̒T�� ----- //
        //SearchCanCarryMagObj(curStage);
        for (int i = 0; i < stageData.Count; i++)
        {
            SearchCanCarryMagObj(i);
        }

        // ----- �X�e�[�W�̃N���A���� ----- //
        int connectCount = 0;  // �q�����Ă��锻��G���A

        // ���݂̃X�e�[�W�̔���G���A���q�����Ă��邩�𒲂ׂ�
        for (int i = 0; i < stageData[curStage].detectAreas.Count; i++)
        {
            // �q�����Ă��鎞�A���̐����J�E���g
            if (stageData[curStage].detectAreas[i].GetIsConnectFg())
            {
                connectCount++;

                // ���ׂĂ̔���G���A���q�����Ă��鎞�A�b���ɂ���ăN���A�t���O��ύX
                if (connectCount == stageData[curStage].detectAreas.Count)
                {
                    clearTimer += Time.deltaTime;   // �q�����Ă���b�����v��

                    // �N���A�Ƃ݂Ȃ��b���𒴂�����N���A
                    if (clearTimer > clearTime)
                    {
                        stageData[curStage].SetClearFg(true);
                        clearTimer = 0.0f;   // �^�C�}�[���Z�b�g
                    }
                }
            }
        }

        // ���݂̃X�e�[�W���N���A�������A���̃X�e�[�W������Ύ��̃X�e�[�W�ɐi��
        if (stageData[curStage].GetClearFg() && curStage + 1 < stageData.Count)
        {
            stageData[curStage].SetClearFg(false);   // ������X�V���Ȃ��悤�Ƀt���O��܂�
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

    // ����G���A�I�u�W�F�N�g�̏�Ԃ����m
    void OnDetectionStateChanged(bool isConnected)
    {
        totalConnected += isConnected ? 1 : -1; // �ڑ�����Ă��鐔�𑝌�
        Debug.Log(totalConnected);

        if (totalConnected == totalAreas)
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
        for (int i = 0; i < stageData.Count; i++)
        {
            // ���̂̎��̓I�u�W�F�N�g�̊e�X�N���v�g�����X�g�ɒǉ�
            foreach (GameObject magObj in stageData[i].magObjSphere)
            {
                if (magObj != null)
                {
                    stageData[i].GetSphereMagCS().Add(magObj.GetComponent<SphereMagnetism>());
                    stageData[i].GetMoveSphereCS().Add(magObj.GetComponent<MoveSphere>());
                }
            }

            // �������̂̍����̎��̓I�u�W�F�N�g�̊e�X�N���v�g�����X�g�ɒǉ�
            foreach (GameObject magObj in stageData[i].magObjSplit1)
            {
                if (magObj != null)
                {
                    stageData[i].GetSplit1HCubeMagCS().Add(magObj.GetComponent<HCubeMagnetism>());
                    stageData[i].GetMoveHCubeLCS().Add(magObj.GetComponent<MoveHCubeL>());
                }
            }

            // �������̂̉E���̎��̓I�u�W�F�N�g�̊e�X�N���v�g�����X�g�ɒǉ�
            foreach (GameObject magObj in stageData[i].magObjSplit2)
            {
                if (magObj != null)
                {
                    stageData[i].GetSplit2HCubeMagCS().Add(magObj.GetComponent<HCubeMagnetism>());
                    stageData[i].GetMoveHCubeRCS().Add(magObj.GetComponent<MoveHCubeR>());
                }
            }

            // �������̂�ڑ����鎥�̓I�u�W�F�N�g�̊e�X�N���v�g�����X�g�ɒǉ�
            foreach (GameObject magObj in stageData[i].magObjConnecter)
            {
                if (magObj != null)
                {
                    stageData[i].GetCubeMagCS().Add(magObj.GetComponent<CubeMagnetism>());
                    stageData[i].GetSplitCubeCS().Add(magObj.GetComponent<SplitCube>());
                }
            }
        }
    }

    // �w�肵���X�e�[�W�̈ړ��ł��鎥�̓I�u�W�F�N�g��T�����A�X�N���v�g�̗L����Ԃ�ύX
    private void SearchCanCarryMagObj(int _index)
    {
        // ���̂̎��̓I�u�W�F�N�g
        for (int i = 0; i < stageData[_index].magObjSphere.Count; i++)
        {
            GameObject magObj = stageData[_index].magObjSphere[i];

            if (magObj != null)
            {
                // �v���C���[�̎��͔͈͂ɓ����Ă��鎥�̓I�u�W�F�N�g�̃X�N���v�g�̂ݗL����
                if (GetDistancePlayerMagToMagObj(_index, 1, i, magObj) < stageData[_index].GetSphereMagCS()[i].GetMagnetismRange() ||
                    GetDistancePlayerMagToMagObj(_index, 2, i, magObj) < stageData[_index].GetSphereMagCS()[i].GetMagnetismRange())
                {
                    Debug.Log(_index + 1 + ": Sphere �L����");
                    stageData[_index].GetSphereMagCS()[i].enabled = true;
                    stageData[_index].GetMoveSphereCS()[i].enabled = true;
                }
                // �����Ă��Ȃ����͖�����
                else if (!magnetism1.inObjMagArea && !magnetism2.inObjMagArea)
                {
                    Debug.Log(_index + 1 + ": Sphere ������");
                    stageData[_index].GetSphereMagCS()[i].enabled = false;
                    stageData[_index].GetMoveSphereCS()[i].enabled = false;
                }
            }
        }

        // �������̂̍����̎��̓I�u�W�F�N�g
        for (int i = 0; i < stageData[_index].magObjSplit1.Count; i++)
        {
            GameObject magObj = stageData[_index].magObjSplit1[i];
            GameObject connecter = stageData[_index].magObjConnecter[i];

            if (magObj != null)
            {
                // ������Ńv���C���[L�̎��͔͈͂ɓ����Ă��鎥�̓I�u�W�F�N�g�̃X�N���v�g�̂ݗL����
                if (GetDistancePlayerMagToMagObj(_index, 1, i, magObj) < stageData[_index].GetSplit1HCubeMagCS()[i].GetMagnetismRange() &&
                    !connecter.activeSelf)
                {
                    Debug.Log(_index + 1 + ": Split1 �L����");
                    stageData[_index].GetSplit1HCubeMagCS()[i].enabled = true;
                    stageData[_index].GetMoveHCubeLCS()[i].enabled = true;
                }
                // �����O�܂��́A�����Ă��Ȃ����͖�����
                else if (!magnetism1.inObjMagArea || connecter.activeSelf)
                {
                    Debug.Log(_index + 1 + ": Split1 ������");
                    stageData[_index].GetSplit1HCubeMagCS()[i].enabled = false;
                    stageData[_index].GetMoveHCubeLCS()[i].enabled = false;
                }

                // ������Ńv���C���[R�̎��͔͈͂ɓ����Ă��鎥�̓I�u�W�F�N�g�̃X�N���v�g�̂ݗL����
                if (GetDistancePlayerMagToMagObj(_index, 2, i, magObj) < stageData[_index].GetSplit1HCubeMagCS()[i].GetMagnetismRange() &&
                    !connecter.activeSelf)
                {
                    Debug.Log(_index + 1 + ": Split1 �L����");
                    stageData[_index].GetSplit1HCubeMagCS()[i].enabled = true;
                    stageData[_index].GetMoveHCubeLCS()[i].enabled = true;
                }
                // �����O�܂��́A�����Ă��Ȃ����͖�����
                else if (!magnetism2.inObjMagArea || connecter.activeSelf)
                {
                    Debug.Log(_index + 1 + ": Split1 ������");
                    stageData[_index].GetSplit1HCubeMagCS()[i].enabled = false;
                    stageData[_index].GetMoveHCubeLCS()[i].enabled = false;
                }
            }
        }

        // �������̂̉E���̎��̓I�u�W�F�N�g
        for (int i = 0; i < stageData[_index].magObjSplit2.Count; i++)
        {
            GameObject magObj = stageData[_index].magObjSplit2[i];
            GameObject connecter = stageData[_index].magObjConnecter[i];

            if (magObj != null)
            {
                // ������Ńv���C���[L�̎��͔͈͂ɓ����Ă��鎥�̓I�u�W�F�N�g�̃X�N���v�g�̂ݗL����
                if (GetDistancePlayerMagToMagObj(_index, 1, i, magObj) < stageData[_index].GetSplit2HCubeMagCS()[i].GetMagnetismRange() &&
                    !connecter.activeSelf)
                {
                    Debug.Log(_index + 1 + ": Split2 �L����");
                    stageData[_index].GetSplit2HCubeMagCS()[i].enabled = true;
                    stageData[_index].GetMoveHCubeRCS()[i].enabled = true;
                }
                // �����O�܂��́A�����Ă��Ȃ����͖�����
                else if (!magnetism1.inObjMagArea || connecter.activeSelf)
                {
                    Debug.Log(_index + 1 + ": Split2 ������");
                    stageData[_index].GetSplit2HCubeMagCS()[i].enabled = false;
                    stageData[_index].GetMoveHCubeRCS()[i].enabled = false;
                }

                // ������Ńv���C���[R�̎��͔͈͂ɓ����Ă��鎥�̓I�u�W�F�N�g�̃X�N���v�g�̂ݗL����
                // Split2�̓v���C���[R�ł̂ݓ�������̂ŁA�����Ő���
                if (GetDistancePlayerMagToMagObj(_index, 2, i, magObj) < stageData[_index].GetSplit2HCubeMagCS()[i].GetMagnetismRange() &&
                    !connecter.activeSelf)
                {
                    Debug.Log(_index + 1 + ": Split2 �L����");
                    stageData[_index].GetSplit2HCubeMagCS()[i].enabled = true;
                    stageData[_index].GetMoveHCubeRCS()[i].enabled = true;
                }
                // �����O�܂��́A�����Ă��Ȃ����͖�����
                else if (!magnetism2.inObjMagArea || connecter.activeSelf)
                {
                    Debug.Log(_index + 1 + ": Split2 ������");
                    stageData[_index].GetSplit2HCubeMagCS()[i].enabled = false;
                    stageData[_index].GetMoveHCubeRCS()[i].enabled = false;
                }
            }
        }

        // �������̂�ڑ����鎥�̓I�u�W�F�N�g
        for (int i = 0; i < stageData[_index].magObjConnecter.Count; i++)
        {
            GameObject magObj = stageData[_index].magObjConnecter[i];

            if (magObj != null)
            {
                // �v���C���[�̎��͔͈͂ɓ����Ă��鎥�̓I�u�W�F�N�g�̃X�N���v�g�̂ݗL����
                if (GetDistancePlayerMagToMagObj(_index, 1, i, magObj) < stageData[_index].GetCubeMagCS()[i].GetMagnetismRange() ||
                    GetDistancePlayerMagToMagObj(_index, 2, i, magObj) < stageData[_index].GetCubeMagCS()[i].GetMagnetismRange())
                {
                    Debug.Log(_index + 1 + ": Connecter �L����");
                    stageData[_index].GetCubeMagCS()[i].enabled = true;
                    stageData[_index].GetSplitCubeCS()[i].enabled = true;
                }
                // �����Ă��Ȃ����͖�����
                else if (!magnetism1.inObjMagArea && !magnetism2.inObjMagArea)
                {
                    Debug.Log(_index + 1 + ": Connecter ������");
                    stageData[_index].GetCubeMagCS()[i].enabled = false;
                    stageData[_index].GetSplitCubeCS()[i].enabled = false;
                }
            }
        }
    }

    // �v���C���[�̎��΂��玥�̓I�u�W�F�N�g�܂ł̋������擾
    private float GetDistancePlayerMagToMagObj(int _index, int _playerMagNumber, int _magObjNumber, GameObject _magObj)
    {
        Vector3 playerMagPos = (_playerMagNumber == 1) ? magnetism1.myPlate.transform.position : magnetism2.myPlate.transform.position;   // �v���C���[�̎��΂̈ʒu
        Vector3 magObjPos = _magObj.transform.position;     // ���̓I�u�W�F�N�g�̈ʒu

        // ��ޕʂŎ��͔͈͂̋������擾�i�eMagnetism.cs�Ɠ��������ŋ��������߂�j
        float surfaceDistance = 0.0f;
        switch (_magObj.name)
        {
            case "MagObj_Sphere":
                // Collider�𗘗p���Ĉ�ԋ߂��\�ʂ̍��W���擾
                Vector3 surfacePoint = stageData[_index].GetSphereMagCS()[_magObjNumber].GetSphereCollider().ClosestPoint(playerMagPos);
                // ���̕\�ʂƎ��΂̋������v�Z
                surfaceDistance = Vector3.Distance(surfacePoint, playerMagPos);
                break;
            case "MagObj_split1":
                // Collider�𗘗p���Ĉ�ԋ߂��\�ʂ̍��W���擾
                surfacePoint = stageData[_index].GetSplit1HCubeMagCS()[_magObjNumber].GetHCubeCollider().ClosestPoint(playerMagPos);
                // �����̍����̕\�ʂƎ��΂̋������v�Z
                surfaceDistance = Vector3.Distance(surfacePoint, playerMagPos);
                break;
            case "MagObj_split2":
                // Collider�𗘗p���Ĉ�ԋ߂��\�ʂ̍��W���擾
                surfacePoint = stageData[_index].GetSplit2HCubeMagCS()[_magObjNumber].GetHCubeCollider().ClosestPoint(playerMagPos);
                // �����̉E���̕\�ʂƎ��΂̋������v�Z
                surfaceDistance = Vector3.Distance(surfacePoint, playerMagPos);
                break;
            case "Connecter":
                // Collider�𗘗p���Ĉ�ԋ߂��\�ʂ̍��W���擾
                Vector3 surface1 = stageData[_index].GetCubeMagCS()[_magObjNumber].GetCube1Collider().ClosestPoint(playerMagPos);
                Vector3 surface2 = stageData[_index].GetCubeMagCS()[_magObjNumber].GetCube2Collider().ClosestPoint(playerMagPos);

                // �\�ʍ��W�Ƃ̋������v�Z
                float distance1 = Vector3.Distance(surface1, playerMagPos);
                float distance2 = Vector3.Distance(surface2, playerMagPos);

                Vector3 targetSurface = (distance1 < distance2) ? surface1 : surface2;
                surfaceDistance = Mathf.Min(distance1, distance2);
                break;
        }
        return surfaceDistance;
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
}
