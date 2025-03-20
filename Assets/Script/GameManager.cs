using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// ����ҁ@�S���C�q�f�L

public class GameManager : MonoBehaviour
{
    public PressurePlates01[] pressurePlates; // ���ׂĂ̊�����o�^
    private int totalPressed = 0; // ������Ă��銴���̐�

    public string resultSceneName = "Result"; // �J�ڐ�̃V�[������Inspector�Őݒ�

    // Start is called before the first frame update
    void Start()
    {
        foreach (PressurePlates01 plate in pressurePlates)
        {
            plate.OnPressurePlateChanged += OnPlateStateChanged; // �C�x���g�o�^
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnPlateStateChanged(bool isPressed)
    {
        totalPressed += isPressed ? 1 : -1; // ������Ă��鐔�𑝌�

        if (totalPressed == pressurePlates.Length) // ���ׂĂ������ꂽ�ꍇ
        {
            Debug.Log("�S�Ă̊�����������Ă��܂��I�Q�[���N���A�IResult��ʂɈڂ�܂�");
            // �����ɃQ�[���N���A����������
            SceneManager.LoadScene(resultSceneName);
        }
    }
}
