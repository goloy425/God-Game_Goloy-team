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

    [Header("�h�A���J�����x")]
    public float moveSpeed = 1.8f;

    [Header("�h�A���J���̂ɂ�����b��")]
    public float openTime = 1.0f;

    private GameManager gameManager;    // �Q�[���}�l�[�W���[
    private bool openFg = false;        // �J���邩�ǂ����̃t���O
    private float timer = 0.0f;         

    // Start is called before the first frame update
    void Start()
    {
        // �Q�[���}�l�[�W���[���擾

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

        // �t���O�������Ă��邩�A�J����̂ɂ�����b���ȓ��̎�
        if (openFg && timer <= openTime)
        {
            timer += Time.deltaTime;
            // ���̃h�A���ړ�
            leftDoor.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.Self);
            // �E�̃h�A���ړ�
            rightDoor.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.Self);
        }
    }
}
