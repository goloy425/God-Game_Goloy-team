using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

	private TitleManager_Con tManager_c;

	private string currentScene = "";	// 現在のステージシーン Continueを表示するかどうかに使う
	private bool noContinue = false;


	// Start is called before the first frame update
	void Start()
	{
		tManager_c = GetComponent<TitleManager_Con>();
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
		if (Application.CanStreamedLevelBeLoaded(currentScene))	{
			noContinue = false;
		}
		else {
			noContinue = true;
		}

		startMenu.SetActive(true);
		tManager_c.SetUINum(1);		// UI切り替えを伝える
	}

	//--- FromTheStartが押された時 ---//
	public void OnFromTheStart(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
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
}
