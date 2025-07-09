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
	[Header("SE　1:ビープ音 2:スタート")]
	public AudioClip beep;
	public AudioClip goGame;

	private Gamepad gamepad;
	private GameInputs inputs;  // GameInputsクラス
	private AudioSource audioSource;
	private TitleManager_KeyMou tManager_k;
	private TitleUISwitch tUIswitch;

	private int UInum = 0;		// 今どのUIパターンを表示しているか 0:PushtoStart 1:選択
	private int buttonIdx = 0;  // どのコマンドを選択しているか
	private int prevIdx = 0;		// UI切り替え用

	// Start is called before the first frame update
	void Start()
	{
		gamepad = Gamepad.current;
		audioSource = GetComponent<AudioSource>();
		tManager_k = GetComponent<TitleManager_KeyMou>();
		tUIswitch = GameObject.Find("Title").GetComponent<TitleUISwitch>();

		inputs = new GameInputs();
		inputs.Enable();
	}

	// Update is called once per frame
	void Update()
	{
		if (gamepad == null) { return; }	// コントローラーが接続されてない場合はスルー

		// 入力の取得
		bool decideKey = inputs.Select.Decide.WasPressedThisFrame();		// 決定（任天堂:A PS:〇 Xbox:B）
		bool selectUpKey = inputs.Select.SelectUp.WasPressedThisFrame();			// 十字キー上
		bool selectDownKey = inputs.Select.SelectDown.WasPressedThisFrame();		// 　 〃 　下

		// 表示されてるUIによって決定ボタンの処理を変える
		if (UInum == 0)		// PushtoStartの時
		{
			if (decideKey)
			{
				start.onClick.Invoke();
			}
		}
		else if (UInum == 1)	// FromTheStartとかの時
		{
			if (decideKey)
			{
				// リトライできるデータがない、かつContinueが選択された時
				if (tManager_k.GetNoContinue() && buttonIdx == 1)
				{
					audioSource.PlayOneShot(beep);
				}
				else
				{
					if (buttonIdx != 2) { audioSource.PlayOneShot(goGame); }	// スタート時のSE再生
					buttons[buttonIdx].onClick.Invoke();
				}
			}

			// 取得した入力処理でインデックスを切り替える
			if (selectUpKey)	// 上
			{
				if(buttonIdx > 0) { buttonIdx--; }
				else { buttonIdx = 2; }
			}
			if (selectDownKey)	// 下
			{
				if (buttonIdx < buttons.Length - 1) { buttonIdx++; }
				else { buttonIdx = 0; }
			}

			// コマンドによるUIの切り替え
			switch(buttonIdx)
			{
				case 0:		// FromTheStart
					if (prevIdx == 1) {
						tUIswitch.Switch_ContinueNormal();
					}
					else if (prevIdx == 2) {
						tUIswitch.Switch_QuitNormal();
					}
					tUIswitch.Switch_StartGlow();
					break;

				case 1:		// Continue
					if (prevIdx == 0) {
						tUIswitch.Switch_StartNormal();
					}
					else if (prevIdx == 2) {
						tUIswitch.Switch_QuitNormal();
					}
					tUIswitch.Switch_ContinueGlow();
					break;

				case 2:		// QuitGame
					if (prevIdx == 0) {
						tUIswitch.Switch_StartNormal();
					}
					else if (prevIdx == 1) {
						tUIswitch.Switch_ContinueNormal();
					}
					tUIswitch.Switch_QuitGlow();
					break;
				default:
					break;
			}
		}

		prevIdx = buttonIdx;
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
