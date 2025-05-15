using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 制作者　ゴロイ
public class Fade : MonoBehaviour
{
    [System.Serializable]
    public class FadeObject
    {
        public GameObject targetObj;  // フェードする画像
        public bool fadeIn = true;       // フェードインするかフェードアウトするか
        public float fadeTime = 1.0f; // フェードの時間
    }

    public List<FadeObject> fadeObjects = new List<FadeObject>(); // Inspectorで複数オブジェクトを設定

    private Dictionary<GameObject, float> startTimes = new Dictionary<GameObject, float>();

    // Start is called before the first frame update
    void Start()
    {
       foreach(var fadeObj in fadeObjects)
        {
            startTimes[fadeObj.targetObj] = Time.time; // 各オブジェクトの開始時間を設定
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var fadeObj in fadeObjects)
        {
            if (fadeObj.targetObj == null) continue; // オブジェクトが設定されていない場合はスキップ

            float elapsedTime = Time.time - startTimes[fadeObj.targetObj];
            float alphaValue = fadeObj.fadeIn
                ? 1.0f - (elapsedTime / fadeObj.fadeTime)
                : (elapsedTime / fadeObj.fadeTime);

            alphaValue = Mathf.Clamp01(alphaValue); // α値を 0-1 に制限

            Image img = fadeObj.targetObj.GetComponent<Image>();
            if (img != null)
            {
                img.color = new Color(0, 0, 0, alphaValue);
            }
        }

    }
    public void Load()
    {
        Application.LoadLevel(Application.loadedLevelName);
    }
}