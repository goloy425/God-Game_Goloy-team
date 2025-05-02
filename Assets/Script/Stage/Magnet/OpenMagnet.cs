//--------------------------------------------------------
// 製作者　本田洸都
// 回路接続時の球体の磁石が開く処理
// MagObj_Sphereオブジェクトにアタッチする
//--------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class OpenMagnet : MonoBehaviour
{
    [Header("磁石が開く速度")]
    public float moveSpeed = 1.8f;

    [Header("磁石が開くのにかかる秒数")]
    public float openTime = 0.5f;

    [Header("回転速度")]
    public float rotationSpeed = 3.0f;

    [Header("回路に合わせる角度")]
    public float rotationY = 0.0f;

    private GameManager gameManager;    // ゲームマネージャー
    private GameObject leftMagnet;
    private GameObject rightMagnet;
    private bool openFg = false;            // 開けるかどうかのフラグ
    private bool completeOpenFg = false;    // 開いたかどうかのフラグ
    private float timer = 0.0f;
    private Vector3 initLeftPos = Vector3.zero;     // 開く前の左側の位置
    private Vector3 initRightPos = Vector3.zero;    // 開く前の右側の位置

    // Start is called before the first frame update
    void Start()
    {
        // ゲームマネージャーを取得
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // 左右のオブジェクトを取得
        leftMagnet = transform.Find("pcS_left_rend").gameObject;
        rightMagnet = transform.Find("pcS_right_rend").gameObject;

        // 初期位置を保存
        initLeftPos = leftMagnet.transform.position;
        initRightPos = rightMagnet.transform.position;

        openFg = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        // テストでキーボードのAを押すと動く
        if (Input.GetKey(KeyCode.A))
        {
            openFg = true;
            Debug.Log(openFg);
        }
        else if (Input.GetKey(KeyCode.S)) { openFg = false; }

        //------ 回路接続時に開ける処理 ------//
        // フラグが立っている時
        if (openFg && !completeOpenFg)
        {
            if (timer <= openTime)
            {
                timer += Time.deltaTime;
                // 左を移動
                leftMagnet.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.Self);
                // 右を移動
                rightMagnet.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.Self);
            }
            else
            {
                completeOpenFg = true;
                timer = 0.0f;
            }

            // 左右を回路のある方向に移動させるために回転させる
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, rotationY, 0.0f), rotationSpeed * Time.deltaTime);
        }
        //------ 回路切断時に閉じる処理 ------//
        else if (!openFg && completeOpenFg)
        {
            if (timer <= openTime)
            {
                timer += Time.deltaTime;
                // 左を移動
                leftMagnet.transform.Translate(-Vector3.right * moveSpeed * Time.deltaTime, Space.Self);
                // 右を移動
                rightMagnet.transform.Translate(-Vector3.left * moveSpeed * Time.deltaTime, Space.Self);
            }
            else
            {
                completeOpenFg = false;
                timer = 0.0f;
            }
        }
    }

    // 開いたかどうかのゲッター
    public bool GetCompleteOpenFg()
    {
        return completeOpenFg;
    }

    // 開けるフラグを変更
    public void SetOpenFg(bool _openFg)
    {
        openFg = _openFg;
    }
}
