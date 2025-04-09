//--------------------------------------------------------------------------
// 本田洸都
// 指定されたオブジェクトがエリア内にいるかどうかを判定
//--------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectArea : MonoBehaviour
{
    [Header("ゲーム開始時の色")]
    public Color color;
    [Header("判定エリアの中心からの大きさ")]
    public float sizeZ = 0.0f;
    public float sizeX = 0.0f;
    [Header("判定を取る磁力オブジェクト")]
    public GameObject[] detectedObjects;
    [Header("接続前の回路の色")]
    public Color initCircuitColor;
    [Header("接続後の回路の色")]
    public Color circuitColor;
    [Header("判定エリアと繋がっている回路（設定なし可）")]
    public Renderer[] circuitsRenderer;
    [Header("接続時のSE")]
    public AudioClip audioClip;

    public Action<bool> OnDetectAreaChanged;    // 検知状態を通知するイベント

    private AudioSource audioSource;
    private bool isConnectFg = false; // 回路が繋がったかどうか
    private int objNum = 0;           // 判定エリア内にいるオブジェクトの数

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // 色を変更
        GetComponent<Renderer>().material.color = color;

        // 回路の色を初期化
        for (int i = 0; i < circuitsRenderer.Length; i++)
        {
            circuitsRenderer[i].material.color = initCircuitColor;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // オブジェクトが衝突した時の処理
    private void OnTriggerEnter(Collider other)
    {
        Vector3 thisPos = this.transform.position;      // 判定エリアの座標
        Vector3 otherPos = other.transform.position;    // 衝突した磁力オブジェクトの座標

        // 衝突した磁力オブジェクトが判定エリア内にいる時
        if (thisPos.x - sizeX <= otherPos.x && otherPos.x <= thisPos.x + sizeX &&
            thisPos.z - sizeZ <= otherPos.z && otherPos.z <= thisPos.z + sizeZ)
        {
            // 指定された全磁力オブジェクトを検索
            for (int i = 0; i < detectedObjects.Length; i++)
            {
                // 判定エリア内のオブジェクトが指定された磁力オブジェクトと同じ時、接続
                if (other.gameObject == detectedObjects[i])
                {
                    // 判定エリア内にオブジェクトが存在しない時に実行
                    if (objNum < 1)
                    {
                        // 判定エリアと繋がっている回路の色を変更
                        for (int j = 0; j < circuitsRenderer.Length; j++)
                        {
                            circuitsRenderer[j].material.color = circuitColor;
                        }
                        audioSource.PlayOneShot(audioClip);  // SE再生
                        isConnectFg = true;
                        OnDetectAreaChanged?.Invoke(true);   // 判定を通知
                        Debug.Log("接続 isConnectFg:" + isConnectFg);
                    }
                    objNum++;
                }
            }

        }
    }

    // オブジェクトがすり抜けた時の処理
    private void OnTriggerExit(Collider other)
    {
            Vector3 thisPos = this.transform.position;      // 判定エリアの座標
            Vector3 otherPos = other.transform.position;    // 衝突した磁力オブジェクトの座標

        // 指定された全磁力オブジェクトを検索
        for (int i = 0; i < detectedObjects.Length; i++)
        {
            // 判定エリア外のオブジェクトが指定された磁力オブジェクトと同じ時、切断
            if (other.gameObject == detectedObjects[i])
            {
                // 判定エリア内に一個以下存在する時に実行
                if (objNum <= 1)
                {
                    // 判定エリアと繋がっている回路の色を変更
                    for (int j = 0; j < circuitsRenderer.Length; j++)
                    {
                        circuitsRenderer[j].material.color = initCircuitColor;
                    }
                    // audioSource.PlayOneShot(audioClip);     // SE再生
                    isConnectFg = false;
                    OnDetectAreaChanged?.Invoke(false);   // 判定を通知
                    Debug.Log("切断 isConnectFg:" + isConnectFg);
                }
                objNum--;
            }
        }
    }

    // 繋がったかどうかのフラグを取得
    public bool GetIsConnectFg()
    {
        return isConnectFg;
    }
}
