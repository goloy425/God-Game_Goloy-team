using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==========================================================
// 制作者：宮本和音
// 選択中のコマンドのUIを切り替えるスクリプト
//==========================================================

public class TitleUISwitch : MonoBehaviour
{
	[Header("切り替え元・切り替え先のUI")]
	public GameObject startNormal;
	public GameObject startGlow;
	public GameObject continueNormal;
	public GameObject continueGlow;
	public GameObject quitNormal;
	public GameObject quitGlow;

	//--- FromTheStart ---//
	public void Switch_StartGlow()
	{
		startNormal.SetActive(false);
		startGlow.SetActive(true);
	}
	public void Switch_StartNormal()
	{
		startGlow.SetActive(false);
		startNormal.SetActive(true);
	}

	//--- Continue ---//
	public void Switch_ContinueGlow()
	{
		continueNormal.SetActive(false);
		continueGlow.SetActive(true);
	}
	public void Switch_ContinueNormal()
	{
		continueGlow.SetActive(false);
		continueNormal.SetActive(true);
	}

	//--- QuitGame ---//
	public void Switch_QuitGlow()
	{
		quitNormal.SetActive(false);
		quitGlow.SetActive(true);
	}
	public void Switch_QuitNormal()
	{
		quitGlow.SetActive(false);
		quitNormal.SetActive(true);
	}
}
