//--------------------------------------------------------
// ����ҁ@�{�c���s
// �N���A���̃h�A���J������
// doorFlame�I�u�W�F�N�g�ɃA�^�b�`����
//--------------------------------------------------------
using System;
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

    Material mat;		// �g�p���Ă���}�e���A��
    private Color emissionColor = new Color(0.24f, 0.75f, 0.39f, 1.0f);		// �ύX��̐F
    private float intensity = 3.5f;		// ���̋��x
    private Color finalEmissionColor;   // �F�ƌ��̋��x���猈�肳�ꂽ�F

    private bool isMoving = false;  // �h�A�������Ă��邩�ǂ����iVibration�X�N���v�g�ɂĎg�p�j

    // Start is called before the first frame update
    void Start()
    {
        // �Q�[���}�l�[�W���[���擾
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // ���E�̃h�A�I�u�W�F�N�g���擾
        leftDoor = transform.Find("doorL_rend").gameObject;
        rightDoor = transform.Find("doorR_rend").gameObject;

        openFg = false;

        //------ �h�A���J������̃��C�g�̐F ------//
        // �}�e���A�����擾
        mat = transform.Find("doorframe_render").GetComponent<Renderer>().material;
        // HDR�̋��x��K�p�iColor * ���x�j
        finalEmissionColor = emissionColor * intensity;
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
            // Emission��L����
            mat.SetColor("_EmissionColor", finalEmissionColor);
            // Emission��K�p���邽�߂̃t���O��ݒ�
            mat.EnableKeyword("_EMISSION");

            timer += Time.deltaTime;
            // ���̃h�A���ړ�
            leftDoor.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.Self);
            // �E�̃h�A���ړ�
            rightDoor.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.Self);

            isMoving = true;
        }
        else if (timer > openTime)
        {
            completeOpenFg = true;
            timer = 0.0f;

            isMoving = false;
        }
    }

    // �J�������ǂ����̃Q�b�^�[
    public bool GetCompleteOpenFg()
    {
        return completeOpenFg;
    }


    // �h�A�������Ă��邩�ǂ����̃Q�b�^�[
    public bool GetDoorMovingFg()
    {
        return isMoving;
    }
}
