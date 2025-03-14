using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlates01 : MonoBehaviour
{
    int NowPressing = 0; // �����ɐG��Ă���I�u�W�F�N�g�̐���ێ�������
    private Renderer plateRenderer; // Renderer�̎Q�Ƃ�ێ�

    // �F�̒�`
    public Color defaultColor = Color.green;  // �����F
    public Color pressedColor = Color.red;    // ���܂ꂽ���̐F

    // Start is called before the first frame update
    void Start()
    {
        // Renderer�R���|�[�l���g���擾
        plateRenderer = GetComponent<Renderer>();
        // �����F��ݒ�
        if (plateRenderer != null)
        {
            plateRenderer.material.color = defaultColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (NowPressing >= 1)
        {
            // �F�������ꂽ��ԂɕύX
            plateRenderer.material.color = pressedColor;
            // 1�l�ȏ�œ����ɐG�ꂽ�Ƃ��̏���
            Debug.Log("�����ł��ғ����Ă��܂�");
        }
        else
        {
            // �F�����ɖ߂�
            plateRenderer.material.color = defaultColor;
        }
    }
    void OnTriggerEnter(Collider col) // �g���K�[�̈�ɃI�u�W�F�N�g���������ꍇ
    {
        NowPressing++;
    }
    void OnTriggerExit(Collider col) // �g���K�[�̈悩��I�u�W�F�N�g���ޏo�����ꍇ
    {
        NowPressing--;
    }
}

