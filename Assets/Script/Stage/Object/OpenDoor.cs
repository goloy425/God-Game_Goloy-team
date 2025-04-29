//--------------------------------------------------------
// 製作者　本田洸都
// クリア時のドアが開く処理
// doorFlameオブジェクトにアタッチする
//--------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [Header("ドアが開く速度")]
    public float moveSpeed = 1.8f;

    [Header("ドアが開くのにかかる秒数")]
    public float openTime = 1.0f;

    [Header("ドアがあるステージ数")]
    public int curStage;

    private GameManager gameManager;    // ゲームマネージャー
    private GameObject leftDoor;
    private GameObject rightDoor;
    private bool openFg = false;            // 開けるかどうかのフラグ
    private bool completeOpenFg = false;    // 開いたかどうかのフラグ
    private float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        // ゲームマネージャーを取得
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // 左右のドアオブジェクトを取得
        leftDoor = transform.Find("doorL_low").gameObject;
        rightDoor = transform.Find("doorR_low").gameObject;

        openFg = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //// テストでキーボードのAを押すと動く
        //if (Input.GetKey(KeyCode.A))
        //{
        //    openFg = true;
        //    Debug.Log(openFg);
        //}

        // 添え字で指定するので-1する
        openFg = gameManager.GetStageClearFg(curStage - 1);

        // フラグが立っているかつ、開けるのにかかる秒数以内の時
        if (openFg && !completeOpenFg && timer <= openTime)
        {
            timer += Time.deltaTime;
            // 左のドアを移動
            leftDoor.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.Self);
            // 右のドアを移動
            rightDoor.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.Self);
        }
        else if(timer > openTime)
        {
            completeOpenFg = true;
            timer = 0.0f;
        }
    }

    // 開いたかどうかのゲッター
    public bool GetCompleteOpenFg()
    {
        return completeOpenFg;
    }
}
