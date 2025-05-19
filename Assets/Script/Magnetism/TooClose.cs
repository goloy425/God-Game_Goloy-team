using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//=====================================================================
// 作成者：宮本和音
// 磁石と磁石、磁石と磁力オブジェクトの近づきすぎを検知するスクリプト
//=====================================================================

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

	public bool stopSlow = false;

	private float dangerDist;	// これ以上近づきすぎるとヤバい！の距離
	private float safetyDist;	// スローモーションを解除する距離、dangerDistnのちょっと上を設定する

	// Start is called before the first frame update
	void Start()
	{
		// 各距離の設定
		dangerDist = magnet1.deadRange + 1.0f;
		safetyDist = dangerDist + 0.3f;
	}

	// Update is called once per frame
	void Update()
	{
		if (magnet1.isSlow || magnet2.isSlow)
		{
			StartCoroutine(TriggerSlowMotionEffect());
		}
		else
		{
			StopCoroutine(TriggerSlowMotionEffect());
		}
	}

	////--- プレイヤーの磁石対オブジェクトの接近判定 ---//
	//private void CheckTooClose()
	//{
	//	// 距離の計測
	//	float dist = Vector3.Distance(plate1.transform.position, plate2.transform.position);

	//	if (dist <= dangerDist)
	//	{
	//		magnet1.isSlow = true;
	//	}

	//	if (magnet1.isSlow && dist > safetyDist)
	//	{
	//		magnet1.isSlow = false;
	//	}

	//	// 各種類の磁力スクリプト（今いるステージ）を取得
	//	var sphereList = gm.GetCurrentStageSphereMagnetisms();
	//	var split1List = gm.GetCurrentStageSplit1Magnetisms();
	//	var split2List = gm.GetCurrentStageSplit2Magnetisms();
	//	var connecterList = gm.GetCurrentStageConnecterMagnetisms();

	//	//--- 距離の計測 ---//
	//	// 球
	//	foreach (var mag in sphereList)
	//	{
	//		// ステージ内に該当オブジェクトが無い、もしくはスクリプトが有効でない場合スルーする
	//		if (mag == null || !mag.enabled) continue;

	//		// 距離の計測
	//		dist1 = Vector3.Distance(plate1.transform.position, mag.transform.position);
	//		dist2 = Vector3.Distance(plate2.transform.position, mag.transform.position);

	//		// 危険距離内に入ったら
	//		if (dist1 <= dangerDist || dist2 <= dangerDist)
	//		{
	//			if (dist1 <= dangerDist) { magnet1.isSlow = true; }	// magnet1
	//			if (dist2 <= dangerDist) { magnet2.isSlow = true; }	// magnet2
	//		}
	//		// 危険距離内にいる状態で安全距離まで離れたら
	//		else if ((magnet1.isSlow && dist1 > safetyDist) || (magnet2.isSlow && dist2 > safetyDist))
	//		{
	//			if (magnet1.isSlow) { magnet1.isSlow = false; }			// magnet1
	//			else if (magnet2.isSlow) { magnet2.isSlow = false; }	// magnet2
	//		}
	//	}

	//	// 2つに分かれるやつ(1)
	//	foreach (var mag in split1List)
	//	{
	//		if (mag == null || !mag.enabled) continue;

	//		dist1 = Vector3.Distance(plate1.transform.position, mag.transform.position);

	//		if (dist1 < dangerDist)
	//		{
	//			Debug.Log("近すぎ（split1Magnetism）");
	//		}
	//	}

	//	// 2つに分かれるやつ(1)
	//	foreach (var mag in split2List)
	//	{
	//		if (mag == null || !mag.enabled) continue;

	//		dist2 = Vector3.Distance(plate2.transform.position, mag.transform.position);

	//		if (dist2 < dangerDist)
	//		{
	//			Debug.Log("近すぎ（split2Magnetism）");
	//			// ゲームオーバー処理や警告など
	//		}
	//	}

	//	// 2つに分かれる前のやつ
	//	foreach (var mag in connecterList)
	//	{
	//		if (mag == null || !mag.enabled) continue;

	//		dist1 = Vector3.Distance(plate1.transform.position, mag.transform.position);
	//		dist2 = Vector3.Distance(plate2.transform.position, mag.transform.position);

	//		if (dist1 < dangerDist || dist2 < dangerDist)
	//		{
	//			Debug.Log("近すぎ（connecterMagnetism）");
	//			// ゲームオーバー処理や警告など
	//		}
	//	}
	//}

	//--- スローモーションの処理 ---//
	IEnumerator TriggerSlowMotionEffect()
	{
		// スローモーション
		Time.timeScale = 0.2f;
		Time.fixedDeltaTime = 0.02f * Time.timeScale;

		// 距離が安全になるか磁石がくっつくまで待つ（リアルタイムで監視）
		while ((magnet1.isSlow || magnet2.isSlow) &&
			   !magnet1.isSnapping && !magnet2.isSnapping)
		{
			yield return null;  // 次のフレームまで待機
		}

		// スローモーション解除
		Time.timeScale = 1f;
		Time.fixedDeltaTime = 0.02f;

		stopSlow = false;
	}

	//--- 危険距離・安全距離のゲッター ---//
	public float GetDangerDist()
	{
		return dangerDist;
	}
	public float GetSafetyDist()
	{
		return safetyDist;
	}
}
