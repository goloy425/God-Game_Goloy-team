using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

//=================================================
// 制作者　宮本和音
//=================================================

// ↓振動の強さを調整する時の目安
// 0.002が最小値 それ以上低くすると振動しない（コントローラーによって差あり？分からん）
// 1.0が最大値（たぶん）だがか～～なり強かったのでもっと数値小さい方がちょうどいい、というか手がしんどくない

// 3/29追記　コントローラーによって同じ数値でも振動の強さに差がある可能性が高い
// 授業内で皆で作業できるようになったら手持ちのコントローラー持ち寄って数値調整するのがいいかも

public class Vibration : MonoBehaviour
{
	[Header("プレイヤーの磁石(順不同)")]
	public Magnetism magnet1;
	public Magnetism magnet2;

	[Header("磁力オブジェクトを登録")]
	public GameObject[] magObjs;

	[Header("チェックで振動オフ")]
	public bool notVibration = false;   // デバッグ中振動がうざくなったらチェック

	private Gamepad gamepad;
	private AdjustMagnetism adjMag;

	// 強化中かどうか取得する用
	private AugMagL playerL;
	private AugMagR playerR;

	// 強化した瞬間だけ振動させる用のフラグ
	public bool vibratedL = false;
	public bool vibratedR = false;

	public bool isVibratingL = false;
	public bool isVibratingR = false;

	public bool prevAugL = false;	
	public bool prevAugR = false;

	Coroutine vibR = null;

	// 磁力スクリプトの取得用
	private SphereMagnetism[] sMag;
	private CubeMagnetism[] cMag;
	private HCubeMagnetism[] hcMag;

	// 要素数管理
	private int sMagCnt = 0;
	private int cMagCnt = 0;
	private int hcMagCnt = 0;

	// Start is called before the first frame update
	void Start()
	{
		gamepad = Gamepad.current;
		adjMag = GameObject.Find("Main Camera").GetComponent<AdjustMagnetism>();

		playerL = GameObject.Find("PlayerL_Controller").GetComponent<AugMagL>();
		playerR = GameObject.Find("PlayerR_Controller").GetComponent<AugMagR>();

		if (gamepad == null) { return; }	// コントローラーがない時はスルー
		else	// 起動時に謎の振動が起こるのを抑制
		{
			gamepad.SetMotorSpeeds(0, 0);
			// 今接続されてるコントローラーの種類を出力
			Debug.Log("接続されているコントローラー:"+gamepad.displayName);
		}

		// シーン内にある磁力オブジェクトを格納していく
		foreach (GameObject obj in magObjs)
		{
			if (gameObject.CompareTag("MagObj_Sphere"))
			{
				TryGetComponent<SphereMagnetism>(out sMag[sMagCnt]);
				sMagCnt++;
			}
			else if (gameObject.CompareTag("MagObj_Cube"))
			{
				TryGetComponent<CubeMagnetism>(out cMag[cMagCnt]);
				cMagCnt++;
			}
			else if (gameObject.CompareTag("MagObj_HCube"))
			{
				TryGetComponent<HCubeMagnetism>(out hcMag[hcMagCnt]);
				hcMagCnt++;
			}
		}

		if (!notVibration)
		{
			//--- ここに足していけば複数の振動をコントロールできる？ ---//
			StartCoroutine(Vibration_Magnets());	// 磁石の距離に応じた振動
		}
	}

	private void Update()
	{
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
				StartCoroutine(Vibration_Augment(0.08f, 0.08f, 0.2f, true));
			}
			else
			{
				vibR = StartCoroutine(Vibration_Augment(0.01f, 0.01f, 0.2f, true));
			}
		}

		// PlayerRが強化状態になったら
		if (nowAugR && !prevAugR && !isVibratingR)
		{
			vibratedR = true;
			isVibratingR = true;

			if (gamepad.displayName == "Xbox Controller")
			{
				StartCoroutine(Vibration_Augment(0.08f, 0.08f, 0.2f, false));
			}
			else
			{
				StartCoroutine(Vibration_Augment(0.01f, 0.01f, 0.2f, false));
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

	//=================================================
	// 磁石の距離に応じて振動させる関数
	//=================================================
	IEnumerator Vibration_Magnets()
	{
		while (true)
		{
			// adjustedがfalseに戻るまで停止
			while (adjMag.Adjusted)
			{
				gamepad.SetMotorSpeeds(0.0f, 0.0f); // 振動をオフ
				yield return null;	// 次のフレームまで待つ
			}

			float distance = Vector3.Distance(magnet1.transform.position, magnet2.transform.position);	// 磁石の距離

			float minDistance = magnet1.deadRange;
			float maxDistance = magnet1.magnetismRange;
			float vibStrength;	// 振動の強さ
			float vibInterval;  // 振動の間隔

			float vibAdjust = 0.0f;		// XboxController

			//--- 数値の幅を変更するならココ ---//
			// 振動の間隔
			float minVibInterval = 0.7f;	// 近い
			float maxVibInterval = 1.5f;	// 遠い

			// 振動の強さ
			float minVibStrength = 0.002f;	// 遠い
			float maxVibStrength = 0.03f;   // 近い

			// Xboxのコントローラーの場合振動を強める
			// 他にも振動弱いコントローラーあったらコンソールで名前見てここに追加していくとよさそう
			if (gamepad.displayName == "Xbox Controller")
			{
				minVibStrength *= 100;
				maxVibStrength *= 100;
				vibAdjust = 2.0f;
			}

			if (distance <= minDistance)	// 近すぎる→最大振動
			{
				vibStrength = maxVibStrength;
				vibInterval = minVibInterval;

				if (magnet1.GetComponent<Magnetism>().isSnapping)	// くっついたら振動切る
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
			gamepad.SetMotorSpeeds(vibStrength, vibStrength + vibAdjust);
			yield return new WaitForSeconds(0.05f);
			gamepad.SetMotorSpeeds(0.0f, 0.0f);
			yield return new WaitForSeconds(vibInterval);
		}
	}


	//================================================================
	// プレイヤーの磁石と磁力オブジェクトの距離に応じて振動させる関数
	//================================================================
	IEnumerator Vibration_MagObj()
	{
		yield return new WaitForSeconds(0);		// とりあえずのやつ
	}


	//=================================================
	// 磁力強化時の振動
	//=================================================
	IEnumerator Vibration_Augment(float targetLow, float targetHigh, float duration, bool isL)
	{
		Debug.Log("振動開始");

		float timer = 0f;
		float startLow = 0f;
		float startHigh = 0f;

		// 段階的に振動強くする
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

		gamepad.SetMotorSpeeds(0, 0);	// 念のため完全停止

		if(isL)
		{
			isVibratingL = false;
			Debug.Log("L側の振動を切ったよ");
		}
		else
		{
			isVibratingR = false;
			Debug.Log("R側の振動を切ったよ");
		}

	}


	void OnDestroy()
	{
		if (gamepad != null)
		{
			gamepad.SetMotorSpeeds(0, 0);	// ゲーム終了時に振動を止める
		}
	}
}
