using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultUISwitch : MonoBehaviour
{
	[Header("êÿÇËë÷Ç¶å≥ÅEêÿÇËë÷Ç¶êÊÇÃUI")]
	public GameObject retryNormal;
	public GameObject retryGlow;
	public GameObject stageResetNormal;
	public GameObject stageResetGlow;
	public GameObject titleNormal;
	public GameObject titleGlow;


	//--- Retry ---//
	public void Switch_RetryGlow()
	{
		retryNormal.SetActive(false);
		retryGlow.SetActive(true);
	}
	public void Switch_RetryNormal()
	{
		retryGlow.SetActive(false);
		retryNormal.SetActive(true);
	}

	//--- StageReset ---//
	public void Switch_StageResetGlow()
	{
		stageResetNormal.SetActive(false);
		stageResetGlow.SetActive(true);
	}
	public void Switch_StageResetNormal()
	{
		stageResetGlow.SetActive(false);
		stageResetNormal.SetActive(true);
	}

	//--- ReturnToTitle ---//
	public void Switch_TitleGlow()
	{
		titleNormal.SetActive(false);
		titleGlow.SetActive(true);
	}
	public void Switch_TitleNormal()
	{
		titleGlow.SetActive(false);
		titleNormal.SetActive(true);
	}
}
