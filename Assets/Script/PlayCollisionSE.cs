//--------------------------------------------------------------------------
// 本田洸都
// 指定したオブジェクトが当たった時に指定したSEを再生する
//--------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCollisionSE : MonoBehaviour
{
    [Header("衝突検知するオブジェクト")]
    public Object collisionObject = null;
    [Header("再生するSE")]
    public AudioClip collisionSE;
    [Header("1回のみ再生するかどうか")]
    public bool playOnceFg = false;

    private AudioSource audioSource;    // AudioSource
    private int playCnt = 0;            // 再生回数

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
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 衝突検知するオブジェクトが指定されている時
        if (collisionObject != null)
        {
            // 当たったオブジェクトが指定したオブジェクトと同じ時
            if (collision.gameObject == collisionObject)
            {
                // 1回のみ再生する時
                if (playOnceFg && playCnt < 1)
                {
                    audioSource.PlayOneShot(collisionSE);   // SEを再生
                    playCnt++;
                }
                // 何回も再生する時
                else if(!playOnceFg)
                {
                    audioSource.PlayOneShot(collisionSE);   // SEを再生
                }
            }
        }
        // 指定されていない時はすべてのオブジェクトと衝突検知
        else
        {
            // 1回のみ再生する時
            if (playOnceFg && playCnt < 1)
            {
                audioSource.PlayOneShot(collisionSE);   // SEを再生
                playCnt++;
            }
            // 何回も再生する時
            else if (!playOnceFg)
            {
                audioSource.PlayOneShot(collisionSE);   // SEを再生
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 衝突検知するオブジェクトが指定されている時
        if (collisionObject != null)
        {
            // 当たったオブジェクトが指定したオブジェクトと同じ時
            if (other.gameObject == collisionObject)
            {
                // 1回のみ再生する時
                if (playOnceFg && playCnt < 1)
                {
                    audioSource.PlayOneShot(collisionSE);   // SEを再生
                    playCnt++;
                }
                // 何回も再生する時
                else if (!playOnceFg)
                {
                    audioSource.PlayOneShot(collisionSE);   // SEを再生
                }
            }
        }
        // 指定されていない時はすべてのオブジェクトと衝突検知
        else
        {
            // 1回のみ再生する時
            if (playOnceFg && playCnt < 1)
            {
                audioSource.PlayOneShot(collisionSE);   // SEを再生
                playCnt++;
            }
            // 何回も再生する時
            else if (!playOnceFg)
            {
                audioSource.PlayOneShot(collisionSE);   // SEを再生
            }
        }
    }
}
