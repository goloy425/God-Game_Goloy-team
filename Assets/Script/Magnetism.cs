using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=================================================
// 制作者　宮本和音
//=================================================

public class Magnetism : MonoBehaviour
{
	[Header("plate(Magnetの直下)の設定")]
	public Magnetism targetMagnet;	// 対になる磁石
	public Transform myPlate;	// 自分のBottomPlate
	public Transform targetPlate;	// くっつける相手のBottomPlate

	[Header("磁力・範囲の設定")]
	public float magnetismRange= 10.0f;	// 引き寄せ合う距離
	[SerializeField] private float deadRange = 1.0f;	// 近づきすぎるとくっつく、の距離
	public float magnetism = 200.0f;	// 磁力
	public float strongMagnetism = 999.0f;	// 磁力
	public float snapDistance = 0.07f;		// くっつく距離の閾値

	//--- magnetismRangeとdeadRangeの設定 ---//
	// とりあえずこの2つだけ、もし他の変数も同じようにする場合は↓
	// 上の「public」を「[SerializeField] private」にしたうえで↓
	// 下のと同じやつを変数名のとこだけ変えて付け足してやればいける

	// 外部から書き換えできないようにする（アクセスはできる）
	// magnetismRangeはAdjustMagnetismからのみ書き換え可にする
	public float MagnetismRange { get; private set; }
	public float DeadRange => deadRange;

	public void SetMagnetismRange(float newRange,object caller)
	{
		if (caller is AdjustMagnetism)
		{
			magnetismRange = newRange;
		}
	}

	[Header("ゲームの進行に関わるフラグ")]
	public bool inMagnetismArea = true;		// 磁力範囲内かどうか
	public bool isSnapping = false;		// くっついてるかどうか

	private Rigidbody rb;

	void Awake()
	{
		//--- 磁力・範囲のデバッグ用 ---//
		// 磁石同士で各変数が一致しているかチェック、不一致なら実行できない

		GameObject magnet1 = GameObject.Find("magnet1");
		GameObject magnet2 = GameObject.Find("magnet2");
		Magnetism mag1 = magnet1.GetComponent<Magnetism>();
		Magnetism mag2 = magnet2.GetComponent<Magnetism>();

		if (mag1.magnetismRange != mag2.magnetismRange || mag1.deadRange != mag2.deadRange ||
			mag1.magnetism != mag2.magnetism || mag1.strongMagnetism != mag2.strongMagnetism ||
			mag1.snapDistance != mag2.snapDistance)
		{
			if (mag1.magnetismRange != mag2.MagnetismRange)
			{
				Debug.LogError("Error：変数不一致　[magnetismRange]が一致していません　" +
					"magnet1：" + mag1.magnetismRange + "　magnet2：" + mag2.magnetismRange);
			}
			if (mag1.deadRange != mag2.deadRange)
			{
				Debug.LogError("Error：変数不一致　[deadRange]が一致していません　" +
					"magnet1：" + mag1.deadRange + "　magnet2：" + mag2.deadRange);
			}
			if (mag1.magnetism != mag2.magnetism)
			{
				Debug.LogError("Error：変数不一致　[magnetism]が一致していません　" +
					"magnet1：" + mag1.magnetism + "　magnet2：" + mag2.magnetism);
			}
			if (mag1.strongMagnetism != mag2.strongMagnetism)
			{
				Debug.LogError("Error：変数不一致　[strongMagnetism]が一致していません　" +
					"magnet1：" + mag1.strongMagnetism + "　magnet2：" + mag2.strongMagnetism);
			}
			if (mag1.snapDistance != mag2.snapDistance)
			{
				Debug.LogError("Error：変数不一致　[snapDistance]が一致していません　" +
					"magnet1：" + mag1.snapDistance + "　magnet2：" + mag2.snapDistance);
			}

			UnityEditor.EditorApplication.isPlaying = false;	// ゲームの実行を停止
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

		myPlate.position = targetPlate.position;
	}


	// Update is called once per frame
	void Update()
	{
	}
}
