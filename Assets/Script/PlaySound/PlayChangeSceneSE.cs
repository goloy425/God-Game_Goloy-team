//------------------------------------------------------------------------------
//  本田洸都
//  GameManagerからゲームクリア、ゲームオーバーのフラグを受け取りSEを再生する
//------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayChangeSceneSE : MonoBehaviour
{
    [Header("シーン遷移時のSE")]
    public AudioClip gameClearSE;
    public AudioClip gameOverSE;
    [Header("SE再生から何秒後に自身を削除するか")]
    public float destroySecond = 6.0f;
    [Header("フラグが立ってから何秒後に再生するか")]
    public float playSecond = 1.0f;

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
            Invoke("PlayGameClearSE", playSecond);               // playSecond秒後にSE再生
;           Destroy(gameObject, destroySecond + playSecond);     // DestroySecond秒後に自身を削除
            playCnt++;
        }

        // ゲームオーバーになった時、1回再生
        if (gameManager.GetGameOverFg() && playCnt < 1)
        {
            Invoke("PlayGameOverSE", playSecond);                // playSecond秒後にSE再生
            Destroy(gameObject, destroySecond + playSecond);     // DestroySecond秒後に自身を削除
            playCnt++;
        }
    }

    // ゲームクリア時のSE再生
    private void PlayGameClearSE()
    {
        audioSource.PlayOneShot(gameClearSE);   // ゲームクリアSE再生
    }

    // ゲームオーバー時のSE再生
    private void PlayGameOverSE()
    {
        audioSource.PlayOneShot(gameOverSE);   // ゲームオーバーSE再生
    }
}