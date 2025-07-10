using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//==========================================================
// 制作者：宮本和音
// キーマウでタイトル画面を操作するスクリプト
//==========================================================

public class TitleManager_KeyMou : MonoBehaviour
{
	[Header("起動時のUIを設定")]
	public GameObject pushtoStart;
	[Header("PushtoStartの次のUI群を設定")]
	public GameObject startMenu;

    [Header("フェードアウトするImageを設定")]
    public FadeController fadeController;

	[Header("ボタン Start→Continue→Quitの順番で設定")]
	public Button[] buttons;
	[Header("SE　1:ビープ音 2:スタート")]
	public AudioClip beep;
	public AudioClip goGame;
    private AudioSource audioSource;
	private TitleUISwitch tUIswitch;

	[Header("ロードシーンに移行するまでの待ち時間")]
	public float delay = 0.5f;
	private float timer = 0.0f;


	private string currentScene = "";   // 現在のステージシーン Continueを表示するかどうかに使う
	private string nextScene = "";		// 次のシーン名
	private bool noContinue = false;
	private bool pressedFg = false;		// 押されたかどうか

	private int UInum = 0;		// 今どのUIパターンを表示しているか 0:PushtoStart 1:選択
	public int buttonIdx = 0;	// どのコマンドを選択しているか
	private int prevIdx = 0;    // UI切り替え用


	// Start is called before the first frame update
	void Start()
	{
		audioSource = GetComponent<AudioSource>();
		tUIswitch = GameObject.Find("Title").GetComponent<TitleUISwitch>();
	}

	// Update is called once per frame
	void Update()
	{
		timer += Time.deltaTime;

		// 表示しているUIによって決定ボタンの処理を変える
		if (UInum == 0)
		{
			if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
			{
				OnPushToStart();
			}
		}
		else
		{
			if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
			{
				// リトライできるデータがない、かつContinueが選択された時
				if (noContinue && buttonIdx == 2)
				{
					audioSource.PlayOneShot(beep);
				}
				else
				{
					if (buttonIdx != 3) { audioSource.PlayOneShot(goGame); } // スタート時のSE再生
					buttons[buttonIdx - 1].onClick.Invoke();
				}
			}

			// 取得した入力処理でインデックスを切り替える
			if (Input.GetKeyDown(KeyCode.UpArrow))	// 上
			{
				if (buttonIdx > 2) { buttonIdx--; }
				else if (buttonIdx == 1) { buttonIdx = 3; }
				else { buttonIdx = 1; }
			}
			if (Input.GetKeyDown(KeyCode.DownArrow))	// 下
			{
				if (buttonIdx < buttons.Length) { buttonIdx++; }
				else if (buttonIdx == 3) { buttonIdx = 1; }
				else { buttonIdx = 3; }
			}
		}

		// コマンドによるUIの切り替え
		switch (buttonIdx)
		{
			case 1:		// FromTheStart
				if (prevIdx == 2)
				{
					tUIswitch.Switch_ContinueNormal();
				}
				else if (prevIdx == 3)
				{
					tUIswitch.Switch_QuitNormal();
				}
				tUIswitch.Switch_StartGlow();
				break;

			case 2:		// Continue
				if (prevIdx == 1)
				{
					tUIswitch.Switch_StartNormal();
				}
				else if (prevIdx == 3)
				{
					tUIswitch.Switch_QuitNormal();
				}
				tUIswitch.Switch_ContinueGlow();
				break;

			case 3:		// QuitGame
				if (prevIdx == 1)
				{
					tUIswitch.Switch_StartNormal();
				}
				else if (prevIdx == 2)
				{
					tUIswitch.Switch_ContinueNormal();
				}
				tUIswitch.Switch_QuitGlow();
				break;
			default:
				break;
		}

		prevIdx = buttonIdx;

		if(pressedFg)
        {

            if (fadeController != null)
            {
                // フェードアウト完了後にシーン遷移
                if (fadeController.GetCompleteFadeOutFg() && timer >= delay)
                {
                    SceneManager.LoadScene(nextScene);
                }
            }
            else
            {
                SceneManager.LoadScene(nextScene);
            }
        }
	}


	//--- ボタン処理ズ ---//
	//--- PushtoStartが押された時 ---//
	public void OnPushToStart()
	{
		// UI切り替え
		pushtoStart.SetActive(false);

		// 保存されたステージシーンを取得
		currentScene = PlayerPrefs.GetString("CurrentScene", currentScene);

		// シーンが存在するか確認
		if (Application.CanStreamedLevelBeLoaded(currentScene))
		{
			noContinue = false;
		}
		else
		{
			noContinue = true;
		}

		startMenu.SetActive(true);
		UInum = 1;
	}

	//--- FromTheStartが押された時 ---//
	public void OnFromTheStart(string sceneName)
	{
		pressedFg = true;
		nextScene = sceneName;
		timer = 0.0f;
        fadeController.StartFadeOut();	// フェードアウト
    }

	//--- Continueが押された時 ---//
	// StageManagerのRetryStageが起動するようにしてある

	//--- QuitGameが押された時 ---//
	public void  OnQuitGame()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
	Application.Quit();//ゲームプレイ終了
#endif
	}

	public bool GetNoContinue()
	{
		return noContinue;
	}
	public int GetUInum()
	{
		return UInum;
	}
}
