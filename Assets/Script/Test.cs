using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // コライダーのサイズを取得
        Vector3 colliderSize = GetComponent<Collider>().bounds.size;
        Debug.Log("コライダーのサイズ: " + colliderSize);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}