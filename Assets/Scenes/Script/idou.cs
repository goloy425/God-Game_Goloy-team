using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class idou : MonoBehaviour
{
    public float speed = 5.0f; // �ړ����x

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 direction = Vector3.zero;

        // ����̃L�[�Ɋ�Â��ĕ�����ݒ�
        if (Input.GetKey(KeyCode.W)) // W�L�[�őO�i
        {
            direction += Vector3.left;
        }
        if (Input.GetKey(KeyCode.S)) // S�L�[�Ō��
        {
            direction += Vector3.right;
        }
        if (Input.GetKey(KeyCode.A)) // A�L�[�ō�
        {
            direction += Vector3.back;
        }
        if (Input.GetKey(KeyCode.D)) // D�L�[�ŉE
        {
            direction += Vector3.forward;
        }

        // �I�u�W�F�N�g���ړ�
        transform.Translate(direction * speed * Time.deltaTime, Space.World);


    }
}
