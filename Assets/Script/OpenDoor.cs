//--------------------------------------------------------
// ����ҁ@�{�c���s
// �N���A���̃h�A���J������
//--------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [Header("���E�̃h�A")]
    public GameObject leftDoor;
    public GameObject rightDoor;

    [Header("�h�A�̈ړ��x")]
    public float doorMobility = 10.0f;

    [Header("�h�A���J���̂ɂ�����b��")]
    public float openTime = 1.0f;

    private GameManager gameManager;    // �Q�[���}�l�[�W���[
    private bool openFg = false;        // �J���邩�ǂ����̃t���O

    // Start is called before the first frame update
    void Start()
    {
        // �Q�[���}�l�[�W���[���擾

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        // �e�X�g�ŃL�[�{�[�h��A�������Ɠ���
        if (Input.GetKeyDown(KeyCode.A))
        {
            openFg = true;
        }

        if (openFg)
        {

        }
    }
}
