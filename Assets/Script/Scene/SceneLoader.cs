using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// ����ҁ@�S���C�q�f�L

public class SceneLoader : MonoBehaviour
{
    public string[] sceneNames; // �V�[������Inspector�Ń��X�g��
    private int currentSceneIndex = 0; // ���݂̃V�[���C���f�b�N�X�ŕێ�

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // �R���g���[���[�̓��͂��`�F�b�N
        if (Input.GetButtonDown("Submit"))
        {
            LoadNextScene();
        }
    }

    public void LoadNextScene()
    {
        // ���̃V�[�������v�Z���Đ؂�ւ�
        currentSceneIndex = (currentSceneIndex + 1) % sceneNames.Length;
        SceneManager.LoadScene(sceneNames[currentSceneIndex]);

    }

    void Awake()
    {
        if (FindObjectsOfType<SceneLoader>().Length > 1)
        {
            Destroy(gameObject); // ������h��
            return;
        }

        DontDestroyOnLoad(this.gameObject); // �V�[�����܂����ŕێ�
    }
}
