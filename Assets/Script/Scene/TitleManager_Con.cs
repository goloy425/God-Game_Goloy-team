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
	private GameInputs inputs;	// GameInputsクラス
	private AudioSource audioSource;
	private TitleManager_KeyMou tManager_k;

	// Start is called before the first frame update
	void Start()
	{
		gamepad = Gamepad.current;
		audioSource = GetComponent<AudioSource>();
		tManager_k = GetComponent<TitleManager_KeyMou>();

		inputs = new GameInputs();
		inputs.Enable();
	}

	// Update is called once per frame
	void Update()
	{
		if (gamepad == null) { return; }    // コントローラーが接続されてない場合はスルー

		// 入力の取得
		bool decideKey = inputs.Select.Decide.WasPressedThisFrame();        // 決定（任天堂:A PS:〇 Xbox:B）
		bool selectUpKey = inputs.Select.SelectUp.WasPressedThisFrame();            // 十字キー上
		bool selectDownKey = inputs.Select.SelectDown.WasPressedThisFrame();        // 　 〃 　下

		// 表示されてるUIによって決定ボタンの処理を変える
		if (tManager_k.GetUInum() == 0)     // PushtoStartの時
		{
			if (decideKey)
			{
				start.onClick.Invoke();
			}
		}
		else if (tManager_k.GetUInum() == 1)	// FromTheStartとかの時
		{
			if (decideKey && tManager_k.buttonIdx != 0)
			{
				// リトライできるデータがない、かつContinueが選択された時
				if (tManager_k.GetNoContinue() && tManager_k.buttonIdx == 2)
				{
					audioSource.PlayOneShot(beep);
				}
				else
				{
					if (tManager_k.buttonIdx != 2) { audioSource.PlayOneShot(goGame); } // スタート時のSE再生
					buttons[tManager_k.buttonIdx - 1].onClick.Invoke();
				}
			}

			// 取得した入力処理でインデックスを切り替える
			if (selectUpKey)	// 上
			{
				if (tManager_k.buttonIdx > 1) { tManager_k.buttonIdx--; }
				else if (tManager_k.buttonIdx == 0) { tManager_k.buttonIdx = 3; }
			}
			if (selectDownKey)  // 下
			{
				if (tManager_k.buttonIdx < buttons.Length) { tManager_k.buttonIdx++; }
				else { tManager_k.buttonIdx = 1; }
			}
		}
	}

	private void OnDestroy()
	{
		inputs?.Dispose();
	}
}
