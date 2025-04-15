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

	private GameManager gameManager;

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

		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
	}


	private void FixedUpdate()
	{
		if (registeredMagnets == null || registeredMagnets.Count == 0) return;

		Magnetism nearestMagnet = null;
		float minDistance = float.MaxValue;
		Vector3 surface = Vector3.zero;

		// 一番近い磁石を探す
		foreach (var magnet in registeredMagnets)
		{
			if (magnet == null || magnet.isSnapping) continue;

			Vector3 magnetPos = magnet.myPlate.position;
			Vector3 tempSurface = GetComponent<SphereCollider>().ClosestPoint(magnetPos);
			float distance = Vector3.Distance(tempSurface, magnetPos);

			if (distance < minDistance)
			{
				minDistance = distance;
				nearestMagnet = magnet;
				surface = tempSurface;
			}
		}

		if (nearestMagnet == null) return;

		// 範囲内かどうかチェック（円形判定）
		if (minDistance > magnetismRange)
		{
			nearestMagnet.inObjMagArea = false;
			return;
		}

		nearestMagnet.inObjMagArea = true;

		// 引き寄せ処理
		Vector3 direction = (surface - nearestMagnet.myPlate.position).normalized;
		float force = (minDistance < deadRange) ? strongMagnetism : magnetism;
		nearestMagnet.GetComponent<Rigidbody>().AddForce(direction * force, ForceMode.Acceleration);

		// 吸着処理
		if (minDistance < nearestMagnet.snapDistance)
		{
			Rigidbody rb = nearestMagnet.GetComponent<Rigidbody>();
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;

			AttachToSurface(nearestMagnet, surface);
			nearestMagnet.isSnapping = true;
		}
	}

	private void AttachToSurface(Magnetism magnet, Vector3 snapPosition)
	{
		if (magnet.isSnapping) return;

		FixedJoint joint = magnet.gameObject.AddComponent<FixedJoint>();
		joint.connectedBody = GetComponent<Rigidbody>();

		magnet.myPlate.position = snapPosition;

		magnet.GetComponent<AudioSource>().PlayOneShot(magnet.magnetSE);

		gameManager.SetGameOverFg(true);	// ゲームオーバーにする
	}
}
