//----------------------------------------------------------------------
// 本田洸都
// 回路の色を一度だけ切り替える
//----------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCircuitColor : MonoBehaviour
{
    [Header("接続後の色")]
    public Color color;

    private void Awake()
    {
        // 初期状態として非アクティブにする
        this.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        // 色を変更
        GetComponent<Renderer>().material.color = color;
    }
}
