using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pose : MonoBehaviour
{
	CanvasGroup Canvas;		// CanvasGroupコンポーネントを取得
	public Canvas pose;
	public Image targetImage;	// 操作するImage

	public Button[] myButton;

	int num = 1;

	bool _pose = false;
	bool select = true;

	private GameInputs inputs;	// GameInputsクラス

	// キー入力取得用
	private bool nowFg;
	private bool prevFg;

	// 点滅周りの変数
	private float time = 0.0f;
	private bool activeCanvas = false;
	private bool blinking = false;

	[Header("キャンバスを点滅させる時間")]
	public float blinkCanvasTime = 0.3f;

	// Start is called before the first frame update
	void Start()
	{
		Canvas = this.GetComponent<CanvasGroup>();
		Canvas.alpha = 0.0f;		// あらかじめ透明化

		inputs = new GameInputs();
		inputs.Enable();
	}

	// Update is called once per frame
	void Update()
	{
		bool key = inputs.Pose.SwitchPose.IsPressed();	// オプションボタン（右側）の入力取得
		nowFg = key;	// フラグの反映

		// キー入力でフラグの切り替え
		if (nowFg && !prevFg)
		{
			if (!_pose)
			{
				// ポーズ画面を開く＆コマンドを初期化
				_pose = true;
				num = 1;

				myButton[num].GetComponent<Image>().color = Color.red;
			}
			else
			{
				// ポーズ画面を閉じる
				_pose = false;
			}

			time = 0.0f;
			blinking = true;
		}

		if (blinking && time < blinkCanvasTime)
		{
			time += Time.deltaTime;
			BrinkPoseCanvas();  // 点滅の演出
		}
		else if (blinking && time >= blinkCanvasTime)
		{
			DisplayPose(_pose);		// フラグに応じてキャンバスの表示・非表示を確定させる
			activeCanvas = _pose;	// フラグの反映
		}

		prevFg = nowFg;		// フラグ更新

		//--- ポーズ画面の操作 ---//
		// 実装終わったらこの行と以下4行のコメント消して大丈夫です
		// inputs.Pose.SelectUp.IsPressed()で十字上
		// 　　〃　   .SelectDown.IsPressed()で十字下
		// 　　〃　   .Decide.IsPressed()で○ボタンの入力取得ができます

		if (_pose)
		{
            Color newColor = targetImage.color;
            newColor.a = 0.5f;
            targetImage.color = newColor;


            float move = Input.GetAxis("Vertical");

			if (select)
			{
				// 上ボタンの入力検出
				if (move > 0.4)
				{
					myButton[num].GetComponent<Image>().color = Color.white;

					--num;
					num = Mathf.Clamp(num, 0, 3);

					select = false;

					myButton[num].GetComponent<Image>().color = Color.red;
				}
				else if (move < -0.4)
				{
					myButton[num].GetComponent<Image>().color = Color.white;

					++num;
					num = Mathf.Clamp(num, 0, 3);

					select = false;

					myButton[num].GetComponent<Image>().color = Color.red;
				}
			}
			else if (move > -0.1 && move < 0.1)
			{
				select = true;
			}

			// 〇ボタン（XBOXのBボタン）の入力検出
			if (Input.GetKeyDown(KeyCode.JoystickButton2)||inputs.Pose.Select.IsPressed())
			{
				OnButtonClicked();
			}
		}
		Debug.Log(num);
	}

	//--- ポーズを開く・閉じる時にキャンバスを点滅させる演出用の関数 ---//
	void BrinkPoseCanvas()
	{
		if (activeCanvas)
		{
			Canvas.alpha = 1.0f;

			Color newColor = targetImage.color;
			newColor.a = 0.5f;
			targetImage.color = newColor;

			activeCanvas = false;
		}
		else
		{
			Canvas.alpha = 0.0f;

			Color newColor = targetImage.color;
			newColor.a = 0.0f;
			targetImage.color = newColor;

			activeCanvas = true;
		}
	}

	//--- 点滅後キャンバスを表示するかしないかを確定させる関数 ---//
	void DisplayPose(bool pose)
	{
		if (pose)
		{
			Canvas.alpha = 1.0f;
		}
		else
		{
			Canvas.alpha = 0.0f;
		}

		blinking = false;	// 点滅終了
	}

	void OnButtonClicked()
	{
		switch (num)
		{
			case 0:
				SceneManager.LoadScene("MainMenu");
				break;

			case 1:
				Canvas.alpha = 0.0f;
				break;

			case 2:

				break;

			case 3:
				SceneManager.LoadScene("Title");
				break;
		}
	}

	public bool GetPose()
	{
		return _pose;
	}

	private void OnDestroy()
	{
		inputs?.Dispose();
	}
}
