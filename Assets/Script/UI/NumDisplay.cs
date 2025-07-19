using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//==========================================================
// 制作者：宮本和音
// 変動する数字のUIを画像で表示するスクリプト
//==========================================================

public class NumDisplay : MonoBehaviour
{
	[Header("数字の画像を0~9の順に設定")]
	[SerializeField] private Sprite[] numberSprites;

	[Header("クリアタイム(時間:分:秒 コロンは飛ばして左から順番)")]
	[SerializeField] private Image[] timeDigit;

	[Header("死亡回数(最大3桁 100→10→1の位の順に設定)")]
	[SerializeField] private Image[] deathsDigit;

	// クリアタイム(秒数)を00:00:00の形式で表示する
	public void SetTime(int totalSeconds)
	{
		int hours = totalSeconds / 3600;
		int minutes = (totalSeconds % 3600) / 60;
		int seconds = totalSeconds % 60;

		string timeStr = hours.ToString("D2") + minutes.ToString("D2") + seconds.ToString("D2");

		int digitIndex = 0;
		for (int i = 0; i < timeStr.Length; i++)
		{
			if (timeDigit[digitIndex].gameObject.name.Contains("colon"))
			{
				digitIndex++;	// コロンは飛ばす
			}

			int digit = int.Parse(timeStr[i].ToString());
			timeDigit[digitIndex].sprite = numberSprites[digit];
			timeDigit[digitIndex].enabled = true;
			digitIndex++;
		}
	}

	// 死亡回数を最大3桁で表示する
	public void SetDeathCount(int count)
	{
		// 3桁固定・0埋め
		string countStr = count.ToString("D3");

		for (int i = 0; i < 3; i++)
		{
			int digit = int.Parse(countStr[i].ToString());
			deathsDigit[i].sprite = numberSprites[digit];
			deathsDigit[i].enabled = true;
		}
	}
}
