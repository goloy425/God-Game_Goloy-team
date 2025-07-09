using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//==========================================================
// 制作者：宮本和音
// リザルト（ゲームオーバー・ステージクリア）管理　キーマウ
//==========================================================


public class ResultManager_KeyMou : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		
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
		SceneManager.LoadScene(sceneName);
	}

	//--- ReturnToTitleが押された時 ---//
	public void OnReturnTitle(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}
}
