//------------------------------------------------------------------------------
//  本田洸都
//  GameManagerからゲームクリア、ゲームオーバーのフラグを受け取りSEを再生する
//------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayChangeSceneSE : MonoBehaviour
{
    [Header("シーン遷移時のSE")]
    public AudioClip gameClearSE;
    public AudioClip gameOverSE;
    [Header("SE再生から何秒後に自身を削除するか")]
    public int DestroySecond = 6;

    private AudioSource audioSource;
    private GameManager gameManager;

    private int playCnt = 0;    // 再生回数

    // Start is called before the first frame update
    void Start()
    {
        // コンポーネントを取得
        audioSource = GetComponent<AudioSource>();

        // AudioSourceがnullの時、エラーメッセージを表示
        if (audioSource == null)
        {
            Debug.LogError(this.name + "にAudioSourceが存在しません");
        }
        // GameManagerコンポーネントを取得
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // シーン遷移しても削除されないようにする
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        // ゲームクリアした時、1回再生
        if(gameManager.GetGameClearFg() && playCnt < 1)
        {
            audioSource.PlayOneShot(gameClearSE);   // ゲームクリアSE再生
            Destroy(gameObject, DestroySecond);     // DestroySecond秒後に自身を削除
            playCnt++;
        }

        // ゲームオーバーになった時、1回再生
        if (gameManager.GetGameOverFg() && playCnt < 1)
        {
            audioSource.PlayOneShot(gameOverSE);    // ゲームオーバーSE再生
            Destroy(gameObject, DestroySecond);     // DestroySecond秒後に自身を削除
            playCnt++;
        }
    }
}
