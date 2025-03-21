using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 制作者　宮本和音

public class Magnetism : MonoBehaviour
{
	[Header("plate(Magnetの直下)の設定")]
	public Transform myPlate;	// 自分のBottomPlate
	public Transform targetPlate;	// くっつける相手のBottomPlate

	[Header("磁力・範囲の設定")]
	public float magnetismRange = 10.0f;	// 引き寄せ合う距離
	public float deadRange = 1.0f;	// 近づきすぎるとくっつく、の距離
	public float magnetism = 200.0f;	// 磁力
	public float strongMagnetism = 999.0f;	// 近づきすぎた時の磁力（超強）
	public float snapDistance = 0.07f;	// くっつく距離の閾値

	private bool isSnapping = false;	// くっついてるかどうか
	private Rigidbody rb;

	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		isSnapping = false;
	}


	void FixedUpdate()
	{
		// 距離を計算
		float distance = Vector3.Distance(myPlate.position, targetPlate.position);
		Vector3 direction = (targetPlate.position - myPlate.position).normalized;

		if (distance < magnetismRange)	// 磁力範囲内なら引き寄せ
		{
			float force = (distance < deadRange) ? strongMagnetism : magnetism;
			rb.AddForce(direction * force, ForceMode.Acceleration);
		}

		if (distance < snapDistance)	// 接近しすぎるとくっつく
		{
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			AttachToTarget();
			isSnapping = true;
		}
	}

	void AttachToTarget()
	{
		if (isSnapping) return;

		// くっつける（＝FixedJointの作成）
		FixedJoint joint = gameObject.AddComponent<FixedJoint>();
		joint.connectedBody = targetPlate.GetComponentInParent<Rigidbody>();
	}

	// Update is called once per frame
	void Update()
	{
	}
}
