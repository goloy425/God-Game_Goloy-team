using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMagnetism : MonoBehaviour
{
	[Header("磁力・範囲の設定(MagRange TooFarAwayで使用中)")]
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

	// くっついているキューブの各コライダー
	private SphereCollider cube1Collider;
	private SphereCollider cube2Collider;

	//--- 磁石のリスト管理 ---//
	private static List<Magnetism> registeredMagnets = new();

	private TooClose tooClose;
	private TooFarAway tooFarAway;

	private GameObject cubeMagUI;      // 磁力オブジェクトの状態のUI
	private GameObject magNormal;      // 通常
	private GameObject magCaution;     // 警告

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
		// コライダーを取得
		cube1Collider = cube1.GetComponent<SphereCollider>();
		cube2Collider = cube2.GetComponent<SphereCollider>();

		tooClose = GameObject.Find("DistanceManager").GetComponent<TooClose>();
		tooFarAway = GameObject.Find("DistanceManager").GetComponent<TooFarAway>();

		// 磁力オブジェクトの状態UIを取得
		cubeMagUI = transform.Find("MachineUI").transform.Find("Ring").gameObject;
		magNormal = cubeMagUI.transform.Find("Normal").gameObject;
		magCaution = cubeMagUI.transform.Find("Caution").gameObject;
	}

	private void FixedUpdate()
	{
		// スクリプトが無効時、UIを非表示
		if (enabled) { cubeMagUI.SetActive(true); }
		else { cubeMagUI.SetActive(false); }

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

			// 磁力範囲外の場合移以降の処理をスルー
			if (surfaceDistance > MagnetismRange)
			{
				magnet.inObjMagArea = false;
				continue;
			}

			magnet.inObjMagArea = true;

			// 引き寄せ処理
			Vector3 direction = (targetSurface - magnetPos).normalized;
			float force = (surfaceDistance < deadRange) ? strongMagnetism : magnetism;

			// 時間補正倍率（スロー中だけ強めに引き寄せ）
			float timeScaleFactor = Time.timeScale < 1f ? 3f / Time.timeScale : 1.0f;

			magnet.GetComponent<Rigidbody>().AddForce(direction * force * timeScaleFactor, ForceMode.Acceleration);

			// 接近警告
			if (surfaceDistance <= tooClose.GetDangerDist())
			{
				magnet.isSlow_magObj = true;
				magCaution.SetActive(true);
				magNormal.SetActive(false);
			}
			else if (magnet.isSlow_magObj && surfaceDistance > tooClose.GetSafetyDist())
			{
				magnet.isSlow_magObj = false;
				magNormal.SetActive(true);
				magCaution.SetActive(false);
			}

			// 離れすぎる直前のやつ
			if (surfaceDistance >= tooFarAway.GetDangerDist_C() && !magnet.inPlayerMagArea)
			{
				magnet.dangerFarAway_magObj = true;
				magnet.isResisting = true;
			}
			else if (magnet.dangerFarAway_magObj && surfaceDistance < tooFarAway.GetSafetyDist_C())
			{
				magnet.dangerFarAway_magObj = false;
				magnet.isResisting = false;
			}

			// 近付きすぎるとくっつく
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
	}


	private void OnDestroy()
	{
		registeredMagnets.Clear();	// リストのクリア
	}

	// 磁力範囲のゲッター
	public float GetMagnetismRange()
	{
		return magnetismRange;
	}

	// くっついている左側のオブジェクトのコライダーのゲッター
	public SphereCollider GetCube1Collider()
	{
		return cube1Collider;
	}

	// くっついている右側のオブジェクトのコライダーのゲッター
	public SphereCollider GetCube2Collider()
	{
		return cube2Collider;
	}
}
