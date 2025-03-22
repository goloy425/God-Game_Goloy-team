using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=================================================
// 制作者　宮本和音
//=================================================

public class Magnetism : MonoBehaviour
{
	[Header("plate(Magnetの直下)の設定")]
	public Magnetism targetMagnet;  // 対になる磁石
	public Transform myPlate;   // 自分のBottomPlate
	public Transform targetPlate;   // くっつける相手のBottomPlate

	[Header("磁力・範囲の設定")]
	[SerializeField] private float magnetismRange = 10.0f;  // 引き寄せ合う距離
	[SerializeField] private float deadRange = 1.0f;    // 近づきすぎるとくっつく、の距離
	public float magnetism = 200.0f;    // 磁力
	public float strongMagnetism = 999.0f;  // 磁力
	public float snapDistance = 0.07f;      // くっつく距離の閾値

	//--- magnetismRangeとdeadRangeの設定 ---//
	// とりあえずこの2つだけ、もし他の変数も同じようにする場合は↓
	// 上の「public」を「[SerializeField] private」にしたうえで↓
	// 下のと同じやつを変数名のとこだけ変えて付け足してやればいける

	// 外部から書き換えできないようにする（アクセスはできる）
	public float MagnetismRange => magnetismRange;
	public float DeadRange => deadRange;

	[Header("ゲームの進行に関わるフラグ")]
	public bool inMagnetismArea = true;     // 磁力範囲内かどうか
	public bool isSnapping = false;     // くっついてるかどうか

	private Rigidbody rb;

	void Awake()
	{
		//--- 磁力・範囲のデバッグ用 ---//
		// 磁石同士で各変数が一致しているかチェック、不一致なら実行できない
		if (magnetismRange != targetMagnet.magnetismRange || deadRange != targetMagnet.deadRange ||
			magnetism != targetMagnet.magnetism || strongMagnetism != targetMagnet.strongMagnetism ||
			snapDistance != targetMagnet.snapDistance)
		{
			if (magnetismRange != targetMagnet.MagnetismRange)
			{
				Debug.LogError("Error：変数不一致　magnetismRangeが一致していません");
			}
			else if (deadRange != targetMagnet.deadRange)
			{
				Debug.LogError("Error：変数不一致　deadRangeが一致していません");
			}
			else if (magnetism != targetMagnet.magnetism)
			{
				Debug.LogError("Error：変数不一致　magnetismが一致していません");
			}
			else if (strongMagnetism != targetMagnet.strongMagnetism)
			{
				Debug.LogError("Error：変数不一致　strongMagnetismが一致していません");
			}
			else if (snapDistance != targetMagnet.snapDistance)
			{
				Debug.LogError("Error：変数不一致　snapDistanceが一致していません");
			}

			UnityEditor.EditorApplication.isPlaying = false;    // ゲームの実行を停止
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		isSnapping = false;
		inMagnetismArea = true;
	}

	void FixedUpdate()
	{
		// 距離を計算
		float distance = Vector3.Distance(myPlate.position, targetPlate.position);
		Vector3 direction = (targetPlate.position - myPlate.position).normalized;

		if (distance < magnetismRange) // 磁力範囲内なら引き寄せ
		{
			inMagnetismArea = true;
			float force = (distance < deadRange) ? strongMagnetism : magnetism;
			rb.AddForce(direction * force, ForceMode.Acceleration);
		}
		else
		{
			inMagnetismArea = false;
		}

		if (distance < snapDistance)    // 接近しすぎるとくっつく
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

		myPlate.position = targetPlate.position;
	}

	// Update is called once per frame
	void Update()
	{
	}
}
