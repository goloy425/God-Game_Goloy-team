//--------------------------------------------------------
// ����ҁ@�{�c���s
// �N���A���̃h�A���J������
// doorFlame�I�u�W�F�N�g�ɃA�^�b�`����
//--------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [Header("�h�A���J�����x")]
    public float moveSpeed = 1.8f;

    [Header("�h�A���J���̂ɂ�����b��")]
    public float openTime = 1.0f;

    [Header("�h�A������X�e�[�W��")]
    public int curStage;

    private GameManager gameManager;    // �Q�[���}�l�[�W���[
    private GameObject leftDoor;
    private GameObject rightDoor;
    private bool openFg = false;            // �J���邩�ǂ����̃t���O
    private bool completeOpenFg = false;    // �J�������ǂ����̃t���O
    private float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        // �Q�[���}�l�[�W���[���擾
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // ���E�̃h�A�I�u�W�F�N�g���擾
        leftDoor = transform.Find("doorL_low").gameObject;
        rightDoor = transform.Find("doorR_low").gameObject;

        openFg = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //// �e�X�g�ŃL�[�{�[�h��A�������Ɠ���
        //if (Input.GetKey(KeyCode.A))
        //{
        //    openFg = true;
        //    Debug.Log(openFg);
        //}

        // �Y�����Ŏw�肷��̂�-1����
        openFg = gameManager.GetStageClearFg(curStage - 1);

        // �t���O�������Ă��邩�A�J����̂ɂ�����b���ȓ��̎�
        if (openFg && !completeOpenFg && timer <= openTime)
        {
            timer += Time.deltaTime;
            // ���̃h�A���ړ�
            leftDoor.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.Self);
            // �E�̃h�A���ړ�
            rightDoor.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.Self);
        }
        else if(timer > openTime)
        {
            completeOpenFg = true;
            timer = 0.0f;
        }
    }

    // �J�������ǂ����̃Q�b�^�[
    public bool GetCompleteOpenFg()
    {
        return completeOpenFg;
    }
}
