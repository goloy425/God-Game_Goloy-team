using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // �R���C�_�[�̃T�C�Y���擾
        Vector3 colliderSize = GetComponent<Collider>().bounds.size;
        Debug.Log("�R���C�_�[�̃T�C�Y: " + colliderSize);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}