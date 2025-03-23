//------------------------------------------------------------------------
// 本田洸都
// 一定間隔で指定したSEを再生
//------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySEAtRegularIntervals : MonoBehaviour
{
    [Header("再生するSE")]
    public AudioClip[] audioClips;
    [Header("何秒間隔で再生するか")]
    public float second = 5.0f;
    [Header("順番通りに再生するかどうか")]
    public bool inOrderFg = true;
    [Header("最初に1回再生するかどうか")]
    public bool playOnAwakeFg = true;

    private AudioSource audioSource;
    private int playNum = 0;            // 再生するSEの番号
    private float elapsedTime = 0.0f;   // 経過時間

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

        // 最初に一回だけ再生する時
        if (playOnAwakeFg)
        {
            // 順番通りに再生する時
            if (inOrderFg)
            {
                // playNum番目のSEを再生
                audioSource.PlayOneShot(audioClips[playNum]);
                playNum = (playNum + 1) % audioClips.Length;    // 要素数以上の番号にならないように更新
            }
            // 順番通りに再生しない時はランダムな順番で再生
            else
            {
                playNum = Random.Range(0, audioClips.Length - 1);
                audioSource.PlayOneShot(audioClips[playNum]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 指定した秒数になった時
        if (elapsedTime >= second)
        {
            // 順番通りに再生する時
            if (inOrderFg)
            {
                // playNum番目のSEを再生
                audioSource.PlayOneShot(audioClips[playNum]);
                playNum = (playNum + 1) % audioClips.Length;    // 要素数以上の番号にならないように更新
            }
            // 順番通りに再生しない時はランダムな順番で再生
            else
            {
                playNum = Random.Range(0, audioClips.Length - 1);
                audioSource.PlayOneShot(audioClips[playNum]);
            }
            elapsedTime = 0.0f;     // 経過時間をリセット
        }
        elapsedTime += Time.deltaTime;
    }

    public void SetElapsedTime(float _elapsedTime)
    {
        elapsedTime = _elapsedTime;
    }
}
