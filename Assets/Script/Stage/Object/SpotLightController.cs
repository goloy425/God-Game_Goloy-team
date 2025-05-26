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
	[Header("ターゲットの角度を設定(y=0固定！)")]
	public Vector3[] targetAngle;

	[Header("動く速度")]
	public float rotateSpeed = 0.1f;

	[Header("ターゲットに留まる秒数")]
	public float stayTime = 2.0f;

	private float stayTimer = 0.0f;		// 計測用のタイマー
	private int currentNum = 0;

	private float currentX;
	private float targetX;
	private float deltaX;
	private bool moveX;

	private float currentZ;
	private float targetZ;
	private float deltaZ;
	private bool moveZ;

	void Update()
	{
		if (targetAngle == null) return;

		stayTimer += Time.deltaTime;

		if (targetAngle[currentNum].x != 0.0f)
		{
			currentX = transform.localEulerAngles.x;
			targetX = targetAngle[currentNum].x;
			deltaX = Mathf.DeltaAngle(currentX, targetX);
			moveX = true;
		}

		if (targetAngle[currentNum].z != 0.0f)
		{
			currentZ = transform.localEulerAngles.z;
			targetZ = targetAngle[currentNum].z;
			deltaZ = Mathf.DeltaAngle(currentZ, targetZ);
			moveZ = true;
		}

		// 最短の角度差を求める（-180〜180°の範囲で出る）


		// 一定のスピードで回転
		if (moveX && moveZ)
		{
			if (Mathf.Abs(deltaX) > 0.5f && Mathf.Abs(deltaZ) > 0.5f)  // ある程度近づいたら止める
			{
				float stepX = Mathf.Sign(deltaX) * rotateSpeed;
				float stepZ = Mathf.Sign(deltaZ) * rotateSpeed;
				transform.Rotate(stepZ, 0f, stepZ, Space.Self);
			}
			else
			{
				UpdateTimer();
			}
		}
		else if (moveX)
		{
			if (Mathf.Abs(deltaX) > 0.5f)
			{
				float stepX = Mathf.Sign(deltaX) * rotateSpeed;
				transform.Rotate(stepX, 0f, 0f, Space.Self);
			}
			else
			{
				UpdateTimer();
			}
		}
		else if(moveZ)
		{
			if (Mathf.Abs(deltaZ) > 0.5f)
			{
				float stepZ = Mathf.Sign(deltaX) * rotateSpeed;
				transform.Rotate(stepZ, 0f, 0f, Space.Self);
			}
			else
			{
				UpdateTimer();
			}
		}
	}

	private void UpdateTimer()
	{
		// タイマー更新
		stayTimer += Time.deltaTime;

		if (stayTimer > stayTime)
		{
			stayTimer = 0f;
			currentNum = (currentNum + 1) % targetAngle.Length;
		}
	}
}