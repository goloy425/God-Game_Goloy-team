using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================================
// 作成者：宮本和音
// マップ用オブジェクト（仮）のアクティブ・非アクティブの切り替え
//==================================================================

public class SwitchMap : MonoBehaviour
{
	[Header("マップオブジェクトのObjectsを設定")]
	public GameObject[] Objects;

	[Header("GameManagerを設定")]
	public GameManager gm;

	// Start is called before the first frame update
	void Start()
	{
		// 邪魔なので作成中以外は非アクティブにしてあるはずだが、一応全部非アクティブにしておく
		//for (int i = 0; i < Objects.Length; i++)
		//{
		//	Objects[i].gameObject.SetActive(false);
		//}

		Objects[0].gameObject.SetActive(false);		// デバッグ用　まだ1-1しかないため

		// 開始ステージのマップだけアクティブにする
		Objects[gm.GetStartStage() - 1].gameObject.SetActive(true);
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
