//----------------------------------------------------------------------------
// 本田洸都
// このスクリプトをアタッチしたImageオブジェクトをフェードできるようにする
// 試しに作ったのでゴロイ君のスクリプトに切り替える可能性あり
//----------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    [Header("フェードにかける時間")]
    public float fadeDuration = 0.5f;

    private Image fadeImage;    // フェードさせるImage
    private bool completeFadeInFg = false;
    private bool completeFadeOutFg = false; 

    // Start is called before the first frame update
    void Start()
    {
        fadeImage = this.gameObject.GetComponent<Image>();
    }

    // フェードインのコルーチン
    IEnumerator FadeIn()
    {
        float timer = 0.0f;

        // フェードイン
        while (timer < fadeDuration)
        {
            completeFadeInFg = false;
            timer += Time.deltaTime;
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1 - (timer / fadeDuration));
            yield return null;
        }
        completeFadeInFg = true;
        yield break;
        //fadeImage.gameObject.SetActive(false);  // フェードイン終了時に非表示
    }

    // フェードアウトのコルーチン
    IEnumerator FadeOut()
    {
        //fadeImage.gameObject.SetActive(true);  // フェードアウト終了時に表示
        float timer = 0.0f;

        // フェードアウト
        while (timer < fadeDuration)
        {
            completeFadeOutFg = false;
            timer += Time.deltaTime;
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, timer / fadeDuration);
            yield return null;
        }
        completeFadeOutFg = true;
        yield break;
    }

    // 外部から呼び出し可能なフェードイン開始関数
    public void StartFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    // 外部から呼び出し可能なフェードアウト開始関数
    public void StartFadeOut()
    {
        StartCoroutine(FadeOut());
    }

    public bool GetCompleteFadeInFg()
    {
        return completeFadeInFg;
    }

    public bool GetCompleteFadeOutFg()
    {
        return completeFadeOutFg;
    }
}
