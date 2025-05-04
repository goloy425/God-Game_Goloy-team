using System.Collections;
using System.Collections.Generic;
//using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

//=================================================
// 制作者　宮本和音
//=================================================


public class Vibration : MonoBehaviour
{
	[Header("プレイヤーの磁石(順不同)")]
	public Magnetism magnet1;
	public Magnetism magnet2;

	//[Header("磁力オブジェクトを登録")]
	//public GameObject[] magObjs;

	[Header("チェックで振動オフ")]
	public bool notVibration = false;   // デバッグ中振動がうざくなったらチェック

	private Gamepad gamepad;
	private AdjustMagnetism adjMag;

	// 磁石がくっついたり離れたりしたら振動オフにする用のフラグ
	private bool gameOver = false;

	// 強化中かどうか取得する用
	private AugMagL playerL;
	private AugMagR playerR;

	// 強化した瞬間だけ振動させる用のフラグ
	private bool vibratedL = false;
	private bool vibratedR = false;

	private bool isVibratingL = false;
	private bool isVibratingR = false;

	private bool prevAugL = false;
	private bool prevAugR = false;

	//// 磁力スクリプトの取得用
	//private SphereMagnetism[] sMag;
	//private CubeMagnetism[] cMag;
	//private HCubeMagnetism[] hcMag;

	//// 要素数管理
	//private int sMagCnt = 0;
	//private int cMagCnt = 0;
	//private int hcMagCnt = 0;

	private AudioSource audiosource;
	public bool playing = false;

	// Start is called before the first frame update
	void Start()
	{
		gamepad = Gamepad.current;
		adjMag = GameObject.Find("Main Camera").GetComponent<AdjustMagnetism>();

		playerL = GameObject.Find("PlayerL_Controller").GetComponent<AugMagL>();
		playerR = GameObject.Find("PlayerR_Controller").GetComponent<AugMagR>();

		audiosource = magnet1.GetComponent<AudioSource>();

		if (gamepad == null) { return; }	// コントローラーがない時はスルー
		else	// 起動時に謎の振動が起こるのを抑制
		{
			gamepad.SetMotorSpeeds(0, 0);
			// 今接続されてるコントローラーの種類を出力
			Debug.Log("接続されているコントローラー:"+gamepad.displayName);
		}

		//// シーン内にある磁力オブジェクトを格納していく
		//foreach (GameObject obj in magObjs)
		//{
		//	if (gameObject.CompareTag("MagObj_Sphere"))
		//	{
		//		TryGetComponent<SphereMagnetism>(out sMag[sMagCnt]);
		//		sMagCnt++;
		//	}
		//	else if (gameObject.CompareTag("MagObj_Cube"))
		//	{
		//		TryGetComponent<CubeMagnetism>(out cMag[cMagCnt]);
		//		cMagCnt++;
		//	}
		//	else if (gameObject.CompareTag("MagObj_HCube"))
		//	{
		//		TryGetComponent<HCubeMagnetism>(out hcMag[hcMagCnt]);
		//		hcMagCnt++;
		//	}
		//}

		if (!notVibration)
		{
			// 磁石の距離に応じた振動
			if (gamepad.displayName == "Xbox Controller")	// Xbox
			{
				StartCoroutine(VibMagnets_X());
			}
			else	// その他（今のところPS5のみ）
			{
				StartCoroutine(VibMagnets());
			}
		}
	}

	private void Update()
	{
		if (notVibration) { return; }   // 振動無しの時はスキップ

		if (audiosource.isPlaying) { playing = true; }
		else { playing = false; }

		if (gameOver)	// ゲームオーバー時は継続する振動をオフ
		{
			gamepad.SetMotorSpeeds(0, 0);
			return;
		}

		// 磁石が磁力エリアから離れた時
		if (!magnet1.inMagnetismArea || !magnet2.inMagnetismArea)
		{
			gameOver = true;
			return;
		}

		// 磁石が何かしらにくっついた時
		if (!gameOver && (magnet1.isSnapping || magnet2.isSnapping))
		{
			StartCoroutine(VibAttached());
			return;
		}


		//--- 強化時の振動周りの処理 ---//
		// 強化状態の更新
		bool nowAugL = playerL.isAugmenting;
		bool nowAugR = playerR.isAugmenting;

		// PlayerLが強化状態になったら
		if (nowAugL && !prevAugL && !isVibratingL)
		{
			vibratedL = true;
			isVibratingL = true;

			if (gamepad.displayName == "Xbox Controller")
			{
				StartCoroutine(VibAugment_X(0.1f, 0.1f, 0.1f, true));
			}
			else
			{
				StartCoroutine(VibAugment(true));
			}
		}

		// PlayerRが強化状態になったら
		if (nowAugR && !prevAugR && !isVibratingR)
		{
			vibratedR = true;
			isVibratingR = true;

			if (gamepad.displayName == "Xbox Controller")
			{
				StartCoroutine(VibAugment_X(0.1f, 0.1f, 0.1f, false));
			}
			else
			{
				StartCoroutine(VibAugment(false));
			}
		}

		// 振動済みの状態でキーが離されたら未振動に戻す
		if (vibratedL && !isVibratingL)
		{
			vibratedL = false;
		}
		if (vibratedR && !isVibratingR)
		{
			vibratedR = false;
		}

		// フラグの更新
		prevAugL = nowAugL;
		prevAugR = nowAugR;
	}


	//=========================================================
	// 基本コントローラー（とりあえずPS5をこっちにしておく）
	//=========================================================
	// ① 磁石の距離に応じた振動
	IEnumerator VibMagnets()
	{
		while (true)
		{
			// adjustedがfalseに戻るまで停止
			while (adjMag.Adjusted)
			{
				gamepad.SetMotorSpeeds(0.0f, 0.0f); // 振動をオフ
				yield return null;	// 次のフレームまで待つ
			}

			float distance = Vector3.Distance(magnet1.transform.position, magnet2.transform.position);  // 磁石の距離

			float minDistance = magnet1.deadRange;
			float maxDistance = magnet1.magnetismRange + 2.0f;
			float vibStrength;	// 振動の強さ
			float vibInterval;  // 振動の間隔

			//--- 数値の幅を変更するならココ ---//
			// 振動の間隔
			float minVibInterval = 0.5f;	// 近い
			float maxVibInterval = 1.5f;	// 遠い

			// 振動の強さ
			float minVibStrength = 0.001f;	// 遠い
			float maxVibStrength = 0.03f;   // 近い


			if (distance <= minDistance)	// 近すぎる→最大振動
			{
				vibStrength = maxVibStrength;
				vibInterval = minVibInterval;

				if (magnet1.isSnapping) // くっついたら振動切る
				{
					vibStrength = 0.0f;
				}
			}
			else if (distance >= maxDistance)	// 遠すぎる→振動なし
			{
				vibStrength = 0.0f;
				vibInterval = 0.0f;
			}
			else	// 範囲内→距離に応じて振動強度と間隔を変える
			{
				float t = (distance - minDistance) / (maxDistance - minDistance);
				vibStrength = Mathf.Lerp(maxVibStrength, minVibStrength, t);
				vibInterval = Mathf.Lerp(minVibInterval, maxVibInterval, t);
			}

			// コントローラーを振動させる
			gamepad.SetMotorSpeeds(vibStrength, vibStrength);
			yield return new WaitForSeconds(0.05f);
			gamepad.SetMotorSpeeds(0.0f, 0.0f);
			yield return new WaitForSeconds(vibInterval);
		}
	}

	// ② 磁力強化時
	IEnumerator VibAugment(bool isL)
	{
		// 1回目の振動
		gamepad.SetMotorSpeeds(0.02f, 0.02f);
		yield return new WaitForSeconds(0.05f);

		gamepad.SetMotorSpeeds(0, 0);
		yield return new WaitForSeconds(0.1f);	// 間

		// 2回目の振動
		gamepad.SetMotorSpeeds(0.02f, 0.02f);
		yield return new WaitForSeconds(0.05f);

		gamepad.SetMotorSpeeds(0, 0);	// 念のため停止

		if (isL)
		{
			isVibratingL = false;
		}
		else
		{
			isVibratingR = false;
		}
	}

	// ③ 磁石がくっついた時の振動
	IEnumerator VibAttached()
	{
		gamepad.SetMotorSpeeds(0.5f, 0.5f);
		yield return new WaitForSeconds(0.06f);

		gameOver = true;
		yield return null;  // 次のフレームまで待つ
	}


	//=================================================
	// Xboxコントローラー
	//=================================================
	// ① 磁石の距離に応じた振動
	IEnumerator VibMagnets_X()
	{
		while (true)
		{
			// adjustedがfalseに戻るまで停止
			while (adjMag.Adjusted)
			{
				gamepad.SetMotorSpeeds(0.0f, 0.0f); // 振動をオフ
				yield return null;  // 次のフレームまで待つ
			}

			float distance = Vector3.Distance(magnet1.transform.position, magnet2.transform.position);  // 磁石の距離

			float minDistance = magnet1.deadRange;
			float maxDistance = magnet1.magnetismRange + 2.0f;
			float vibStrength;  // 振動の強さ
			float vibInterval;  // 振動の間隔

			//--- 数値の幅を変更するならココ ---//
			// 振動の間隔
			float minVibInterval = 0.5f;	// 近い
			float maxVibInterval = 1.5f;	// 遠い

			// 振動の強さ
			float minVibStrength = 0.1f;
			float maxVibStrength = 3.0f;

			if (distance <= minDistance)	// 近すぎる→最大振動
			{
				vibStrength = maxVibStrength;
				vibInterval = minVibInterval;

				if (magnet1.isSnapping) // くっついたら振動切る
				{
					vibStrength = 0.0f;
				}
			}
			else if (distance >= maxDistance)   // 遠すぎる→振動なし
			{
				vibStrength = 0.0f;
				vibInterval = 0.0f;
			}
			else    // 範囲内→距離に応じて振動強度と間隔を変える
			{
				float t = (distance - minDistance) / (maxDistance - minDistance);
				vibStrength = Mathf.Lerp(maxVibStrength, minVibStrength, t);
				vibInterval = Mathf.Lerp(minVibInterval, maxVibInterval, t);
			}

			// コントローラーを振動させる
			gamepad.SetMotorSpeeds(vibStrength, vibStrength);
			yield return new WaitForSeconds(0.1f);
			gamepad.SetMotorSpeeds(0.0f, 0.0f);
			yield return new WaitForSeconds(vibInterval);
		}
	}

	// ② 磁力強化時
	IEnumerator VibAugment_X(float targetLow, float targetHigh, float duration, bool isL)
	{
		float timer = 0f;
		float startLow = 0f;
		float startHigh = 0f;

		// 段階的に振動を強くする
		while (timer < duration)
		{
			timer += Time.deltaTime;
			float t = timer / duration;

			float currentLow = Mathf.Lerp(startLow, targetLow, t);
			float currentHigh = Mathf.Lerp(startHigh, targetHigh, t);
			gamepad.SetMotorSpeeds(currentLow, currentHigh);

			yield return null;
		}

		// ちょっと余韻
		yield return new WaitForSeconds(0.1f);

		// 緩やかに振動を戻す
		timer = 0f;
		while (timer < duration)
		{
			timer += Time.deltaTime;
			float t = timer / duration;

			float currentLow = Mathf.Lerp(targetLow, 0f, t);
			float currentHigh = Mathf.Lerp(targetHigh, 0f, t);
			gamepad.SetMotorSpeeds(currentLow, currentHigh);

			yield return null;
		}

		gamepad.SetMotorSpeeds(0, 0);   // 念のため完全停止

		if (isL)
		{
			isVibratingL = false;
		}
		else
		{
			isVibratingR = false;
		}
	}


	//================================================================
	// プレイヤーの磁石と磁力オブジェクトの距離に応じて振動させる関数
	//================================================================
	//IEnumerator Vibration_MagObj()
	//{
	//	yield return new WaitForSeconds(0);
	//}


	void OnDestroy()
	{
		if (gamepad != null)
		{
			gamepad.SetMotorSpeeds(0, 0);   // ゲーム終了時に振動を止める
			gameOver = false;
		}
	}
}
