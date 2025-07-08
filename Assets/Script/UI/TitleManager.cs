using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//==========================================================
// 制作者：宮本和音
// タイトル画面のUI・処理を管理するスクリプト
//==========================================================

public class TitleManager : MonoBehaviour
{
	[Header("PushtoStartのボタンを設定")]
	public Button start;

	private GameInputs inputs;  // GameInputsクラス
	private int UInum = 0;	// 今どのUIパターンを表示しているか 0:PushtoStart 1:選択

	// Start is called before the first frame update
	void Start()
	{
		inputs = new GameInputs();
		inputs.Enable();
	}

	// Update is called once per frame
	void Update()
	{
		bool keyDecision = inputs.Title.Decide.IsPressed();	// キー入力取得

		if (UInum == 0)
		{
			// とりあえず
			if (keyDecision)
			{
				start.onClick.Invoke();
			}
		}
	}

	private void OnDestroy()
	{
		inputs?.Dispose();
	}
}
