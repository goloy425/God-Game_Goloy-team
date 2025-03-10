using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class idou : MonoBehaviour
{
    public float speed = 5.0f; // 移動速度

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 direction = Vector3.zero;

        // 特定のキーに基づいて方向を設定
        if (Input.GetKey(KeyCode.W)) // Wキーで前進
        {
            direction += Vector3.left;
        }
        if (Input.GetKey(KeyCode.S)) // Sキーで後退
        {
            direction += Vector3.right;
        }
        if (Input.GetKey(KeyCode.A)) // Aキーで左
        {
            direction += Vector3.back;
        }
        if (Input.GetKey(KeyCode.D)) // Dキーで右
        {
            direction += Vector3.forward;
        }

        // オブジェクトを移動
        transform.Translate(direction * speed * Time.deltaTime, Space.World);


    }
}
