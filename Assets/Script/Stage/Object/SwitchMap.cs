using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================================
// 作成者：宮本和音
// マップ用オブジェクト（仮）のアクティブ・非アクティブの切り替え
//==================================================================

public class SwitchMap : MonoBehaviour
{
	[Header("マップオブジェクト(カメラ+Objectsの塊)を設定")]
	public GameObject[] Objects;

	[Header("GameManagerを設定")]
	public GameManager gm;

	private int curStage;
	private int prevStage;

	// Start is called before the first frame update
	void Start()
	{
		// 邪魔なので作成中以外は非アクティブにしてあるはずだが、一応全部非アクティブにしておく
		for (int i = 0; i < Objects.Length; i++)
		{
			Objects[i].gameObject.SetActive(false);
		}

		// 開始ステージのマップだけアクティブにする
		Objects[gm.GetStartStage() - 1].gameObject.SetActive(true);

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
			SwitchMiniMap();
		}

		prevStage = curStage;
	}

	//--- ミニマップの切り替え ---//
	private void SwitchMiniMap()
	{
		Objects[prevStage].gameObject.SetActive(false);
		Objects[curStage].gameObject.SetActive(true);
	}
}
