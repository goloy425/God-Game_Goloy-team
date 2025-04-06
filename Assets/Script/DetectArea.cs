//--------------------------------------------------------------------------
// 本田洸都
// 指定されたオブジェクトがエリア内にいるかどうかを判定
//--------------------------------------------------------------------------
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
    [Header("接続後の回路の色")]
    public Color circuitColor;
    [Header("判定エリアと繋がっている回路")]
    public Renderer[] circuitsRenderer;
    [Header("接続時のSE")]
    public AudioClip audioClip;

    private AudioSource audioSource;
    private bool isConnectFg = false; // 回路が繋がったかどうか

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // 色を変更
        GetComponent<Renderer>().material.color = color;
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
                // 判定エリア内のオブジェクトが指定された磁力オブジェクトと同じ時、繋げる
                if (other.gameObject == detectedObjects[i])
                {
                    // 判定エリアと繋がっている回路の色を変更
                    for (int j = 0; j < circuitsRenderer.Length; j++)
                    {
                        circuitsRenderer[j].material.color = circuitColor;
                        audioSource.PlayOneShot(audioClip);     // SE再生
                    }
                    isConnectFg = true;
                    Debug.Log(isConnectFg);
                }
            }
        }
    }

    // 繋がったかどうかのフラグを取得
    public bool GetIsConnectFg()
    {
        return isConnectFg;
    }
}
