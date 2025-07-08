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
		public bool delete = false;   // フェードイン/アウト後、消すかどうか
		public bool isFadeScreen = false;	// フェード用のスクリーンかどうか
		public float fadeTime = 1.0f; // フェードの時間（秒）
		public float delay = 0.0f;    // フェード開始までの遅延時間（秒）

		// Update内でGetComponentするのを避ける用
		[HideInInspector] public CanvasGroup canvasGroup;
	}

	public List<FadeObject> fadeObjects = new List<FadeObject>();	// Inspectorで複数オブジェクトを設定

	private Dictionary<GameObject, float> startTimes = new Dictionary<GameObject, float>();	// 各オブジェクトのフェード開始時間を管理するもの

	// Start is called before the first frame update
	void Start()
	{
		foreach (var fadeObj in fadeObjects)
		{
			// ゲーム開始時に各オブジェクトのフェード開始時間を記録
			startTimes[fadeObj.targetObj] = Time.time + fadeObj.delay;
			fadeObj.canvasGroup = fadeObj.targetObj.GetComponent<CanvasGroup>();
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
				? 1.0f - (elapsedTime / fadeObj.fadeTime)	// フェードイン
				: (elapsedTime / fadeObj.fadeTime);			// フェードアウト

			alphaValue = Mathf.Clamp01(alphaValue); // α値を 0-1 に制限

			// "CanvasGroup"を使用してフェード処理を適用
			if(fadeObj.canvasGroup != null)
			{
				fadeObj.canvasGroup.alpha = alphaValue;
			}

			// フェード完了判定
			if (elapsedTime >= fadeObj.fadeTime)
			{
				if (fadeObj.delete)
				{
					// 非表示にする場合
					fadeObj.targetObj.SetActive(false);

					// もしくは削除する場合
					// Destroy(fadeObj.targetObj);
				}
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