using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoLoader : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string nextSceneName = "Stage1";
    public float delay = 1.0f;      // ロードを待つ秒数
    private float timer = 0.0f;
    private bool loadFg = false;    // ロードするフラグ
    private bool finishFg = false;

    void Start()
    {
        // 動画終了時のイベントを登録
        videoPlayer.loopPointReached += OnVideoFinished;

        videoPlayer.Play(); // 動画再生開始
    }

    private void Update()
    {
        // 待つ秒数経過後にロード
        if(timer >= delay)
        {
            loadFg = true;
        }
        else
        {
            // タイマー更新
            timer += Time.deltaTime;
        }

        // ロード開始
        if(loadFg)
        {
            StartCoroutine("ChangeS", nextSceneName);
            loadFg = false;
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        finishFg = true;
    }

    private IEnumerator ChangeS(string _name)
    {
        // 非同期でシーンを読み込む
        UnityEngine.AsyncOperation async = SceneManager.LoadSceneAsync(_name);
        async.allowSceneActivation = false; // 読み込み終わっても遷移しないようにする

        // 読み込み完了までループ
        while (true)
        {
            // 進捗が9割を超えたら
            if (async.progress >= 0.9f && finishFg)
            {
                async.allowSceneActivation = true;  //遷移を許可する
            }
            yield return null;  // 1フレーム待つ
        }
    }
}
