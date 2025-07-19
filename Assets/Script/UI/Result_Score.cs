using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==========================================================
// 制作者：宮本和音
// ステージクリア画面でクリアタイム・死亡回数を表示する
//==========================================================

public class Result_Score : MonoBehaviour
{
	private NumDisplay nDis;

	// Start is called before the first frame update
	void Start()
	{
		nDis = GetComponent<NumDisplay>();
	}

	// Update is called once per frame
	void Update()
	{
		//--- プレイ時間 ---//
		if (PlayerPrefs.HasKey("PlayTime"))
		{
			int time = Mathf.FloorToInt(PlayerPrefs.GetFloat("PlayTime"));
			nDis.SetTime(time);
		}
		else
		{
			int time = 0;
			nDis.SetTime(time);
		}


		//--- 死亡回数 ---//
		if (PlayerPrefs.HasKey("Deaths"))
		{
			int deaths = PlayerPrefs.GetInt("Deaths");
			nDis.SetDeathCount(deaths);
		}
		else
		{
			int deaths = 0;
			nDis.SetDeathCount(deaths);
		}
	}

	private void OnDestroy()
	{
		PlayerPrefs.DeleteKey("PlayTime");
		PlayerPrefs.DeleteKey("Deaths");
	}
}
