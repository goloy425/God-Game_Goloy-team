using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCameraController : MonoBehaviour
{
    [Header("カメラで追従する対象物")]
    public Transform target1;    // 対象物１
    public Transform target2;    // 対象物２

    [Header("各ステージの中心位置（床オブジェクト）")]
    public List<Transform> floorTransform;

    [Header("各ステージのドアオブジェクト")]
    public List<GameObject> doorFlame;

    [Header("ステージ数")]
    public int stageNum;

    [Header("カメラ設定")]
    public float minSize = 5f;          // 最小サイズ
    public float maxSize = 15f;         // 最大サイズを15に制限
    public float zoomSpeed = 5f;        // 拡大速度
    public float padding = 2f;          // 対象物間の距離に余裕を持たせるための調節値
    public Vector3 initialOffset = new Vector3(0, 5, -10); // カメラの初期位置を指定し、ズームによって変化させる基本的なオフセット値
    private float startDirectionTime = 2.0f;    // 最初の演出にかける秒数

    private Camera cam;                 // メインカメラへの参照を保持
    private Vector3 dynamicOffset;      // 動的に変化するカメラオフセットを保持
    private Vector3 midPoint;

    private int curStage = 0;                   // 現在のステージ
    private int startStage;                     // 開始ステージ
    private float startMinSize = 20.0f;         // 最初にステージの全体を写してから戻す
    bool[] completeDirectionFg = { };           // ステージクリア後演出が終わったかどうか
    private float timer = 0.0f;

    private GameManager gameManager;
    private MovePlayerL movePlayerL;
    private MovePlayerR movePlayerR;
    private Rigidbody rbL;
    private Rigidbody rbR;
    private List<OpenDoor> openDoor = new List<OpenDoor>();

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;             // シーン内のメインカメラを取得
        cam.orthographic = true;       // カメラを正射影モードに設定
        dynamicOffset = initialOffset; // 初期オフセットをセット
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        movePlayerL = target1.gameObject.GetComponent<MovePlayerL>();
        movePlayerR = target2.gameObject.GetComponent<MovePlayerR>();
        rbL = target1.gameObject.GetComponent<Rigidbody>();
        rbR = target2.gameObject.GetComponent<Rigidbody>();

        // スクリプトを取得
        foreach (GameObject door in doorFlame)
        {
            openDoor.Add(door.GetComponent<OpenDoor>());
        }

        completeDirectionFg = new bool[stageNum];   // 演出完了フラグを初期化
        startMinSize = maxSize - 1.0f;              // 0除算回避のため最大サイズと同じにしない

        startStage = gameManager.GetStartStage();
        // 開始ステージが2以上で設定されている時
        if (startStage > 0)
        {
            // 入力を添え字に合わせる
            curStage = startStage - 1;
            // 開始ステージ以前のステージの演出を完了済みにする
            for (int i = 0; i < startStage - 1; i++) { completeDirectionFg[i] = true; }
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target1 == null || target2 == null) return; // 対象物が設定されていない場合、処理を終了する

        // ステージ数が存在している値の時
        if (curStage < stageNum)
        {
            // ステージ1開始時か開始ステージが設定されている時にステージ全体を写して戻す演出を行う
            if (!completeDirectionFg[0] || (startStage > 0 && !completeDirectionFg[startStage - 1]))
            {
                if ((curStage == 0 || (startStage > 0 && curStage == startStage - 1)) && timer <= startDirectionTime)
                {
                    // プレイヤーが動かないようにする
                    rbL.velocity = Vector3.zero;
                    rbR.velocity = Vector3.zero;

                    StartDirection(curStage);
                    timer += Time.deltaTime;
                }
                else if (timer > startDirectionTime)
                {
                    timer = 0.0f;
                    completeDirectionFg[curStage] = true;
                    curStage++;
                }
                // プレイヤーに追従するカメラ処理に戻す
                else
                {
                    MoveCamera();
                }
            }
            // ステージクリア後に次ステージ全体を写して戻す演出を行う
            else if (!completeDirectionFg[curStage] && gameManager.GetStageClearFg(curStage - 1) && timer <= startDirectionTime)
            {
                // プレイヤーが動かないようにする
                rbL.velocity = Vector3.zero;
                rbR.velocity = Vector3.zero;

                StartDirection(curStage);
                timer += Time.deltaTime;
            }
            // 演出時間経過後かつ、ドアが開ききった後にタイマーリセット
            else if (timer > startDirectionTime && openDoor[curStage].GetCompleteOpenFg())
            {
                timer = 0.0f;
                completeDirectionFg[curStage] = true;
                curStage++;
            }
            else
            {
                MoveCamera();
            }
        }
        else
        {
            MoveCamera();
        }
    }

    // 必要なカメラサイズを計算する補助関数
    float CalculateRequiredSize(float distance)
    {
        float aspectRatio = (float)Screen.width / Screen.height;     // 画面のアスペクト比を計算
        float requiredSizeByHeight = (distance / 2f);                // 高さ方向で必要なサイズを計算
        float requiredSizeByWidth = (distance / (2f * aspectRatio)); // 幅方向で必要なサイズを計算

        return Mathf.Max(requiredSizeByHeight, requiredSizeByWidth); // 高さ方向と幅方向のうち、より大きなサイズを返す

    }

    // スタート時にステージを全部写し、もとに戻す演出
    private void StartDirection(int _stageNum)
    {
        Vector3 targetPos = floorTransform[_stageNum].position;
        float speed = 3.0f;  // なめらかさのスピード
        midPoint = Vector3.Lerp(midPoint, targetPos, Time.deltaTime * speed);

        float distance = Vector3.Distance(target1.position, target2.position) + padding; // 対象物間の距離を計算し、余裕分を追加

        float requiredSize = CalculateRequiredSize(distance); // 必要なカメラサイズを計算
        requiredSize = Mathf.Clamp(requiredSize, startMinSize, maxSize); // 必要サイズが最小・最大サイズの範囲内に収まるように制限

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, requiredSize, Time.deltaTime * zoomSpeed); // 現在のカメラサイズから必要なサイズにスムーズに補間

        float zoomFactor = Mathf.Clamp01((cam.orthographicSize - startMinSize) / (maxSize - startMinSize)); // 現在のズームレベルを0〜1の範囲で計算
        dynamicOffset = new Vector3(initialOffset.x, initialOffset.y, Mathf.Lerp(initialOffset.z, initialOffset.z * 2f, zoomFactor)); // ズームレベルに応じてカメラの奥行き方向のオフセットを動的に調整

        transform.position = midPoint + dynamicOffset; // カメラを中間点とオフセットを考慮した位置に移動
    }

    // プレイヤーに追従するカメラ処理
    private void MoveCamera()
    {
        Vector3 targetPos = (target1.position + target2.position) / 2f;
        float speed = 3.0f;  // なめらかさのスピード
        midPoint = Vector3.Lerp(midPoint, targetPos, Time.deltaTime * speed);
        //midPoint = (target1.position + target2.position) / 2f; // 2つの対象物の中間点を計算

        float distance = Vector3.Distance(target1.position, target2.position) + padding; // 対象物間の距離を計算し、余裕分を追加

        float requiredSize = CalculateRequiredSize(distance); // 必要なカメラサイズを計算
        requiredSize = Mathf.Clamp(requiredSize, minSize, maxSize); // 必要サイズが最小・最大サイズの範囲内に収まるように制限

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, requiredSize, Time.deltaTime * zoomSpeed); // 現在のカメラサイズから必要なサイズにスムーズに補間

        float zoomFactor = Mathf.Clamp01((cam.orthographicSize - minSize) / (maxSize - minSize)); // 現在のズームレベルを0〜1の範囲で計算
        dynamicOffset = new Vector3(initialOffset.x, initialOffset.y, Mathf.Lerp(initialOffset.z, initialOffset.z * 2f, zoomFactor)); // ズームレベルに応じてカメラの奥行き方向のオフセットを動的に調整

        transform.position = midPoint + dynamicOffset; // カメラを中間点とオフセットを考慮した位置に移動
    }
}
