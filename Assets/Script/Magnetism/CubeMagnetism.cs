using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMagnetism : MonoBehaviour
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

	[Header("くっついてるキューブを設定")]
	public Transform cube1;
	public Transform cube2;

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
		// GameManagerを取得
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void FixedUpdate()
	{
		//--- 磁石の引き寄せ処理 ---//
		foreach (var magnet in registeredMagnets)
		{
			if (magnet == null || magnet.isSnapping) continue;

			Vector3 magnetPos = magnet.myPlate.position;

			// Colliderを利用して一番近い表面の座標を取得
			Vector3 surface1 = cube1.GetComponent<SphereCollider>().ClosestPoint(magnetPos);
			Vector3 surface2 = cube2.GetComponent<SphereCollider>().ClosestPoint(magnetPos);

			// 表面座標との距離を計算
			float distance1 = Vector3.Distance(surface1, magnetPos);
			float distance2 = Vector3.Distance(surface2, magnetPos);

			Vector3 targetSurface = (distance1 < distance2) ? surface1 : surface2;
			float surfaceDistance = Mathf.Min(distance1, distance2);

			if (surfaceDistance > MagnetismRange)
			{
				magnet.inObjMagArea = false;
				continue;
			}

			magnet.inObjMagArea = true;

			Vector3 direction = (targetSurface - magnetPos).normalized;
			float force = (surfaceDistance < deadRange) ? strongMagnetism : magnetism;
			magnet.GetComponent<Rigidbody>().AddForce(direction * force, ForceMode.Acceleration);

			if (surfaceDistance < magnet.snapDistance)
			{
				Rigidbody rb = magnet.GetComponent<Rigidbody>();
				rb.velocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;

				AttachToSurface(magnet, targetSurface);
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

        gameManager.SetGameOverFg(true);    // ゲームオーバーにする
    }
}
