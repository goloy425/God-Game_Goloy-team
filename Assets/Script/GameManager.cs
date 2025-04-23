using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 制作者　ゴロイヒデキ   本田洸都


// 外部から設定可能なステージのデータクラス
[System.Serializable]
public class StageData
{
    public List<GameObject> magObjSphere;       // 球体の磁力オブジェクト
    public List<GameObject> magObjSplit1;       // 分裂物体の左側の磁力オブジェクト
    public List<GameObject> magObjSplit2;       // 分裂物体の右側の磁力オブジェクト
    public List<GameObject> magObjConnecter;    // 分裂物体を接続する磁力オブジェクト
    public List<DetectArea> detectAreas;        // クリア判定オブジェクト

    private List<SphereMagnetism> sphereMagCS;  // 球体の磁力スクリプト
    private List<HCubeMagnetism> hCubeMagCS;    // 分裂物体の磁力スクリプト
    private List<CubeMagnetism> cubeMagCS;      // コネクターの磁力スクリプト
                                                
    private List<MoveSphere> moveSphereCS;      // 球体の動作スクリプト
    private List<MoveHCubeL> moveHCubeLCS;      // 分裂物体の左側の動作スクリプト
    private List<MoveHCubeR> moveHCubeRCS;      // 分裂物体の右側の動作スクリプト
                                                
    private bool clearFg = false;               // クリアフラグ


    public bool GetClearFg() { return clearFg; }                    // クリアフラグのゲッター
    public void SetClearFg(bool _clearFg) { clearFg = _clearFg; }   // クリアフラグのセッター
    public List<SphereMagnetism> GetSphereMagCS() { return sphereMagCS; }   // 球体の磁力スクリプトリストのゲッター
    public List<HCubeMagnetism> GetHCubeMagCS() { return hCubeMagCS; }      // 球体の磁力スクリプトリストのゲッター
    public List<CubeMagnetism> GetCubeMagCS() { return cubeMagCS; }         // 球体の磁力スクリプトリストのゲッター
    public List<MoveSphere> GetMoveSphereCS() { return moveSphereCS; }      // 球体の磁力スクリプトリストのゲッター
    public List<MoveHCubeL> GetMoveHCubeLCS() { return moveHCubeLCS; }      // 球体の磁力スクリプトリストのゲッター
    public List<MoveHCubeR> GetMoveHCubeRCS() { return moveHCubeRCS; }      // 球体の磁力スクリプトリストのゲッター
}


public class GameManager : MonoBehaviour
{
    [Header("各ステージの磁力オブジェクト（最初にステージ数を設定）")]
    public List<StageData> stageDatas;

    [Header("プレイヤーの磁石")]
    public GameObject magnet1;
    public GameObject magnet2;

    [Header("各ステージのクリア判定オブジェクト")]
    public DetectArea[] detectAreas;    // すべての判定エリアを登録
    private int totalConnected = 0;     // 接続された判定エリアの数


    [Header("遷移先のシーン名")]
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

        // プレイヤーの磁石の磁力スクリプトを取得
        magnetism1 = magnet1.GetComponent<Magnetism>();
        magnetism2 = magnet2.GetComponent<Magnetism>();

        // 設定された全ての磁力オブジェクトにアタッチされた各スクリプトをリストに追加
        AddAllMagCSList();

        // ステージ1の各磁力スクリプトを有効化
        ActiveMagObjects(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        // ----- ステージのクリア処理 ----- //
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
                    stageDatas[curStage].SetClearFg(true);
                }
            }
        }

        // 現在のステージをクリアした時、次のステージがあれば次のステージに進む
        if (stageDatas[curStage].GetClearFg() && curStage + 1 < stageDatas.Count)
        {
            curStage++;
            ActiveMagObjects(curStage); // 次のステージの磁力オブジェクトを有効化
            stageDatas[curStage - 1].SetClearFg(false);   // 複数回更新しないようにフラグを折る
        }


        // ----- ゲームオーバーの判定処理 ----- //
        // プレイヤーの磁石に何かがくっついた時
        if (magnetism1.isSnapping || magnetism2.isSnapping)
        {
            Debug.Log("磁石がくっつきました！ゲームオーバー！Result画面に移ります");
            // ここにゲームオーバー処理を書く

            // changeSceneTime秒後にゲームオーバーシーンに遷移
            Invoke("MoveGameOverScene", changeSceneTime);    
        }

        //// オブジェクトの磁力範囲外に出た時
        //if (!magnetism1.inObjMagArea || !magnetism2.inObjMagArea)
        //{
        //    Debug.Log("オブジェクトの磁力範囲外に出ました！ゲームオーバー！Result画面に移ります");
        //    // ここにゲームオーバー処理を書く

        //    // changeSceneTime秒後にゲームオーバーシーンに遷移
        //    Invoke("MoveGameOverScene", changeSceneTime);
        //}

        //// プレイヤーの磁力範囲外に出た時
        //if (!magnetism1.inPlayerMagArea || !magnetism2.inPlayerMagArea)
        //{
        //    Debug.Log("プレイヤーの磁力範囲外に出ました！ゲームオーバー！Result画面に移ります");
        //    // ここにゲームオーバー処理を書く

        //    // changeSceneTime秒後にゲームオーバーシーンに遷移
        //    Invoke("MoveGameOverScene", changeSceneTime);
        //}

        // 磁力範囲外に出た時
        if (!magnetism1.inMagnetismArea || !magnetism2.inMagnetismArea)
        {
            Debug.Log("磁力範囲外に出ました！ゲームオーバー！Result画面に移ります");
            // ここにゲームオーバー処理を書く

            // changeSceneTime秒後にゲームオーバーシーンに遷移
            Invoke("MoveGameOverScene", changeSceneTime);
        }
    }

    // GameManagerが削除された時にリスト内の要素をすべて削除する
    private void OnDestroy()
    {
        //DeleteAllMagCSList();
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


    // 設定された磁力オブジェクトにアタッチされている各スクリプトをリストに格納する
    private void AddAllMagCSList()
    {
        for (int i = 0; i < stageDatas.Count; i++)
        {
            // 球体の磁力オブジェクトの各スクリプトをリストに追加
            foreach (GameObject magObj in stageDatas[i].magObjSphere)
            {
                if (magObj != null)
                {
                    stageDatas[i].GetSphereMagCS().Add(magObj.GetComponent<SphereMagnetism>());
                    stageDatas[i].GetMoveSphereCS().Add(magObj.GetComponent<MoveSphere>());
                }
            }

            // 分裂物体の左側の磁力オブジェクトの各スクリプトをリストに追加
            foreach (GameObject magObj in stageDatas[i].magObjSplit1)
            {
                if (magObj != null)
                {
                    stageDatas[i].GetHCubeMagCS().Add(magObj.GetComponent<HCubeMagnetism>());
                    stageDatas[i].GetMoveHCubeLCS().Add(magObj.GetComponent<MoveHCubeL>());
                }
            }

            // 分裂物体の右側の磁力オブジェクトの各スクリプトをリストに追加
            foreach (GameObject magObj in stageDatas[i].magObjSplit2)
            {
                if (magObj != null)
                {
                    stageDatas[i].GetHCubeMagCS().Add(magObj.GetComponent<HCubeMagnetism>());
                    stageDatas[i].GetMoveHCubeRCS().Add(magObj.GetComponent<MoveHCubeR>());
                }
            }

            // 分裂物体を接続する磁力オブジェクトの各スクリプトをリストに追加
            foreach (GameObject magObj in stageDatas[i].magObjConnecter)
            {
                if (magObj != null)
                {
                    stageDatas[i].GetCubeMagCS().Add(magObj.GetComponent<CubeMagnetism>());
                }
            }
        }
    }

    // リストに格納されている各スクリプトを全て削除する
    private void DeleteAllMagCSList()
    {
        for (int i = 0; i < stageDatas.Count; i++)
        {
            // 球体の磁力オブジェクトの各スクリプトをリストから削除
            if (stageDatas[i].magObjSphere != null)
            {
                stageDatas[i].GetSphereMagCS().Clear();
                stageDatas[i].GetMoveSphereCS().Clear();
            }

            // 分裂物体の左側の磁力オブジェクトの各スクリプトをリストから削除
            if (stageDatas[i].magObjSplit1 != null)
            {
                stageDatas[i].GetHCubeMagCS().Clear();
                stageDatas[i].GetMoveHCubeLCS().Clear();
            }

            // 分裂物体の右側の磁力オブジェクトの各スクリプトをリストから削除
            if (stageDatas[i].magObjSplit2 != null)
            {
                stageDatas[i].GetHCubeMagCS().Clear();
                stageDatas[i].GetMoveHCubeRCS().Clear();
            }

            // 分裂物体を接続する磁力オブジェクトの各スクリプトをリストから削除
            if (stageDatas[i].magObjConnecter != null)
            {
                stageDatas[i].GetCubeMagCS().Clear();
            }
        }
    }

    // 指定したステージの磁力オブジェクトを有効化
    private void ActiveMagObjects(int _stageIndex)
    {
        // 全ステージの各スクリプトを無効化
        for (int i = 0; i < stageDatas.Count; i++)
        {
            //foreach (GameObject obj in stageDatas[i].magObjects)
            //{
            //    if (obj != null) { obj.SetActive(false); }
            //}

            // 球体の磁力オブジェクト
            if (stageDatas[i].magObjSphere != null)
            {
                foreach (SphereMagnetism sphereMag in stageDatas[i].GetSphereMagCS()) { sphereMag.enabled = false; }
                foreach (MoveSphere moveSphere in stageDatas[i].GetMoveSphereCS()) { moveSphere.enabled = false; }
            }

            // 分裂物体の左側の磁力オブジェクト
            if (stageDatas[i].magObjSplit1 != null)
            {
                foreach (HCubeMagnetism hCubeMag in stageDatas[i].GetHCubeMagCS()) { hCubeMag.enabled = false; }
                foreach (MoveHCubeL moveHCubeL in stageDatas[i].GetMoveHCubeLCS()) { moveHCubeL.enabled = false; }
            }

            // 分裂物体の右側の磁力オブジェクト
            if (stageDatas[i].magObjSplit2 != null)
            {
                foreach (HCubeMagnetism hCubeMag in stageDatas[i].GetHCubeMagCS()) { hCubeMag.enabled = false; }
                foreach (MoveHCubeR moveHCubeR in stageDatas[i].GetMoveHCubeRCS()) { moveHCubeR.enabled = false; }
            }

            // 分裂物体を接続する磁力オブジェクト
            if (stageDatas[i].magObjConnecter != null)
            {
                foreach (CubeMagnetism CubeMag in stageDatas[i].GetCubeMagCS()) { CubeMag.enabled = false; }
            }
        }

        // 指定されたステージの各スクリプトを有効化
        if (_stageIndex >= 0 &&  _stageIndex < stageDatas.Count)
        {
            //foreach (GameObject obj in stageDatas[_stageIndex].magObjects)
            //{
            //    if (obj != null) { obj.SetActive(true); }
            //}

            // 球体の磁力オブジェクト
            if (stageDatas[_stageIndex].magObjSphere != null)
            {
                foreach (SphereMagnetism sphereMag in stageDatas[_stageIndex].GetSphereMagCS()) { sphereMag.enabled = true; }
                foreach (MoveSphere moveSphere in stageDatas[_stageIndex].GetMoveSphereCS()) { moveSphere.enabled = true; }
            }

            // 分裂物体の左側の磁力オブジェクト
            if (stageDatas[_stageIndex].magObjSplit1 != null)
            {
                foreach (HCubeMagnetism hCubeMag in stageDatas[_stageIndex].GetHCubeMagCS()) { hCubeMag.enabled = true; }
                foreach (MoveHCubeL moveHCubeL in stageDatas[_stageIndex].GetMoveHCubeLCS()) { moveHCubeL.enabled = true; }
            }

            // 分裂物体の右側の磁力オブジェクト
            if (stageDatas[_stageIndex].magObjSplit2 != null)
            {
                foreach (HCubeMagnetism hCubeMag in stageDatas[_stageIndex].GetHCubeMagCS()) { hCubeMag.enabled = true; }
                foreach (MoveHCubeR moveHCubeR in stageDatas[_stageIndex].GetMoveHCubeRCS()) { moveHCubeR.enabled = true; }
            }

            // 分裂物体を接続する磁力オブジェクト
            if (stageDatas[_stageIndex].magObjConnecter != null)
            {
                foreach (CubeMagnetism CubeMag in stageDatas[_stageIndex].GetCubeMagCS()) { CubeMag.enabled = true; }
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
