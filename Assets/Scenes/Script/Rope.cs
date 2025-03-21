using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//===========================================================
// �쐬��
// �R���v�l
// 
// 2025/3/21 
// �X�V�@�R���v�l
//
//===========================================================

public class Rope : MonoBehaviour
{
    // �߂̐������߂�
    public GameObject[] vertices = new GameObject[15];
    LineRenderer line;

    // Start is called before the first frame update
    void Start()
    {
        // LineRenderer���擾
        line = GetComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Unlit/Color"));
        line.positionCount = vertices.Length;

        foreach (GameObject v in vertices)
        {
            v.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        int idx = 0;
        foreach (GameObject v in vertices)
        {
            line.SetPosition(idx, v.transform.position);
            idx++;
        }
    }
}
