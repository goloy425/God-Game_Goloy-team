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

	[Header("プレイヤー(Controllerじゃない)を設定")]
	public GameObject playerL;
	public GameObject playerR;

	[Header("遠すぎ危険の時のRB.dragの数値")]
	[SerializeField] private float dangerDrag = 0.0f;

	private float dangerDist;	// 危険距離
	private float safetyDist;	//  ↑ から脱する距離

	// 2つに分かれるやつ用
	private float dangerDist_c;
	private float safetyDist_c;

	// プレイヤー用
	private float pDangerDist;
	private float pSafetyDist;

	// プレイヤーのコンポーネント
	private Magnetism magL;
	private Magnetism magR;
	private Rigidbody pLrb;
	private Rigidbody pRrb;

	// Start is called before the first frame update
	void Start()
	{
		// 各距離を設定　AdjustMagnetismが関わってくるので現状の通常磁力範囲を固定値で入れてある…
		dangerDist = 7.0f * 0.8f;
		safetyDist = dangerDist - 0.05f;

		dangerDist_c = 8.0f * 0.8f;	// 8.0f：2つに分かれるやつの磁力範囲
		safetyDist_c = dangerDist_c - 0.05f;

		pDangerDist = 9.0f * 0.8f;		// 9.0F：プレイヤーの磁力範囲
		pSafetyDist = pDangerDist - 0.05f;

		// プレイヤーのコンポーネントを取得
		magL = playerL.transform.Find("Magnet1").GetComponent<Magnetism>();
		magR = playerR.transform.Find("Magnet2").GetComponent<Magnetism>();
		pLrb = playerL.GetComponent<Rigidbody>();
		pRrb = playerR.GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	void Update()
	{
		// どっちも危なくない時はdragをもとの値に戻しておく
		if (!magL.dangerFarAway_magObj && !magL.dangerFarAway_pMag)
		{
			pLrb.drag = 0.0f;
		}
		if (!magR.dangerFarAway_magObj && !magR.dangerFarAway_pMag)
		{
			pRrb.drag = 0.0f;
		}

		// Lの抵抗処理
		if (magL.GetIsResisting())
		{
			pLrb.drag = dangerDrag;
		}
		else
		{
			pLrb.drag = 0.0f;
		}

		// Rの抵抗処理
		if (magR.GetIsResisting())
		{
			pRrb.drag = dangerDrag;
		}
		else
		{
			pRrb.drag = 0.0f;
		}
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
