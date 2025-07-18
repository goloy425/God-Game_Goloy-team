using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//===========================================================================
// 作成者：宮本和音
// 磁石同士or磁石とオブジェクトが近づきすぎた時にスローにするスクリプト
//===========================================================================

public class TooClose : MonoBehaviour
{
	[Header("GameManagerを設定")]
	public GameManager gm;

	[Header("プレイヤーの磁石を設定")]
	public Magnetism magnet1;
	public Magnetism magnet2;

	[Header("Plateを設定")]
	public GameObject plate1;
	public GameObject plate2;

	private float dangerDist;   // 危険距離
	private float safetyDist;   //  ↑ から脱する距離

	// プレイヤー用
	private float pDangerDist;
	private float pSafetyDist;

	// Start is called before the first frame update
	void Start()
	{
		// 各距離を設定
		dangerDist = magnet1.deadRange + 0.1f;
		safetyDist = dangerDist + 0.2f;

		pDangerDist = magnet1.deadRange + 0.5f;
		pSafetyDist = pDangerDist + 0.2f;
	}

	// Update is called once per frame
	void Update()
	{
		if (magnet1.isSlow_pMag || magnet2.isSlow_pMag || magnet1.isSlow_magObj || magnet2.isSlow_magObj)
		{
			StartCoroutine(TriggerSlowMotionEffect());
		}
		else
		{
			StopCoroutine(TriggerSlowMotionEffect());
		}
	}

	//--- スローモーション切替 ---//
	IEnumerator TriggerSlowMotionEffect()
	{
		// スローモーションにする
		Time.timeScale = 0.2f;
		Time.fixedDeltaTime = 0.02f * Time.timeScale;

		// 安全な距離まで離れるかくっつくかするまでスロー継続
		while ((magnet1.isSlow_pMag || magnet2.isSlow_pMag || magnet1.isSlow_magObj || magnet2.isSlow_magObj) &&
			   !magnet1.isSnapping && !magnet2.isSnapping)
		{
			yield return null;      // 次のフレームまで待つ
		}

		// スロー解除
		Time.timeScale = 1f;
		Time.fixedDeltaTime = 0.02f;
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

	public float GetPDangerDist()
	{
		return pDangerDist;
	}
	public float GetPSafetyDist()
	{
		return pSafetyDist;
	}
}
