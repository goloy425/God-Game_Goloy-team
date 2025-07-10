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
	[Header("ボタン Retry→StageReset→ReturnTitleの順番で設定")]
	public Button[] buttons;

	private Gamepad gamepad;
	private GameInputs inputs;  // GameInputsクラス
	private ResultManager_KeyMou rManager_k;

	// Start is called before the first frame update
	void Start()
	{
		gamepad = Gamepad.current;
		rManager_k = GetComponent<ResultManager_KeyMou>();

		inputs = new GameInputs();
		inputs.Enable();
	}

	// Update is called once per frame
	void Update()
	{
		if (gamepad == null) { return; }    // コントローラーが接続されてない場合はスルー

		// 入力の取得
		bool decideKey = inputs.Select.Decide.WasPressedThisFrame();	// 決定（任天堂:A PS:〇 Xbox:B）
		bool selectUpKey = inputs.Select.SelectUp.WasPressedThisFrame();		// 十字キー上
		bool selectDownKey = inputs.Select.SelectDown.WasPressedThisFrame();	// 　 〃 　下

		// 決定ボタンの処理
		if (decideKey)
		{
			if (buttons.Length == 1)
			{
				buttons[rManager_k.buttonIdx].onClick.Invoke();
			}
			else
			{
				if (rManager_k.buttonIdx != 0)
				{ 
					buttons[rManager_k.buttonIdx - 1].onClick.Invoke();
				}
			}
		}

		if (buttons.Length > 1)
		{
			// 取得した入力処理でインデックスを切り替える
			if (selectUpKey)	// 上
			{
				if (rManager_k.buttonIdx > 1) { rManager_k.buttonIdx--; }
				else { rManager_k.buttonIdx = 3; }
			}
			if (selectDownKey)	// 下
			{
				if (rManager_k.buttonIdx < buttons.Length) { rManager_k.buttonIdx++; }
				else { rManager_k.buttonIdx = 1; }
			}
		}

		// UIの切り替えはKeyMouの方でやる
	}

	private void OnDestroy()
	{
		inputs?.Dispose();
	}
}
