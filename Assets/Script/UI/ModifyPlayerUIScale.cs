//-------------------------------------------------------------------------
// 制作者　本田洸都
// プレイヤーUIのスケールをカメラのスケールに合わせる
//-------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyPlayerUIScale : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    private Vector3 prevCamScale = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 curCamPos = new Vector3(0.0f, 0.0f, 0.0f);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        prevCamScale = curCamPos;
        curCamPos = mainCamera.transform.localScale;
    }
}
