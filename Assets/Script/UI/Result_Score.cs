using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//==========================================================
// 制作者：宮本和音
// ステージクリア画面でクリアタイム・死亡回数を表示する
//==========================================================

public class Result_Score : MonoBehaviour
{
	private TextMeshProUGUI playTime;
	private TextMeshProUGUI deaths;
	private GameManager gm;

	// Start is called before the first frame update
	void Start()
	{
		playTime = GameObject.Find("PlayTime").GetComponent<TextMeshProUGUI>();
		deaths = GameObject.Find("Deaths").GetComponent<TextMeshProUGUI>();
	}

	// Update is called once per frame
	void Update()
	{
		//--- プレイ時間 ---//
		float time = PlayerPrefs.GetFloat("PlayTime");

		int minutes = Mathf.FloorToInt(time / 60);
		int seconds = Mathf.FloorToInt(time % 60);
		int fraction = Mathf.FloorToInt((time * 10) % 10);

		playTime.text=string.Format("{0:00}:{1:00}.{2}", minutes, seconds, fraction);

        //--- 死亡回数 ---//
    }
}
