using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HCubeMagnetism : MonoBehaviour
{
	[Header("磁力・範囲の設定")]
	[SerializeField] private float magnetismRange = 10.0f;
	[SerializeField] private float deadRange = 1.0f;
	public float magnetism = 200.0f;
	public float strongMagnetism = 999.0f;

	public float MagnetismRange => magnetismRange;
	public float DeadRange => deadRange;

	[Header("プレイヤーオブジェクトを設定")]
	public GameObject playerL;
	public GameObject playerR;

	//--- 磁石のリスト管理 ---//
	private static List<Magnetism> registeredMagnets = new();

	public static void Register(Magnetism magnet)
	{
		if (!registeredMagnets.Contains(magnet))
		{
			registeredMagnets.Add(magnet);
		}
	}
	public static void Unregister(Magnetism magnet)
	{
		registeredMagnets.Remove(magnet);
	}


	private void Start()
	{
		// 半分になる前は非アクティブにしておく
		enabled = false;
	}


	private void FixedUpdate()
	{
		//--- 磁石の引き寄せ処理 ---//
		foreach (var magnet in registeredMagnets)
		{
			if (magnet == null || magnet.isSnapping) continue;

			Vector3 magnetPos = magnet.myPlate.position;

			if (!magnet.inObjMagArea) continue;

			// このキューブ自身のコライダーに対してClosestPointを使う
			Vector3 surface = GetComponent<SphereCollider>().ClosestPoint(magnetPos);
			float distance = Vector3.Distance(surface, magnetPos);

			Vector3 direction = (surface - magnetPos).normalized;
			float force = (distance < deadRange) ? strongMagnetism : magnetism;
			magnet.GetComponent<Rigidbody>().AddForce(direction * force, ForceMode.Acceleration);

			if (distance < magnet.snapDistance)
			{
				Rigidbody rb = magnet.GetComponent<Rigidbody>();
				rb.velocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;

				AttachToSurface(magnet, surface);
				magnet.isSnapping = true;
			}
		}
	}

	private void AttachToSurface(Magnetism magnet, Vector3 snapPosition)
	{
		if (magnet.isSnapping) return;

		FixedJoint joint = magnet.gameObject.AddComponent<FixedJoint>();
		joint.connectedBody = GetComponent<Rigidbody>();

		magnet.myPlate.position = snapPosition;

		magnet.GetComponent<AudioSource>().PlayOneShot(magnet.magnetSE);
	}
}
