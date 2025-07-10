using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//==========================================================
// 制作者：宮本和音
// リザルト（ゲームオーバー・ステージクリア）管理　キーマウ
//==========================================================


public class ResultManager_KeyMou : MonoBehaviour
{
	[Header("ボタン Retry→StageReset→ReturnTitleの順番で設定")]
	public Button[] buttons;

	private ResultUISwitch rUIswitch;

	public int buttonIdx = 0;	// どのコマンドを選択しているか
	private int prevIdx = 0;	// UI切り替え用

	// Start is called before the first frame update
	void Start()
	{
		GameObject.Find("UI").TryGetComponent<ResultUISwitch>(out rUIswitch);
	}

	// Update is called once per frame
	void Update()
	{
		// 決定ボタン(スペースキー・エンターキー)の処理
		if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
		{
			if (buttons.Length == 1)
			{
				buttons[buttonIdx].onClick.Invoke();
			}
			else
			{
				buttons[buttonIdx - 1].onClick.Invoke();
			}
		}

		// buttonsが2個以上ある時
		if (buttons.Length > 1)
		{
			// 取得した入力処理でインデックスを切り替える
			if (Input.GetKeyDown(KeyCode.UpArrow))		// 上
			{
				if (buttonIdx > 1) { buttonIdx--; }
				else if (buttonIdx == 1) { buttonIdx = 3; }
				else { buttonIdx = 1; }
			}
			if (Input.GetKeyDown(KeyCode.DownArrow))	// 下
			{
				if (buttonIdx < buttons.Length) { buttonIdx++; }
				else if (buttonIdx >= buttons.Length) { buttonIdx = 1; }
				else { buttonIdx = 3; }
			}
		}

		if (rUIswitch == null) { return; }

		// コマンドによるUIの切り替え
		switch (buttonIdx)
		{
			case 1:		// Retry
				if (prevIdx == 2)
				{
					rUIswitch.Switch_StageResetNormal();
				}
				else if (prevIdx == 3)
				{
					rUIswitch.Switch_TitleNormal();
				}
				rUIswitch.Switch_RetryGlow();
				break;

			case 2:		// StageReset
				if (prevIdx == 1)
				{
					rUIswitch.Switch_RetryNormal();
				}
				else if (prevIdx == 3)
				{
					rUIswitch.Switch_TitleNormal();
				}
				rUIswitch.Switch_StageResetGlow();
				break;

			case 3:		// ReturnToTitle
				if (prevIdx == 1)
				{
					rUIswitch.Switch_RetryNormal();
				}
				else if (prevIdx == 2)
				{
					rUIswitch.Switch_StageResetNormal();
				}
				rUIswitch.Switch_TitleGlow();
				break;
			default:
				break;
		}

		prevIdx = buttonIdx;
	}

	//--- ボタンの処理 ---//
	//--- Retryが押された時 ---//
	public void OnRetry(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}

	//--- StageResetが押された時 ---//
	public void OnStageReset(string sceneName)
	{
		PlayerPrefs.SetInt("CurrentStageNum", 0);	// 現在ステージを1に戻す
		PlayerPrefs.DeleteKey("MagObjPositions");	// 磁石のデータを削除
		PlayerPrefs.SetInt("Deaths", 0);			// 死亡回数をリセット
		PlayerPrefs.SetFloat("PlayTime", 0.0f);     // プレイ時間をリセット
		PlayerPrefs.Save();
		SceneManager.LoadScene(sceneName);
	}

	//--- ReturnToTitleが押された時 ---//
	public void OnReturnTitle(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}
}
