//-------------------------------------------------------------------------
// 制作者　本田洸都
// プレイヤーUIのスケールをカメラのスケールに合わせる
//-------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyStateUIRotation : MonoBehaviour
{
    // 初期回転
    private Quaternion initialRotation;

    // Start is called before the first frame update
    void Start()
    {
        // 初期回転を取得
        initialRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        // 回転をもとに戻す
        transform.rotation = initialRotation;
    }
}
