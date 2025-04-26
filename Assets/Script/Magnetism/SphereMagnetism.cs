using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=================================================
// 作成者：宮本和音
// 磁力付きオブジェクトにアタッチするスクリプト
//=================================================


public class SphereMagnetism : MonoBehaviour
{
	[Header("磁力・範囲の設定")]
	[SerializeField] private float magnetismRange = 10.0f;
	[SerializeField] private float deadRange = 1.0f;
	public float magnetism = 200.0f;
	public float strongMagnetism = 999.0f;

	[Header("プレイヤーオブジェクトを設定")]
	public GameObject playerL;
	public GameObject playerR;

	[Header("球が運搬可能かどうかのフラグ")]
	public bool canCarry;

	public float MagnetismRange => magnetismRange;
	public float DeadRange => deadRange;

	// 磁力強化フラグをそれぞれ取得する用
	private AugMagL magL_Aug;
	private AugMagR magR_Aug;

	// DeadRangeを取得・設定する用
	private Magnetism magnet1;
	private Magnetism magnet2;
	private float oridinalDRange;

	private SphereCollider sCollider;  // 磁石を引き寄せる頂点の計算用
	private MoveSphere moveS;

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

	// Start is called before the first frame update
	void Start()
	{
		// TryGetComponentの返り値はbool、SphereColliderが見つからなかった時falseになる
		if (!TryGetComponent<SphereCollider>(out sCollider))
		{
			Debug.LogError("SphereColliderついてないよ");
		}

		playerL.TryGetComponent<AugMagL>(out magL_Aug);
		playerR.TryGetComponent<AugMagR>(out magR_Aug);

		magnet1 = GameObject.Find("Magnet1").GetComponent<Magnetism>();
		magnet2 = GameObject.Find("Magnet2").GetComponent<Magnetism>();
		oridinalDRange = magnet1.deadRange;		// 本来のdeadRangeを保存しておく

		TryGetComponent<MoveSphere>(out moveS);
	}

	// Update is called once per frame
	void Update()
	{
		//--- 球が動かせるかどうかの判定 ---//
		if (magL_Aug.isAugmenting && magR_Aug.isAugmenting)
		{
			canCarry = true;
			moveS.StartCarrying();

			// 両方強化中はプレイヤーの磁石同士でくっつかないようにするためdeadRangeを0にしておく
			magnet1.SetDeadRange(0.0f, this);
			magnet2.SetDeadRange(0.0f, this);
		}
		else
		{
			canCarry = false;
			moveS.StopCarrying();

			magnet1.SetDeadRange(oridinalDRange, this);
			magnet2.SetDeadRange(oridinalDRange, this);
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

			// 球の表面にくっつけるために球の半径を計算
			float radius = sCollider.radius * transform.localScale.x;
			Vector3 surfacePoint = center + directionToMagnet * radius;

			// 球の表面と磁石の距離を計算
			float surfaceDistance = Vector3.Distance(surfacePoint, magnetPos);
			if (surfaceDistance > magnetismRange)
			{
				magnet.inSphereObjMagArea = false;
				continue;   // 磁力範囲外ならスキップ
			}

			magnet.inSphereObjMagArea = true;	// フラグを立てておく

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


	// 磁力範囲のゲッター
	public float GetMagnetismRange()
	{
		return magnetismRange;
	}
}