using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==========================================================
// 制作者：宮本和音
// 磁石同士or磁石とオブジェクトが離れすぎる直前のやつ
//==========================================================


public class TooFarAway : MonoBehaviour
{
	[Header("プレイヤーの磁石を設定")]
	public Magnetism magnet1;
	public Magnetism magnet2;

	private float dangerDist;	// 危険距離
	private float safetyDist;	//  ↑ から脱する距離

	// 2つに分かれるやつ用
	private float dangerDist_c;
	private float safetyDist_c;

	// プレイヤー用
	private float pDangerDist;
	private float pSafetyDist;

	// Start is called before the first frame update
	void Start()
	{
		// 各距離を設定
		dangerDist = magnet1.magnetismRange * 0.75f;
		safetyDist = dangerDist - 0.4f;

		dangerDist_c = 8.0f * 0.85f;	// 2つに分かれるやつの磁力範囲
		safetyDist_c = dangerDist_c - 0.4f;

		pDangerDist = magnet1.magnetismRange * 0.85f;
		pSafetyDist = pDangerDist - 0.4f;
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	//--- 各距離のゲッター ---//
	public float GetDangerDist()
	{
		return dangerDist;
	}
	public float GetSafetyDist()
	{
		return safetyDist;
	}

	public float GetDangerDist_C()
	{
		return dangerDist_c;
	}
	public float GetSafetyDist_C()
	{
		return safetyDist_c;
	}

	public float GetPDangerDist()
	{
		return pDangerDist;
	}
	public float GetPSafetyDist()
	{
		return pSafetyDist;
	}
}
