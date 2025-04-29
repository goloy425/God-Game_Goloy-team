//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class StageManager : MonoBehaviour
//{
//    public int currentStage = 2; // ���݂̃X�e�[�W�ԍ����Ǘ�������J�ϐ�

//    // ���̃X�e�[�W�ɐi��
//    public void NextStage()
//    {
//        currentStage++; // �X�e�[�W�ԍ����X�V
//        Debug.Log("���݂̃X�e�[�W�ԍ�: " + currentStage);

//        string nextSceneName = "Stage" + currentStage; // ���̃V�[�����𐶐�
//        if (Application.CanStreamedLevelBeLoaded(nextSceneName)) // �V�[�������݂��邩�m�F
//        {
//            PlayerPrefs.SetInt("CurrentStage", currentStage); // �X�e�[�W�ԍ���ۑ�
//            SceneManager.LoadScene(nextSceneName); // ���̃V�[����ǂݍ���

//        }
//        else
//        {
//            Debug.LogWarning("���̃V�[����������܂���: " + nextSceneName);
//        }
//    }

//    // ���݂̃X�e�[�W�����g���C����
//    public void RetryStage()
//    {
//        currentStage = PlayerPrefs.GetInt("CurrentStage", currentStage); // �ۑ����ꂽ�X�e�[�W�ԍ����擾
//        string retrySceneName = "Stage" + currentStage; // ���݂̃X�e�[�W���𐶐�
//        if (Application.CanStreamedLevelBeLoaded(retrySceneName)) // �V�[�������݂��邩�m�F
//        {
//            SceneManager.LoadScene(retrySceneName); // ���݂̃V�[�����ēǂݍ���
//        }
//        else
//        {
//            Debug.LogWarning("���g���C��̃V�[����������܂���: " + retrySceneName);
//        }
//    }

//    // �����X�e�[�W�ɖ߂�
//    public void ResetToFirstStage()
//    {
//        currentStage = 2; // �X�e�[�W�ԍ���������

//        string initialSceneName = "Stage" + currentStage; // �����X�e�[�W�����w��
//        PlayerPrefs.SetInt("CurrentStage", currentStage); // �X�e�[�W�ԍ���ۑ�
//        SceneManager.LoadScene(initialSceneName); // �ŏ��̃V�[����ǂݍ���
//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public int currentStage = 1; // ���݂̃X�e�[�W�ԍ����Ǘ�������J�ϐ�

    //private static StageManager instance;

    //// Awake���\�b�h��DontDestroyOnLoad��ݒ�
    //void Awake()
    //{
    //    if (instance == null)
    //    {
    //        instance = this;
    //        DontDestroyOnLoad(gameObject); // ����GameObject���V�[���ԂŔj�����Ȃ�
    //    }
    //    else
    //    {
    //        Destroy(gameObject); // ���łɑ��݂���ꍇ�͐V�����C���X�^���X��j��
    //    }
    //}

    void Start()
    {
        // PlayerPrefs���猻�݂̃X�e�[�W��ǂݍ���
        if (PlayerPrefs.HasKey("CurrentStage"))
        {
            currentStage = PlayerPrefs.GetInt("CurrentStage");
        }
    }


    // ���̃X�e�[�W�ɐi��
    public void NextStage()
    {
        currentStage++; // �X�e�[�W�ԍ����X�V
        Debug.Log("���݂̃X�e�[�W�ԍ�: " + currentStage);

        string nextSceneName = "Stage" + currentStage; // ���̃V�[�����𐶐�
        if (Application.CanStreamedLevelBeLoaded(nextSceneName)) // �V�[�������݂��邩�m�F
        {
            PlayerPrefs.SetInt("CurrentStage", currentStage); // �X�e�[�W�ԍ���ۑ�
            SceneManager.LoadScene(nextSceneName); // ���̃V�[����ǂݍ���
        }
        else
        {
            Debug.LogWarning("���̃V�[����������܂���: " + nextSceneName);
        }


    }

    // ���݂̃X�e�[�W�����g���C����
    public void RetryStage()
    {
        currentStage = PlayerPrefs.GetInt("CurrentStage", currentStage); // �ۑ����ꂽ�X�e�[�W�ԍ����擾
        string retrySceneName = "Stage" + currentStage; // ���݂̃X�e�[�W���𐶐�
        if (Application.CanStreamedLevelBeLoaded(retrySceneName)) // �V�[�������݂��邩�m�F
        {
            SceneManager.LoadScene(retrySceneName); // ���݂̃V�[�����ēǂݍ���
        }
        else
        {
            Debug.LogWarning("���g���C��̃V�[����������܂���: " + retrySceneName);
        }
    }

    // �����X�e�[�W�ɖ߂�
    public void ResetToFirstStage()
    {
        currentStage = 1; // �X�e�[�W�ԍ���������

        string initialSceneName = "Stage" + currentStage; // �����X�e�[�W�����w��
        PlayerPrefs.SetInt("CurrentStage", currentStage); // �X�e�[�W�ԍ���ۑ�
        SceneManager.LoadScene(initialSceneName); // �ŏ��̃V�[����ǂݍ���
    }
}