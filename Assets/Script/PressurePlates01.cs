using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlates01 : MonoBehaviour
{
    public Action<bool> OnPressurePlateChanged; // 感圧板の状態変化を通知するイベント

    int NowPressing = 0; // 感圧板に触れているオブジェクトの数を保持するやつ
    private Renderer plateRenderer; // Rendererの参照を保持

    // 色の定義
    public Color defaultColor = Color.green;  // 初期色
    public Color pressedColor = Color.red;    // 踏まれた時の色

    // Start is called before the first frame update
    void Start()
    {
        // Rendererコンポーネントを取得
        plateRenderer = GetComponent<Renderer>();
        // 初期色を設定
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
            // 色を押された状態に変更
            plateRenderer.material.color = pressedColor;
            // 1人以上で同時に触れたときの処理
            Debug.Log("感圧版が稼働しています");
        }
        else
        {
            // 色を元に戻す
            plateRenderer.material.color = defaultColor;
        }
    }
    void OnTriggerEnter(Collider col) // トリガー領域にオブジェクトが入った場合
    {
        NowPressing++;
        OnPressurePlateChanged?.Invoke(true); // 押された状態を通知
    }
    void OnTriggerExit(Collider col) // トリガー領域からオブジェクトが退出した場合
    {
        NowPressing--;
        OnPressurePlateChanged?.Invoke(false); // 離れた状態を通知
    }
}

