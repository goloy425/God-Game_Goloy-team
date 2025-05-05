//--------------------------------------------------------
// ����ҁ@�{�c���s
// ��H�ڑ����̋��̂̎��΂��J������
// MagObj_Sphere�I�u�W�F�N�g�ɃA�^�b�`����
//--------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class OpenMagnet : MonoBehaviour
{
    [Header("���΂��J�����x")]
    public float moveSpeed = 1.8f;

    [Header("���΂��J���̂ɂ�����b��")]
    public float openTime = 0.5f;

    [Header("��]���x")]
    public float rotationSpeed = 3.0f;

    [Header("��H�ɍ��킹��p�x")]
    public float rotationY = 0.0f;

    private GameManager gameManager;    // �Q�[���}�l�[�W���[
    private GameObject leftMagnet;
    private GameObject rightMagnet;
    private bool openFg = false;            // �J���邩�ǂ����̃t���O
    private bool completeOpenFg = false;    // �J�������ǂ����̃t���O
    private float timer = 0.0f;
    private Vector3 initLeftPos = Vector3.zero;     // �J���O�̍����̈ʒu
    private Vector3 initRightPos = Vector3.zero;    // �J���O�̉E���̈ʒu

    // Start is called before the first frame update
    void Start()
    {
        // �Q�[���}�l�[�W���[���擾
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // ���E�̃I�u�W�F�N�g���擾
        leftMagnet = transform.Find("pcS_left_rend").gameObject;
        rightMagnet = transform.Find("pcS_right_rend").gameObject;

        // �����ʒu��ۑ�
        initLeftPos = leftMagnet.transform.position;
        initRightPos = rightMagnet.transform.position;

        openFg = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        // �e�X�g�ŃL�[�{�[�h��A�������Ɠ���
        if (Input.GetKey(KeyCode.A))
        {
            openFg = true;
            Debug.Log(openFg);
        }
        else if (Input.GetKey(KeyCode.S)) { openFg = false; }

        //------ ��H�ڑ����ɊJ���鏈�� ------//
        // �t���O�������Ă��鎞
        if (openFg && !completeOpenFg)
        {
            if (timer <= openTime)
            {
                timer += Time.deltaTime;
                // �����ړ�
                leftMagnet.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.Self);
                // �E���ړ�
                rightMagnet.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.Self);
            }
            else
            {
                completeOpenFg = true;
                timer = 0.0f;
            }

            // ���E����H�̂�������Ɉړ������邽�߂ɉ�]������
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, rotationY, 0.0f), rotationSpeed * Time.deltaTime);
        }
        //------ ��H�ؒf���ɕ��鏈�� ------//
        else if (!openFg && completeOpenFg)
        {
            if (timer <= openTime)
            {
                timer += Time.deltaTime;
                // �����ړ�
                leftMagnet.transform.Translate(-Vector3.right * moveSpeed * Time.deltaTime, Space.Self);
                // �E���ړ�
                rightMagnet.transform.Translate(-Vector3.left * moveSpeed * Time.deltaTime, Space.Self);
            }
            else
            {
                completeOpenFg = false;
                timer = 0.0f;
            }
        }
    }

    // �J�������ǂ����̃Q�b�^�[
    public bool GetCompleteOpenFg()
    {
        return completeOpenFg;
    }

    // �J����t���O��ύX
    public void SetOpenFg(bool _openFg)
    {
        openFg = _openFg;
    }
}
