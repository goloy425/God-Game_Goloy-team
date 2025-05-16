using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================================
// 作成者：宮本和音
// ミニマップ用のカメラのオンオフを切り替える
//==================================================================

public class ControlMiniMapCam : MonoBehaviour
{
	[Header("miniMapCamをステージ順に設定")]
	public Camera[] miniMapCams;

	[Header("GameManagerを設定")]
	public GameManager gm;

	private int curStage;
	private int prevStage;

	// Start is called before the first frame update
	void Start()
	{
		// 開始時のカメラを設定
		switch (gm.GetStartStage() - 1)
		{
			case 0:
				miniMapCams[0].gameObject.SetActive(true);
				break;
			case 1:
				miniMapCams[1].gameObject.SetActive(true);
				break;
			case 2:
				miniMapCams[2].gameObject.SetActive(true);
				break;
			case 3:
				miniMapCams[3].gameObject.SetActive(true);
				break;
			case 4:
				miniMapCams[4].gameObject.SetActive(true);
				break;
		}

		// 最初は揃えておく
		curStage = gm.GetCurStage();
		prevStage = curStage;
	}

	// Update is called once per frame
	void Update()
	{
		curStage = gm.GetCurStage();

		// カメラの切り替え
		if (curStage != prevStage)
		{
			SwitchMiniMapCam();
		}

		prevStage = curStage;
	}

	//--- ミニマップカメラの切り替え ---//
	private void SwitchMiniMapCam()
	{
		miniMapCams[prevStage].gameObject.SetActive(false);
		miniMapCams[curStage].gameObject.SetActive(true);
	}
}
