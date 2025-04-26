using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

// 制作者　ゴロイヒデキ   本田洸都


// 外部から設定可能なステージのデータクラス
[System.Serializable]
public class StageData
{
    public List<GameObject> magObjSphere = new List<GameObject>();              // 球体の磁力オブジェクト
    public List<GameObject> magObjSplit1 = new List<GameObject>();              // 分裂物体の左側の磁力オブジェクト
    public List<GameObject> magObjSplit2 = new List<GameObject>();              // 分裂物体の右側の磁力オブジェクト
    public List<GameObject> magObjConnecter = new List<GameObject>();           // 分裂物体を接続する磁力オブジェクト
    public List<DetectArea> detectAreas = new List<DetectArea>();               // クリア判定オブジェクト

    private List<SphereMagnetism> sphereMagCS = new List<SphereMagnetism>();    // 球体の磁力スクリプト
    private List<HCubeMagnetism> split1HCubeMagCS = new List<HCubeMagnetism>(); // 分裂物体の左側の磁力スクリプト
    private List<HCubeMagnetism> split2HCubeMagCS = new List<HCubeMagnetism>(); // 分裂物体の右側の磁力スクリプト
    private List<CubeMagnetism> cubeMagCS = new List<CubeMagnetism>();          // コネクターの磁力スクリプト
                                                
    private List<MoveSphere> moveSphereCS = new List<MoveSphere>();             // 球体の動作スクリプト
    private List<MoveHCubeL> moveHCubeLCS = new List<MoveHCubeL>();             // 分裂物体の左側の動作スクリプト
    private List<MoveHCubeR> moveHCubeRCS = new List<MoveHCubeR>();             // 分裂物体の右側の動作スクリプト
                                                
    private bool clearFg = false;               // クリアフラグ

    // クリアフラグのゲッター
    public bool GetClearFg() { return clearFg; }

    // クリアフラグのセッター                   
    public void SetClearFg(bool _clearFg) { clearFg = _clearFg; }   
    
    // 球体の磁力スクリプトリストのゲッター
    public List<SphereMagnetism> GetSphereMagCS() { return sphereMagCS; }   

    // 分裂物体の左側の磁力スクリプトリストのゲッター
    public List<HCubeMagnetism> GetSplit1HCubeMagCS() { return split1HCubeMagCS; }  
    
    // 分裂物体の右側の磁力スクリプトリストのゲッター
    public List<HCubeMagnetism> GetSplit2HCubeMagCS() { return split2HCubeMagCS; }
    
    // コネクターの磁力スクリプトリストのゲッター
    public List<CubeMagnetism> GetCubeMagCS() { return cubeMagCS; }

    // 球体の動作スクリプトリストのゲッター
    public List<MoveSphere> GetMoveSphereCS() { return moveSphereCS; }

    // 分裂物体の左側の動作スクリプトリストのゲッター
    public List<MoveHCubeL> GetMoveHCubeLCS() { return moveHCubeLCS; }

    // 分裂物体の右側の動作スクリプトリストのゲッター
    public List<MoveHCubeR> GetMoveHCubeRCS() { return moveHCubeRCS; }      
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

    private float clearTimer = 0.0f;          // 接続され続けている秒数
    private float clearTime = 4.0f;           // 接続されている秒数がこの秒数を超えるとクリアとみなす
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

        //// ステージ1の各磁力スクリプトを有効化
        //ActiveMagObjects(0);

        Debug.Log("ステージ数 : " + stageDatas.Count);
        // 全ステージの移動できる磁力オブジェクトの探索
        for (int i = 0; i < stageDatas.Count; i++)
        {
            Debug.Log("ステージ" + (i + 1) + "初期化");
            SearchCanCarryMagObj(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        // ----- 移動できる磁力オブジェクトの探索 ----- //
        SearchCanCarryMagObj(curStage);
        //for (int i = 0; i < stageDatas.Count; i++)
        //{
        //    SearchCanCarryMagObj(i);
        //}

        // ----- ステージのクリア処理 ----- //
        int connectCount = 0;  // 繋がっている判定エリア

        // 現在のステージの判定エリアが繋がっているかを調べる
        for (int i = 0; i < stageDatas[curStage].detectAreas.Count; i++)
        {
            // 繋がっている時、その数をカウント
            if (stageDatas[curStage].detectAreas[i].GetIsConnectFg())
            {
                connectCount++;

                // すべての判定エリアが繋がっている時、秒数によってクリアフラグを変更
                if (connectCount == stageDatas[curStage].detectAreas.Count)
                {
                    clearTimer += Time.deltaTime;   // 繋がっている秒数を計測

                    // クリアとみなす秒数を超えたらクリア
                    if (clearTimer > clearTime) 
                    {
                        stageDatas[curStage].SetClearFg(true);
                        clearTimer = 0.0f;   // タイマーリセット
                    }   
                }
            }
        }

        // 現在のステージをクリアした時、次のステージがあれば次のステージに進む
        if (stageDatas[curStage].GetClearFg() && curStage + 1 < stageDatas.Count)
        {
            stageDatas[curStage].SetClearFg(false);   // 複数回更新しないようにフラグを折る
            curStage++;
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

    // 判定エリアオブジェクトの状態を検知
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
                    stageDatas[i].GetSplit1HCubeMagCS().Add(magObj.GetComponent<HCubeMagnetism>());
                    stageDatas[i].GetMoveHCubeLCS().Add(magObj.GetComponent<MoveHCubeL>());
                }
            }

            // 分裂物体の右側の磁力オブジェクトの各スクリプトをリストに追加
            foreach (GameObject magObj in stageDatas[i].magObjSplit2)
            {
                if (magObj != null)
                {
                    stageDatas[i].GetSplit2HCubeMagCS().Add(magObj.GetComponent<HCubeMagnetism>());
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
                stageDatas[i].GetSplit1HCubeMagCS().Clear();
                stageDatas[i].GetMoveHCubeLCS().Clear();
            }

            // 分裂物体の右側の磁力オブジェクトの各スクリプトをリストから削除
            if (stageDatas[i].magObjSplit2 != null)
            {
                stageDatas[i].GetSplit2HCubeMagCS().Clear();
                stageDatas[i].GetMoveHCubeRCS().Clear();
            }

            // 分裂物体を接続する磁力オブジェクトの各スクリプトをリストから削除
            if (stageDatas[i].magObjConnecter != null)
            {
                stageDatas[i].GetCubeMagCS().Clear();
            }
        }
    }

    // 指定したステージの移動できる磁力オブジェクトを探索し、スクリプトの有効状態を変更
    private void SearchCanCarryMagObj(int _index)
    {
        // 球体の磁力オブジェクト
        //foreach (GameObject magObj in stageDatas[_index].magObjSphere)
        for(int i = 0; i < stageDatas[_index].magObjSphere.Count; i++)
        {
            GameObject magObj = stageDatas[_index].magObjSphere[i];

            if (magObj != null)
            {
                // プレイヤーLの磁力範囲に入っている磁力オブジェクトのスクリプトのみ有効化
                if (GetDistancePlayerMagToMagObj(_index, 1, i, magObj) < stageDatas[_index].GetSphereMagCS()[i].GetMagnetismRange())
                {
                    Debug.Log(_index + 1 + ": Sphere 有効化");
                    stageDatas[_index].GetSphereMagCS()[i].enabled = true;
                    stageDatas[_index].GetMoveSphereCS()[i].enabled = true;
                }
                // 入っていない時は無効化
                else if (!magnetism1.inObjMagArea)
                {
                    Debug.Log(_index + 1 + ": Sphere 無効化");
                    stageDatas[_index].GetSphereMagCS()[i].enabled = false;
                    stageDatas[_index].GetMoveSphereCS()[i].enabled = false;
                }

                // プレイヤーRの磁力範囲に入っている磁力オブジェクトのスクリプトのみ有効化
                if (GetDistancePlayerMagToMagObj(_index, 2, i, magObj) < stageDatas[_index].GetSphereMagCS()[i].GetMagnetismRange())
                {
                    Debug.Log(_index + 1 + ": Sphere 有効化");
                    stageDatas[_index].GetSphereMagCS()[i].enabled = true;
                    stageDatas[_index].GetMoveSphereCS()[i].enabled = true;
                }
                // 入っていない時は無効化
                else if (!magnetism2.inObjMagArea)
                {
                    Debug.Log(_index + 1 + ": Sphere 無効化");
                    stageDatas[_index].GetSphereMagCS()[i].enabled = false;
                    stageDatas[_index].GetMoveSphereCS()[i].enabled = false;
                }
            }
        }

        // 分裂物体の左側の磁力オブジェクト
        //foreach (GameObject magObj in stageDatas[_index].magObjSplit1)
        for (int i = 0; i < stageDatas[_index].magObjSplit1.Count; i++)
        {
            GameObject magObj = stageDatas[_index].magObjSplit1[i];
            GameObject connecter = stageDatas[_index].magObjConnecter[i];

            if (magObj != null)
            {
                // 分割後でプレイヤーLの磁力範囲に入っている磁力オブジェクトのスクリプトのみ有効化
                if (GetDistancePlayerMagToMagObj(_index, 1, i, magObj) < stageDatas[_index].GetSplit1HCubeMagCS()[i].GetMagnetismRange() &&
                    !connecter.activeSelf)
                {
                    Debug.Log(_index + 1 + ": Split1 有効化");
                    stageDatas[_index].GetSplit1HCubeMagCS()[i].enabled = true;
                    stageDatas[_index].GetMoveHCubeLCS()[i].enabled = true;
                }
                // 分割前または、入っていない時は無効化
                else if (!magnetism1.inObjMagArea || connecter.activeSelf)
                {
                    Debug.Log(_index + 1 + ": Split1 無効化");
                    stageDatas[_index].GetSplit1HCubeMagCS()[i].enabled = false;
                    stageDatas[_index].GetMoveHCubeLCS()[i].enabled = false;
                }

                // 分割後でプレイヤーRの磁力範囲に入っている磁力オブジェクトのスクリプトのみ有効化
                if (GetDistancePlayerMagToMagObj(_index, 2, i, magObj) < stageDatas[_index].GetSplit1HCubeMagCS()[i].GetMagnetismRange() &&
                    !connecter.activeSelf)
                {
                    Debug.Log(_index + 1 + ": Split1 有効化");
                    stageDatas[_index].GetSplit1HCubeMagCS()[i].enabled = true;
                    stageDatas[_index].GetMoveHCubeLCS()[i].enabled = true;
                }
                // 分割前または、入っていない時は無効化
                else if (!magnetism2.inObjMagArea || connecter.activeSelf)
                {
                    Debug.Log(_index + 1 + ": Split1 無効化");
                    stageDatas[_index].GetSplit1HCubeMagCS()[i].enabled = false;
                    stageDatas[_index].GetMoveHCubeLCS()[i].enabled = false;
                }
            }
        }

        // 分裂物体の右側の磁力オブジェクト
        //foreach (GameObject magObj in stageDatas[_index].magObjSplit2)
        for (int i = 0; i < stageDatas[_index].magObjSplit2.Count; i++)
        {
            GameObject magObj = stageDatas[_index].magObjSplit2[i];
            GameObject connecter = stageDatas[_index].magObjConnecter[i];

            if (magObj != null)
            {
                // 分割後でプレイヤーLの磁力範囲に入っている磁力オブジェクトのスクリプトのみ有効化
                if (GetDistancePlayerMagToMagObj(_index, 1, i, magObj) < stageDatas[_index].GetSplit2HCubeMagCS()[i].GetMagnetismRange() &&
                    !connecter.activeSelf)
                {
                    Debug.Log(_index + 1 + ": Split2 有効化");
                    stageDatas[_index].GetSplit2HCubeMagCS()[i].enabled = true;
                    stageDatas[_index].GetMoveHCubeRCS()[i].enabled = true;
                }
                // 分割前または、入っていない時は無効化
                else if (!magnetism1.inObjMagArea || connecter.activeSelf)
                {
                    Debug.Log(_index + 1 + ": Split2 無効化");
                    stageDatas[_index].GetSplit2HCubeMagCS()[i].enabled = false;
                    stageDatas[_index].GetMoveHCubeRCS()[i].enabled = false;
                }

                // 分割後でプレイヤーRの磁力範囲に入っている磁力オブジェクトのスクリプトのみ有効化
                if (GetDistancePlayerMagToMagObj(_index, 2, i, magObj) < stageDatas[_index].GetSplit2HCubeMagCS()[i].GetMagnetismRange() &&
                    !connecter.activeSelf)
                {
                    Debug.Log(_index + 1 + ": Split2 有効化");
                    stageDatas[_index].GetSplit2HCubeMagCS()[i].enabled = true;
                    stageDatas[_index].GetMoveHCubeRCS()[i].enabled = true;
                }
                // 分割前または、入っていない時は無効化
                else if (!magnetism2.inObjMagArea || connecter.activeSelf)
                {
                    Debug.Log(_index + 1 + ": Split2 無効化");
                    stageDatas[_index].GetSplit2HCubeMagCS()[i].enabled = false;
                    stageDatas[_index].GetMoveHCubeRCS()[i].enabled = false;
                }
            }
        }

        // 分裂物体を接続する磁力オブジェクト
        //foreach (GameObject magObj in stageDatas[_index].magObjConnecter)
        for (int i = 0; i < stageDatas[_index].magObjConnecter.Count; i++)
        {
            GameObject magObj = stageDatas[_index].magObjConnecter[i];

            if (magObj != null)
            {
                // プレイヤーLの磁力範囲に入っている磁力オブジェクトのスクリプトのみ有効化
                if (GetDistancePlayerMagToMagObj(_index, 1, i, magObj) < stageDatas[_index].GetCubeMagCS()[i].GetMagnetismRange())
                {
                    Debug.Log(_index + 1 + ": Connecter 有効化");
                    stageDatas[_index].GetCubeMagCS()[i].enabled = true;
                }
                // 入っていない時は無効化
                else if (!magnetism1.inObjMagArea)
                {
                    Debug.Log(_index + 1 + ": Connecter 無効化");
                    stageDatas[_index].GetCubeMagCS()[i].enabled = false;
                }

                // プレイヤーRの磁力範囲に入っている磁力オブジェクトのスクリプトのみ有効化
                if (GetDistancePlayerMagToMagObj(_index, 2, i, magObj) < stageDatas[_index].GetCubeMagCS()[i].GetMagnetismRange())
                {
                    Debug.Log(_index + 1 + ": Connecter 有効化");
                    stageDatas[_index].GetCubeMagCS()[i].enabled = true;
                }
                // 入っていない時は無効化
                else if (!magnetism2.inObjMagArea)
                {
                    Debug.Log(_index + 1 + ": Connecter 無効化");
                    stageDatas[_index].GetCubeMagCS()[i].enabled = false;
                }
            }
        }
    }

    // プレイヤーの磁石から磁力オブジェクトまでの距離を取得
    private float GetDistancePlayerMagToMagObj(int _index, int _playerMagNumber, int _magObjNumber, GameObject _magObj)
    {
        Vector3 playerMagPos = (_playerMagNumber == 1) ? magnetism1.myPlate.transform.position : magnetism2.myPlate.transform.position;   // プレイヤーの磁石の位置
        Vector3 magObjPos = _magObj.transform.position;     // 磁力オブジェクトの位置
        //float playerMagArea = (_playerMagNumber == 1) ? magnetism1.magnetismRange * 1.1f : magnetism2.magnetismRange * 1.1f;              // プレイヤーの磁石の磁力範囲

        // 種類別で磁力範囲の距離を取得（各Magnetism.csと同じ処理で距離を求める）
        //float magObjArea = 0.0f;
        float surfaceDistance = 0.0f;
        switch (_magObj.name)
        {
            case "MagObj_Sphere":
                // 球の表面にくっつけるために球の半径を計算
                Vector3 surfacePoint = stageDatas[_index].GetSphereMagCS()[_magObjNumber].GetSphereCollider().ClosestPoint(playerMagPos);
                // 球の表面と磁石の距離を計算
                surfaceDistance = Vector3.Distance(surfacePoint, playerMagPos);
                //magObjArea = stageDatas[_index].GetSphereMagCS()[_magObjNumber].GetMagnetismRange() * 1.2f;     // 球体の磁力オブジェクトの磁力範囲
                break;
            case "MagObj_split1":
                surfacePoint = stageDatas[_index].GetSplit1HCubeMagCS()[_magObjNumber].GetHCubeCollider().ClosestPoint(playerMagPos);
                surfaceDistance = Vector3.Distance(surfacePoint, playerMagPos);
                //magObjArea = stageDatas[_index].GetSplit1HCubeMagCS()[_magObjNumber].GetMagnetismRange() * 2.0f;      // 分裂物体の左側の磁力オブジェクトの磁力範囲
                break;
            case "MagObj_split2":
                surfacePoint = stageDatas[_index].GetSplit2HCubeMagCS()[_magObjNumber].GetHCubeCollider().ClosestPoint(playerMagPos);
                surfaceDistance = Vector3.Distance(surfacePoint, playerMagPos);
                //magObjArea = stageDatas[_index].GetSplit2HCubeMagCS()[_magObjNumber].GetMagnetismRange() * 2.0f;      // 分裂物体の右側の磁力オブジェクトの磁力範囲
                break;
            case "Connecter":
                // Colliderを利用して一番近い表面の座標を取得
                Vector3 surface1 = stageDatas[_index].GetCubeMagCS()[_magObjNumber].GetCube1Collider().ClosestPoint(playerMagPos);
                Vector3 surface2 = stageDatas[_index].GetCubeMagCS()[_magObjNumber].GetCube2Collider().ClosestPoint(playerMagPos);

                // 表面座標との距離を計算
                float distance1 = Vector3.Distance(surface1, playerMagPos);
                float distance2 = Vector3.Distance(surface2, playerMagPos);

                Vector3 targetSurface = (distance1 < distance2) ? surface1 : surface2;
                surfaceDistance = Mathf.Min(distance1, distance2);
                //magObjArea = stageDatas[_index].GetCubeMagCS()[_magObjNumber].GetMagnetismRange() * 2.0f;       // 分裂物体を接続する磁力オブジェクトの磁力範囲
                break;
        }

        //float centerDistance = Vector3.Distance(playerMagPos, magObjPos);   // 中心位置の距離

        //float magnetismAreaDistance = Mathf.Max(0.0f, centerDistance - (playerMagArea + magObjArea));   // 磁力範囲の距離
        return surfaceDistance;
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
