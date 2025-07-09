using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

//==================================================================
// 制作者：宮本和音
// リザルト（ゲームオーバー・ステージクリア）管理　コントローラー
//==================================================================

public class ResultManager_Con : MonoBehaviour
{
	[Header("ボタン Start→Continue→Quitの順番で設定")]
	public Button[] buttons;

	private Gamepad gamepad;
	private GameInputs inputs;  // GameInputsクラス
	private ResultUISwitch rUIswitch;

	public int buttonIdx = 0;	// どのコマンドを選択しているか
	private int prevIdx = 0;	// UI切り替え用

	// Start is called before the first frame update
	void Start()
	{
		gamepad = Gamepad.current;
		GameObject.Find("UI").TryGetComponent<ResultUISwitch>(out rUIswitch);

		inputs = new GameInputs();
		inputs.Enable();
	}

	// Update is called once per frame
	void Update()
	{
		if (gamepad == null) { return; }    // コントローラーが接続されてない場合はスルー

		// 入力の取得
		bool decideKey = inputs.Select.Decide.WasPressedThisFrame();    // 決定（任天堂:A PS:〇 Xbox:B）
		bool selectUpKey = inputs.Select.SelectUp.WasPressedThisFrame();        // 十字キー上
		bool selectDownKey = inputs.Select.SelectDown.WasPressedThisFrame();    // 　 〃 　下

		// 決定ボタンの処理
		if (decideKey)
		{
			buttons[buttonIdx].onClick.Invoke();
		}

		if (buttons.Length > 1)
		{
			// 取得した入力処理でインデックスを切り替える
			if (selectUpKey)	// 上
			{
				if (buttonIdx > 0) { buttonIdx--; }
				else { buttonIdx = 2; }
			}
			if (selectDownKey)	// 下
			{
				if (buttonIdx < buttons.Length - 1) { buttonIdx++; }
				else { buttonIdx = 0; }
			}
		}

		if(rUIswitch == null) { return; }
		
		// コマンドによるUIの切り替え
		switch (buttonIdx)
		{
			case 0:		// Retry
				if (prevIdx == 1)
				{
					rUIswitch.Switch_StageResetNormal();
				}
				else if (prevIdx == 2)
				{
					rUIswitch.Switch_TitleNormal();
				}
				rUIswitch.Switch_RetryGlow();
				break;

			case 1:		// StageReset
				if (prevIdx == 0)
				{
					rUIswitch.Switch_RetryNormal();
				}
				else if (prevIdx == 2)
				{
					rUIswitch.Switch_TitleNormal();
				}
				rUIswitch.Switch_StageResetGlow();
				break;

			case 2:		// ReturnToTitle
				if (prevIdx == 0)
				{
					rUIswitch.Switch_RetryNormal();
				}
				else if (prevIdx == 1)
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

	private void OnDestroy()
	{
		inputs?.Dispose();
	}
}
