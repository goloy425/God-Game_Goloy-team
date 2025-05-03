//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class StageManager : MonoBehaviour
//{
//    public int currentStage = 2; // 現在のステージ番号を管理する公開変数

//    // 次のステージに進む
//    public void NextStage()
//    {
//        currentStage++; // ステージ番号を更新
//        Debug.Log("現在のステージ番号: " + currentStage);

//        string nextSceneName = "Stage" + currentStage; // 次のシーン名を生成
//        if (Application.CanStreamedLevelBeLoaded(nextSceneName)) // シーンが存在するか確認
//        {
//            PlayerPrefs.SetInt("CurrentStage", currentStage); // ステージ番号を保存
//            SceneManager.LoadScene(nextSceneName); // 次のシーンを読み込む

//        }
//        else
//        {
//            Debug.LogWarning("次のシーンが見つかりません: " + nextSceneName);
//        }
//    }

//    // 現在のステージをリトライする
//    public void RetryStage()
//    {
//        currentStage = PlayerPrefs.GetInt("CurrentStage", currentStage); // 保存されたステージ番号を取得
//        string retrySceneName = "Stage" + currentStage; // 現在のステージ名を生成
//        if (Application.CanStreamedLevelBeLoaded(retrySceneName)) // シーンが存在するか確認
//        {
//            SceneManager.LoadScene(retrySceneName); // 現在のシーンを再読み込み
//        }
//        else
//        {
//            Debug.LogWarning("リトライ先のシーンが見つかりません: " + retrySceneName);
//        }
//    }

//    // 初期ステージに戻る
//    public void ResetToFirstStage()
//    {
//        currentStage = 2; // ステージ番号を初期化

//        string initialSceneName = "Stage" + currentStage; // 初期ステージ名を指定
//        PlayerPrefs.SetInt("CurrentStage", currentStage); // ステージ番号を保存
//        SceneManager.LoadScene(initialSceneName); // 最初のシーンを読み込む
//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public int currentStage = 1; // 現在のステージ番号を管理する公開変数
    private string currentScene = ""; // 現在のステージシーン

    //private static StageManager instance;

    //// AwakeメソッドでDontDestroyOnLoadを設定
    //void Awake()
    //{
    //    if (instance == null)
    //    {
    //        instance = this;
    //        DontDestroyOnLoad(gameObject); // このGameObjectをシーン間で破棄しない
    //    }
    //    else
    //    {
    //        Destroy(gameObject); // すでに存在する場合は新しいインスタンスを破棄
    //    }
    //}

    void Start()
    {
        // PlayerPrefsから現在のステージを読み込む
        if (PlayerPrefs.HasKey("CurrentStage"))
        {
            //currentStage = PlayerPrefs.GetInt("CurrentStage");
            currentScene = PlayerPrefs.GetString("CurrentScene");
        }
    }


    // 次のステージに進む
    public void NextStage()
    {
        currentStage++; // ステージ番号を更新
        Debug.Log("現在のステージ番号: " + currentStage);

        string nextSceneName = "Stage" + currentStage; // 次のシーン名を生成
        if (Application.CanStreamedLevelBeLoaded(nextSceneName)) // シーンが存在するか確認
        {
            PlayerPrefs.SetInt("CurrentStage", currentStage); // ステージ番号を保存
            SceneManager.LoadScene(nextSceneName); // 次のシーンを読み込む
        }
        else
        {
            Debug.LogWarning("次のシーンが見つかりません: " + nextSceneName);
        }


    }

    // 現在のステージをリトライする
    public void RetryStage()
    {
        currentStage = PlayerPrefs.GetInt("CurrentStage", currentStage); // 保存されたステージ番号を取得
        currentScene = PlayerPrefs.GetString("CurrentScene", currentScene); // 保存されたステージシーンを取得
        string retrySceneName = "Stage" + currentStage; // 現在のステージ名を生成

        //if (Application.CanStreamedLevelBeLoaded(retrySceneName)) // シーンが存在するか確認
        if (Application.CanStreamedLevelBeLoaded(currentScene)) // シーンが存在するか確認
        {
            SceneManager.LoadScene(currentScene); // 現在のシーンを再読み込み
        }
        else
        {
            Debug.LogWarning("リトライ先のシーンが見つかりません: " + currentScene);
        }
    }

    // 初期ステージに戻る
    public void ResetToFirstStage()
    {
        currentStage = 1; // ステージ番号を初期化

        string initialSceneName = "Stage" + currentStage; // 初期ステージ名を指定
        PlayerPrefs.SetInt("CurrentStage", currentStage); // ステージ番号を保存
        SceneManager.LoadScene(initialSceneName); // 最初のシーンを読み込む
    }

    // アプリ終了時にリトライ用のデータを消す
    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("CurrentStageNum");
        PlayerPrefs.DeleteKey("CurrentScene");
        PlayerPrefs.DeleteKey("MagObjPositions");
    }
}