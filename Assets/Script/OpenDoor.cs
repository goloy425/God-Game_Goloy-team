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

    [Header("ドアが開く速度")]
    public float moveSpeed = 1.8f;

    [Header("ドアが開くのにかかる秒数")]
    public float openTime = 1.0f;

    private GameManager gameManager;    // ゲームマネージャー
    private bool openFg = false;        // 開けるかどうかのフラグ
    private float timer = 0.0f;         

    // Start is called before the first frame update
    void Start()
    {
        // ゲームマネージャーを取得

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

        // フラグが立っているかつ、開けるのにかかる秒数以内の時
        if (openFg && timer <= openTime)
        {
            timer += Time.deltaTime;
            // 左のドアを移動
            leftDoor.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.Self);
            // 右のドアを移動
            rightDoor.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.Self);
        }
    }
}
