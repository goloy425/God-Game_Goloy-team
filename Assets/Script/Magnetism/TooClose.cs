using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=============================================================
// 作成者：宮本和音
// 磁石が近づきすぎた時にスローモーションになるやつ（テスト）
//=============================================================

public class TooClose : MonoBehaviour
{
	[Header("プレイヤーの磁石を設定")]
	public Magnetism magnet1;
	public Magnetism magnet2;

	[Header("Plateを設定")]
	public GameObject plate1;
	public GameObject plate2;

	private float distance;	// 磁石同士の距離
	private bool isSlow;	// スローモーションかどうか

	//private float dangerDistance;	// これ以上近づきすぎるとヤバい！の距離
	//private float safetyDistance;	// スローモーションを解除する距離
	
	// Start is called before the first frame update
	void Start()
	{

	}


	private void FixedUpdate()
	{
		// 危険距離に入ったらスローモーション
		if ((magnet1.inDangerZone || (magnet1.inDangerZone_obj || magnet2.inDangerZone)) && !isSlow)
		{
			StartCoroutine(TriggerSlowMotionEffect());
		}
	}

	//--- スローモーションの処理 ---//
	IEnumerator TriggerSlowMotionEffect()
	{
		isSlow = true;

		// スローモーション
		Time.timeScale = 0.2f;
		Time.fixedDeltaTime = 0.02f * Time.timeScale;

		// 磁石の揺れで短時間の間にスローが入ったり解けたりするのを防ぐ
		if (!magnet1.isSnapping && !magnet2.isSnapping &&
			magnet1.inMagnetismArea && magnet2.inMagnetismArea &&
			magnet1.inObjMagArea && magnet2.inObjMagArea)
		{
			yield return new WaitForSeconds(0.15f);
		}

		// 距離が安全になるか磁石がくっつくまで待つ（リアルタイムで監視）
		while (!magnet1.inSafeZone && !magnet2.inSafeZone && !magnet1.isSnapping && !magnet2.isSnapping)
		{
			yield return null;  // 次のフレームまで待機
		}

		// スローモーション解除
		Time.timeScale = 1f;
		Time.fixedDeltaTime = 0.02f;

		isSlow = false;
	}

	//--- スローモーションかどうかを返す関数 ---//
	public bool GetIsSlow()
	{
		return isSlow;
	}
}
