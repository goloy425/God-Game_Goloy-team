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

    // UIの向きベクトル
    private Vector3 UIdir = new Vector3(0.0f, 0.0f, 0.0f);

    // Start is called before the first frame update
    void Start()
    {
        // カメラの方向に向きベクトルを設定
        UIdir = -mainCamera.transform.forward;  
        this.transform.forward = UIdir;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
