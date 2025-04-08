using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HCubeMagnetism : MonoBehaviour
{
	[Header("磁力・範囲の設定")]
	[SerializeField] private float magnetismRange = 10.0f;
	// ↑こっちのスクリプトでは使ってないけどDrawCircleに使ってるので残しておく
	[SerializeField] private float deadRange = 1.0f;
	public float magnetism = 200.0f;
	public float strongMagnetism = 999.0f;

	public Vector3 magnetismScale = new Vector3(1f, 1.5f, 1f);	// 楕円形
	[SerializeField] private float ellipsoidDistance = 3f;	// 有効範囲の半径っぽいイメージ

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


	private void FixedUpdate()
	{
		//--- 磁石の引き寄せ処理 ---//
		foreach (var magnet in registeredMagnets)
		{
			if (magnet == null || magnet.isSnapping) continue;

			Vector3 magnetPos = magnet.myPlate.position;

			// このキューブ自身のコライダーに対してClosestPointを使う
			Vector3 surface = GetComponent<BoxCollider>().ClosestPoint(magnetPos);
			float distance = Vector3.Distance(surface, magnetPos);

			// --- 楕円形距離による範囲判定 --- //
			// バグったので消してある、修正必須
			//if (!IsWithinEllipsoidRange(magnetPos, surface))
			//{
			//	magnet.inObjMagArea = false;
			//	continue;
			//}

			magnet.inObjMagArea = true;

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

	private bool IsWithinEllipsoidRange(Vector3 magnetPos, Vector3 surface)
	{
		Vector3 diff = magnetPos - surface;

		float scaledX = diff.x / magnetismScale.x;
		float scaledY = diff.y / magnetismScale.y;
		float scaledZ = diff.z / magnetismScale.z;

		float distanceSq = scaledX * scaledX + scaledY * scaledY + scaledZ * scaledZ;

		return distanceSq <= ellipsoidDistance * ellipsoidDistance;
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
