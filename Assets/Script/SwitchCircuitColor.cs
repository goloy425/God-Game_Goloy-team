//----------------------------------------------------------------------
// �{�c���s
// ��H�̐F����x�����؂�ւ���
//----------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCircuitColor : MonoBehaviour
{
    [Header("�ڑ���̐F")]
    public Color color;

    private void Awake()
    {
        // ������ԂƂ��Ĕ�A�N�e�B�u�ɂ���
        this.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        // �F��ύX
        GetComponent<Renderer>().material.color = color;
    }
}
