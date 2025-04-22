using System.Collections;
using System.Collections.Generic;
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
	private Coroutine vibrationCoroutine;
	private AdjustMagnetism adjMag;

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

		if (gamepad == null) { return; }	// コントローラーがない時はスルー
		else	// 起動時に謎の振動が起こるのを抑制
		{
			gamepad.SetMotorSpeeds(0, 0);
			// 今接続されてるコントローラーの種類を出力
			Debug.Log("接続されているコントローラー:"+Gamepad.current.displayName);
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

		if (!notVibration)	// 振動オフじゃない時
		{
			//--- ここに足していけば複数種類の振動をコントロールできる？ ---//
			// プレイヤーの磁石と磁力オブジェクトの距離
			if (magnet1.inObjMagArea || magnet2.inObjMagArea)
			{
				StartCoroutine(Vibration_MagObj());
			}
			// プレイヤーの磁石同士の距離に応じた振動
			else if (magnet1.inPlayerMagArea)
			{
				StartCoroutine(Vibration_Magnets());
			}
		}
	}

	private void FixedUpdate()
	{
		//--- 振動の切り替え ---//
		// プレイヤーの磁石とオブジェクトの方が優先
		if (magnet1.inObjMagArea || magnet2.inObjMagArea)
		{
			StartCoroutine(Vibration_MagObj());
		}
		// プレイヤーの磁石同士の距離に応じた振動
		else if (magnet1.inPlayerMagArea)
		{
			StartCoroutine(Vibration_Magnets());
		}
		else
		{
            gamepad.SetMotorSpeeds(0, 0);	// 振動停止
        }
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

			//--- 数値の幅を変更するならココ ---//
			// 振動の間隔
			float minVibInterval = 0.7f;	// 近い
			float maxVibInterval = 1.5f;	// 遠い

			// 振動の強さ
			float minVibStrength = 0.002f;	// 遠い
			float maxVibStrength = 0.03f;   // 近い

			// Xboxのコントローラーの場合
			if(Gamepad.current.displayName=="Xbox Controller")
			{
				minVibStrength = 1.0f;
				maxVibStrength = 15;
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
			gamepad.SetMotorSpeeds(vibStrength, vibStrength);
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


	void OnDestroy()
	{
		if (gamepad != null)
		{
			gamepad.SetMotorSpeeds(0, 0);	// ゲーム終了時に振動を止める
		}
	}
}
