using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

//==========================================================
// 制作者：宮本和音
// タイトル画面のUI・処理を管理するスクリプト
//==========================================================

public class TitleManager_Con : MonoBehaviour
{
	[Header("PushtoStartのボタンを設定")]
	public Button start;
	[Header("ボタン Start→Continue→Quitの順番で設定")]
	public Button[] buttons;

	private Gamepad gamepad;
	private GameInputs inputs;  // GameInputsクラス
	private TitleManager_KeyMou tManager_k;
	private TitleUISwitch tUIswitch;

	private int UInum = 0;		// 今どのUIパターンを表示しているか 0:PushtoStart 1:選択
	public int buttonIdx = 0;	// 

	// Start is called before the first frame update
	void Start()
	{
		gamepad = Gamepad.current;
		tManager_k = GetComponent<TitleManager_KeyMou>();
		tUIswitch = GameObject.Find("Title").GetComponent<TitleUISwitch>();

		inputs = new GameInputs();
		inputs.Enable();
	}

	// Update is called once per frame
	void Update()
	{
		if (gamepad == null) { return; }

		bool decideKey = inputs.Title.Decide.WasPressedThisFrame();
		bool selectUpKey = inputs.Title.SelectUp.WasPressedThisFrame();
		bool selectDownKey = inputs.Title.SelectDown.WasPressedThisFrame();

		if (UInum == 0)
		{
			if (decideKey)
			{
				start.onClick.Invoke();
			}
		}
		else if (UInum == 1)
		{
			if (decideKey)
			{
				if (tManager_k.GetNoContinue() && buttonIdx == 1)
				{
					Debug.Log("リトライないよ");
					// のSEを鳴らしたい
				}
				else
				{
					buttons[buttonIdx].onClick.Invoke();
				}
			}

			if (selectUpKey && buttonIdx > 0)
			{
				buttonIdx--;
			}
			if (selectDownKey && buttonIdx < buttons.Length - 1)
			{
				buttonIdx++;
			}

			switch(buttonIdx)
			{
				case 0:
					tUIswitch.Switch_StartGlow();
					tUIswitch.Switch_ContinueNormal();
					break;
				case 1:
					tUIswitch.Switch_ContinueGlow();
					tUIswitch.Switch_StartNormal();
					tUIswitch.Switch_QuitNormal();
					break;
				case 2:
					tUIswitch.Switch_QuitGlow();
					tUIswitch.Switch_ContinueNormal();
					break;
				default:
					break;
			}
		}
	}

	public void SetUINum(int num)
	{
		UInum = num;
	}


	private void OnDestroy()
	{
		inputs?.Dispose();
	}
}
