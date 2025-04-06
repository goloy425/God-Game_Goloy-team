using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=================================================
// 制作者　宮本和音
// 磁力付きオブジェクトにアタッチするスクリプト
//=================================================


public class SphereMagnetism : MonoBehaviour
{
	[Header("磁力・範囲の設定")]
	public float magnetismRange = 10.0f;
	[SerializeField] private float deadRange = 1.0f;
	public float magnetism = 200.0f;
	public float strongMagnetism = 999.0f;

	public float MagnetismRange => magnetismRange;
	public float DeadRange => deadRange;

	private SphereCollider sphereCollider;  // 磁石を引き寄せる頂点の計算に使う

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
		sphereCollider = GetComponent<SphereCollider>();
		if (sphereCollider == null)
		{
			Debug.LogError("SphereColliderついてないよ");
		}
	}

	private void FixedUpdate()
	{
		//--- 磁石の引き寄せ処理 ---//
		foreach (var magnet in registeredMagnets)
		{
			if (magnet == null || magnet.isSnapping) continue;

			Vector3 magnetPos = magnet.myPlate.position;
			// ↑とりあえず現段階磁力を含んだオブジェクトはプレイヤーの磁石しかないのでmyPlateのPositionにしてある
			Vector3 center = transform.position;
			Vector3 directionToMagnet = (magnetPos - center).normalized;

			float radius = sphereCollider.radius * transform.localScale.x;
			Vector3 surfacePoint = center + directionToMagnet * radius;

			// 球の表面と磁石の距離を計算
			float surfaceDistance = Vector3.Distance(surfacePoint, magnetPos);
			if (surfaceDistance > magnetismRange) continue;	// 磁力範囲外ならスキップ

			// 引き寄せる処理
			Vector3 direction = (surfacePoint - magnetPos).normalized;
			float force = (surfaceDistance < deadRange) ? strongMagnetism : magnetism;
			magnet.GetComponent<Rigidbody>().AddForce(direction * force, ForceMode.Acceleration);

			// 近づきすぎるとくっつく
			if (surfaceDistance < magnet.snapDistance)
			{
				Rigidbody rb = magnet.GetComponent<Rigidbody>();
				rb.velocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
				AttachToSurface(magnet);

				magnet.isSnapping = true;
			}
		}
	}

	void AttachToSurface(Magnetism magnet)
	{
		if (magnet.isSnapping) return;

		// くっつける（＝FixedJointの作成）
		FixedJoint joint = magnet.gameObject.AddComponent<FixedJoint>();
		joint.connectedBody = GetComponent<Rigidbody>();

		// 位置合わせ
		magnet.myPlate.position = transform.position;

		magnet.GetComponent<AudioSource>().PlayOneShot(magnet.magnetSE);
	}
}
