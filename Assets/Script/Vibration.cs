using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// 制作者　宮本和音

// ↓振動の強さを調整する時の目安
// 0.002が最小値 それ以上低くすると振動しない（コントローラーによって差あり？分からん）
// 1.0が最大値（たぶん）だがか～～なり強かったのでもっと数値小さい方がちょうどいい、というか手がしんどくない

public class Vibration : MonoBehaviour
{
	[Header("プレイヤーの磁石(順不同)")]
	public Magnetism magnet1;
	public Magnetism magnet2;

	[Header("チェックで振動オフ")]
	public bool notVibration = false;   // デバッグ中振動がうざくなったらチェック

	private Gamepad gamepad;
	private Coroutine vibrationCoroutine;

	// Start is called before the first frame update
	void Start()
	{
		gamepad = Gamepad.current;

		if (gamepad == null) { return; }		// コントローラーがない時はスルー
		else { gamepad.SetMotorSpeeds(0, 0); }	// 起動時に謎の振動が起こるのを抑制

		//--- ここに足していけば複数の振動をコントロールできる？ ---//
		if (!notVibration)
		{
			vibrationCoroutine = StartCoroutine(Vibration_MagnetDistance());	// 磁石の距離に応じた振動
		}
	}


	//=================================================
	// 磁石の距離に応じて振動させる関数
	//=================================================
	IEnumerator Vibration_MagnetDistance()
	{
		while (true)
		{
			float distance = Vector3.Distance(magnet1.transform.position, magnet2.transform.position);	// 磁石の距離

			float minDistance = magnet1.DeadRange;
			float maxDistance = magnet1.MagnetismRange;
			float vibStrength;  // 振動の強さ
			float vibInterval;  // 振動の間隔

			//--- 数値の幅を変更するならココ ---//
			// 振動の強さ
			float minVibStrength = 0.002f;	// 遠い
			float maxVibStrength = 0.03f;	// 近い

			// 振動の間隔
			float minVibInterval = 0.7f;	// 近い
			float maxVibInterval = 1.5f;	// 遠い


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

	void OnDestroy()
	{
		if (gamepad != null)
		{
			gamepad.SetMotorSpeeds(0, 0);	// ゲーム終了時に振動を止める
		}
	}
}
