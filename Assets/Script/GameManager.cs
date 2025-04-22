using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 制作者　ゴロイヒデキ   本田洸都

// 外部から設定可能なステージのデータ
[System.Serializable]
public class StageData
{
    public List<GameObject> magObjects;     // 磁力オブジェクト
    public List<DetectArea> detectAreas;    // クリア判定オブジェクト
    public bool clearFg = false;            // クリアフラグ
}

public class GameManager : MonoBehaviour
{
    [Header("各ステージの磁力オブジェクト（最初にステージ数を設定）")]
    public List<StageData> stageDatas;

    [Header("プレイヤーの磁石")]
    public GameObject magnet1;
    public GameObject magnet2;

    public DetectArea[] detectAreas;    // すべての判定エリアを登録
    private int totalConnected = 0;     // 接続された判定エリアの数

    public string resultSceneName = "Result";       // 遷移先のシーン名をInspectorで設定
    public string gameOverSceneName = "GameOver";   // 遷移先のシーン名をInspectorで設定

    private bool gameClearFg = false;         // ゲームクリアしたかどうか
    private bool gameOverFg = false;          // ゲームオーバーしたかどうか

    private float changeSceneTime = 1.0f;     // 何秒後にリザルトシーンに遷移するか

    private Magnetism magnetism1 = null;      // プレイヤーLのマグネティズム
    private Magnetism magnetism2 = null;      // プレイヤーRのマグネティズム

    private int curStage = 0;                 // 現在のステージ数

    public PressurePlates01[] pressurePlates; // すべての感圧板を登録
    private int totalPressed = 0; // 押されている感圧板の数


    // Start is called before the first frame update
    void Start()
    {
        foreach (PressurePlates01 plate in pressurePlates)
        {
            plate.OnPressurePlateChanged += OnPlateStateChanged; // イベント登録
        }

        foreach (DetectArea detectArea in detectAreas)
        {
            detectArea.OnDetectAreaChanged += OnDetectionStateChanged; // イベント登録
        }

        // 磁力スクリプトを取得
        magnetism1 = magnet1.GetComponent<Magnetism>();
        magnetism2 = magnet2.GetComponent<Magnetism>();

        // ステージ1の磁力オブジェクトを有効化
        ActiveMagObjects(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        int connectCount = 0;  // 繋がっている判定エリア

        // 現在のステージの判定エリアが繋がっているかを調べる
        for (int i = 0; i < stageDatas[curStage].detectAreas.Count; i++)
        {
            // 繋がっている時、その数をカウント
            if (stageDatas[curStage].detectAreas[i].GetIsConnectFg())
            {
                connectCount++;

                // すべての判定エリアが繋がっている時、クリアフラグを立てる
                if (connectCount == stageDatas[curStage].detectAreas.Count)
                {
                    stageDatas[curStage].clearFg = true;
                }
            }
        }

        // 現在のステージをクリアした時、次のステージに進む
        if (stageDatas[curStage].clearFg )
        {
            curStage++;
            ActiveMagObjects(curStage); // 次のステージの磁力オブジェクトを有効化
            stageDatas[curStage - 1].clearFg = false;   // 複数回更新しないようにフラグを折る
        }

        // プレイヤーの磁石に何かがくっついた時または、磁力範囲外に出た時
        if ((magnetism1.isSnapping || magnetism2.isSnapping) || 
            (!magnetism1.inMagnetismArea || !magnetism2.inMagnetismArea))
        {
            Debug.Log("磁石がくっつきました！ゲームオーバー！Result画面に移ります");
            // ここにゲームオーバー処理を書く

            // changeSceneTime秒後にゲームオーバーシーンに遷移
            Invoke("MoveGameOverScene", changeSceneTime);    
        }
    }

    void OnPlateStateChanged(bool isPressed)
    {
        totalPressed += isPressed ? 1 : -1; // 押されている数を増減

        if (totalPressed == pressurePlates.Length) // すべてが押された場合
        {
            gameClearFg = true; // ゲームクリア
            Debug.Log("全ての感圧板が押されています！ゲームクリア！Result画面に移ります");
            // ここにゲームクリア処理を書く

            // changeSceneTime秒後にリザルトシーンに遷移
            Invoke("MoveResultScene", changeSceneTime);
        }
    }

    void OnDetectionStateChanged(bool isConnected)
    {
        totalConnected += isConnected ? 1 : -1; // 接続されている数を増減
        Debug.Log(totalConnected);

        if (totalConnected == detectAreas.Length)
        {
            gameClearFg = true; // ゲームクリア
            Debug.Log("全ての回路が接続されています！ゲームクリア！Result画面に移ります");
            // ここにゲームクリア処理を書く

            // changeSceneTime秒後にリザルトシーンに遷移
            Invoke("MoveResultScene", changeSceneTime);
        }
    }

    // 指定したステージの磁力オブジェクトを有効化
    private void ActiveMagObjects(int _stageIndex)
    {
        // 全ステージの磁力オブジェクトを無効化
        for (int i = 0; i < stageDatas.Count; i++)
        {
            foreach (GameObject obj in stageDatas[i].magObjects)
            {
                if (obj != null) { obj.SetActive(false); }
            }
        }

        // 指定されたステージのオブジェクトを有効化
        if(_stageIndex >= 0 &&  _stageIndex < stageDatas.Count)
        {
            foreach (GameObject obj in stageDatas[_stageIndex].magObjects)
            {
                if (obj != null) { obj.SetActive(true); }
            }
        }
    }


    // リザルトシーンに遷移
    private void MoveResultScene()
    {
        SceneManager.LoadScene(resultSceneName);
    }

    // ゲームオーバーシーンに遷移
    private void MoveGameOverScene()
    {
        SceneManager.LoadScene(gameOverSceneName);
    }

    // ゲームクリアフラグを取得
    public bool GetGameClearFg()
    {
        return gameClearFg;
    }

    // ゲームオーバーフラグを取得
    public bool GetGameOverFg()
    {
        return gameOverFg;
    }

    // ゲームクリアフラグをセット
    public void SetGameClearFg(bool _gameClearFg)
    {
        gameClearFg = _gameClearFg;
    }

    // ゲームオーバーフラグをセット
    public void SetGameOverFg(bool _gameOverFg)
    {
        gameOverFg = _gameOverFg;
    }
}
