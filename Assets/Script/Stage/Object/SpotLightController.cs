using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//===========================================================
// 作成者：宮本和音
// スポットライトの動き（首振り）を制御するスクリプト
//===========================================================

public class SpotLightController : MonoBehaviour
{
	[Header("ターゲットの角度を2つ設定")]
	public float[] targetAngle;

	[Header("ターゲットに留まる秒数")]
	public float stayTime = 2.0f;

	private float stayTimer = 0f;
	public int currentNum = 0;

	private Quaternion rota;
	private float newRotaX;

    void Update()
	{
        rota = transform.rotation;		// 角度の更新

        if (targetAngle == null) return;

		newRotaX += 0.001f;
		transform.rotation = new Quaternion(newRotaX, rota.y, rota.z, rota.w);
	}
}