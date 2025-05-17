using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 制作者　ゴロイ
public class Fade : MonoBehaviour
{
    // フェードするオブジェクト情報を管理するクラス
    [System.Serializable]
    public class FadeObject
    {
        public GameObject targetObj;  // フェードする画像
        public bool fadeIn = true;    // フェードインするかフェードアウトするか
        public float fadeTime = 1.0f; // フェードの時間（秒）
        public float delay = 0.0f;    // フェード開始までの遅延時間（秒）
    }

    public List<FadeObject> fadeObjects = new List<FadeObject>(); // Inspectorで複数オブジェクトを設定

    private Dictionary<GameObject, float> startTimes = new Dictionary<GameObject, float>(); // 各オブジェクトのフェード開始時間を管理するもの

    // Start is called before the first frame update
    void Start()
    {
        // ゲーム開始時に各オブジェクトのフェード開始時間を記録
       foreach(var fadeObj in fadeObjects)
        {
            startTimes[fadeObj.targetObj] = Time.time + fadeObj.delay; // 各オブジェクトの開始時間 + 遅延時間を設定
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var fadeObj in fadeObjects)
        {
            if (fadeObj.targetObj == null) continue; // オブジェクトが設定されていない場合はスキップ

            float elapsedTime = Time.time - startTimes[fadeObj.targetObj]; // 経過時間を計算
            
            // フェードの進行度を計算
            float alphaValue = fadeObj.fadeIn
                ? 1.0f - (elapsedTime / fadeObj.fadeTime) // フェードイン
                : (elapsedTime / fadeObj.fadeTime);       // フェードアウト

            alphaValue = Mathf.Clamp01(alphaValue); // α値を 0-1 に制限

            // "CanvasGroup"を使用してフェード処理を適用
            CanvasGroup canvasGroup = fadeObj.targetObj.GetComponent<CanvasGroup>();
            if(canvasGroup != null)
            {
                canvasGroup.alpha = alphaValue;
            }


            //// UI要素を取得し、フェードを適応
            //Image img = fadeObj.targetObj.GetComponent<Image>();
            //if (img != null)
            //{
            //    img.color = new Color(0, 0, 0, alphaValue);
            //}
        }

    }
}