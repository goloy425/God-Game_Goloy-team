//--------------------------------------------------------
// 製作者　本田洸都
// クリア時のドアが開く処理
//--------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [Header("左右のドア")]
    public GameObject leftDoor;
    public GameObject rightDoor;

    [Header("ドアの移動度")]
    public float doorMobility = 10.0f;

    [Header("ドアが開くのにかかる秒数")]
    public float openTime = 1.0f;

    private GameManager gameManager;    // ゲームマネージャー
    private bool openFg = false;        // 開けるかどうかのフラグ

    // Start is called before the first frame update
    void Start()
    {
        // ゲームマネージャーを取得

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        // テストでキーボードのAを押すと動く
        if (Input.GetKeyDown(KeyCode.A))
        {
            openFg = true;
        }

        if (openFg)
        {

        }
    }
}
