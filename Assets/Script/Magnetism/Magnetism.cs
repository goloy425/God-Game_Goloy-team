using System.Collections;
using System.Collections.Generic;
using Unity.Profiling.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

//=================================================
// 制作者　宮本和音
// プレイヤーの磁石の処理
//=================================================

public class Magnetism : MonoBehaviour
{
	[Header("plate(Magnetの直下)の設定")]
	public Magnetism targetMagnet;	// 対になる磁石
	public Transform myPlate;		// 自分のBottomPlate
	public Transform targetPlate;   // くっつける相手のBottomPlate

	[Header("磁力・範囲の設定")]
	public float magnetismRange = 10.0f;	// 引き寄せ合う距離
	public float deadRange = 1.0f;			// 近づきすぎるとくっつく、の距離
	public float magnetism = 200.0f;		// 磁力
	public float strongMagnetism = 999.0f;	// 磁力（近づきすぎの方）
	public float snapDistance = 0.07f;		// くっつく距離の閾値

	//--- magnetismRangeとdeadRangeの設定 ---//
	// とりあえずこの2つだけ、もし他の変数も同じようにする場合は↓
	// 上の「public」を「[SerializeField] private」にしたうえで↓
	// 下のと同じやつを変数名のとこだけ変えて付け足してやればいける

	// 特定のスクリプトを除いて外部から書き換えできないようにする（アクセスはできる）
	public float MagnetismRange { get; private set; }
	public float DeadRange { get; private set; }

	// magnetismRangeはAdjustMagnetismからのみ書き換え可にする
	public void SetMagnetismRange(float newRange,object caller)
	{
		if (caller is AdjustMagnetism)
		{
			magnetismRange = newRange;
		}
	}

	// deadRangeはSphereMagnetismからのみ書き換え可にする
	public void SetDeadRange(float newRange,object caller)
	{
		if (caller is SphereMagnetism)
		{
			deadRange = newRange;
		}
	}

	[Header("ゲームの成否に関わるフラグ")]
	public bool inMagnetismArea = true;		// 何かしらの磁力範囲内かどうか
	// 下の「磁力範囲内かどうかのフラグ」2つがどちらともfalseになったら↑がfalseになる＝ゲームオーバー
	public bool isSnapping = false;		// くっついてるかどうか
	// これがtrueになる＝磁石が何かしらとくっついた＝ゲームオーバー

	[Header("磁力範囲内かどうかのフラグ")]
	public bool inPlayerMagArea = true;		// プレイヤーの磁石の磁力範囲内かどうか
	public bool inObjMagArea = true;		// オブジェクトの磁力範囲内かどうか

	[Header("磁石がくっついた時のSE")]
	public AudioClip magnetSE;
	private AudioSource audioSource;

	private Rigidbody rb;
	private CustomCameraController ccc;
	private TooClose tooClose;
	private TooFarAway tooFarAway;

	// 強化状態かどうか確認するためのやつ
	private AugMagL playerL;
	private AugMagR playerR;

	private bool L_isAugmenting = false;
	private bool R_isAugmenting = false;

	// スローモーション管理
	[HideInInspector] public bool isSlow_pMag = false;		// プレイヤーの磁石と近付きすぎた
	[HideInInspector] public bool isSlow_magObj = false;	// 磁力オブジェクト　　　〃

	// 離れすぎる直前ですよのフラグ
	/*[HideInInspector]*/ public bool dangerFarAway_pMag = false;
	/*[HideInInspector]*/ public bool dangerFarAway_magObj = false;

	private float time = 0.0f;
	public bool isResisting = false;

	// 磁力範囲とかのデバッグ用Awake()はコメントアウトして最後尾に持ってった
	// 必要ならコメントアウト解除してこの直下に貼ること

	void OnEnable()
	{
		// 磁石レジストリにプレイヤーの磁石を追加する
		RegisterToAllMagnets();
	}

	void OnDisable()
	{
		// 磁石レジストリをクリアする
		UnregisterFromAllMagnets();
	}

	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
		ccc = GameObject.Find("Main Camera").GetComponent<CustomCameraController>();
		tooClose = GameObject.Find("DistanceManager").GetComponent<TooClose>();
		tooFarAway = GameObject.Find("DistanceManager").GetComponent<TooFarAway>();

		// 成否に関わるフラグを初期化しておく
		isSnapping = false;
		inMagnetismArea = true;

		// 強化フラグの取得
		playerL = GameObject.Find("PlayerL_Controller").GetComponent<AugMagL>();
		playerR = GameObject.Find("PlayerR_Controller").GetComponent<AugMagR>();
	}

	private void Update()
	{
		// フラグの状況は常に更新しておく
		L_isAugmenting = playerL.isAugmenting;
		R_isAugmenting = playerR.isAugmenting;
	}

	void FixedUpdate()
	{
		//--- 磁力範囲内か外かを常に判定する ---//
		if (inPlayerMagArea || inObjMagArea)
		{
			inMagnetismArea = true;
		}
		else
		{
			inMagnetismArea = false;
		}

		//--- プレイヤーの磁石同士の引き寄せ処理 ---//
		// 距離を計算
		float distance = Vector3.Distance(myPlate.position, targetPlate.position);
		Vector3 direction = (targetPlate.position - myPlate.position).normalized;

		// プレイヤーごとの判定
		// アタッチされているオブジェクトがmagnet1(true)かmagnet2(false)か判定
		bool isSelfL = gameObject.name == "magnet1";
		// 自分が強化しているか相手が強化しているか判定
		bool isSelfAugmenting = isSelfL ? L_isAugmenting : R_isAugmenting;

		// 時間補正倍率（スロー中だけ強めに引き寄せる）
		float timeScaleFactor = Time.timeScale < 1f ? 3f / Time.timeScale : 1.0f;

		// 片方の磁石に引き寄せられるのは強化中でない時だけ
		if (distance < magnetismRange && !isSelfAugmenting)
		{
			inPlayerMagArea = true;
			float force = (distance < deadRange) ? strongMagnetism : magnetism;
			rb.WakeUp();	// スリープ対策
			rb.AddForce(direction * force * timeScaleFactor, ForceMode.Acceleration);
		}
		else
		{
			inPlayerMagArea = false;
		}

		if (inPlayerMagArea) { dangerFarAway_magObj = false; }

		// プレイヤーの磁石同士が離れすぎる直前のやつ
		// （磁力オブジェクトの磁力範囲内にいる時は入らない）
		if (distance >= tooFarAway.GetPDangerDist() && !inObjMagArea)
		{
			dangerFarAway_pMag = true;
			isResisting = true;
		}
		else if (dangerFarAway_pMag && distance < tooFarAway.GetPSafetyDist() || inObjMagArea)
		{
			dangerFarAway_pMag = false;
			isResisting = false;
		}

		// カメラがプレイヤーに追従し始めるまで後の処理をスルーする
		if (time < ccc.startDirectionTime)
		{
			time += Time.deltaTime;
			return;
		}

		// スローモーション切替用フラグの管理
		if (distance <= tooClose.GetPDangerDist())
		{
			isSlow_pMag = true;
		}
		else if (isSlow_pMag && distance > tooClose.GetPSafetyDist())
		{
			isSlow_pMag = false;
		}

		//--- 離れすぎる直前の抵抗処理 ---//
		if (isResisting)
		{
			
		}

		// くっつく処理：両方が強化状態にある時は無視
		if (distance < snapDistance && !L_isAugmenting && !R_isAugmenting)
		{
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;

			SnapToTarget();
			AttachToTarget();
		}
	}

	void SnapToTarget()
	{
		if(isSnapping) return;

		Collider myCollider = myPlate.GetComponent<BoxCollider>();
		Collider targetCollider = targetPlate.GetComponent<BoxCollider>();

		// Colliderの中心を取得
		Vector3 myCenter = myCollider.bounds.center;
		Vector3 targetCenter = targetCollider.bounds.center;

		// 中心点の差分を求める
		Vector3 offset = targetCenter - myCenter;

		// 差分だけPlateを動かす
		myPlate.position += offset;

		// 回転も合わせる（必要なら）
		myPlate.rotation = targetPlate.rotation;
	}

	void AttachToTarget()
	{
		if (isSnapping) return;

		// くっつける（＝FixedJointの作成）
		FixedJoint joint = gameObject.AddComponent<FixedJoint>();
		joint.connectedBody = targetPlate.GetComponentInParent<Rigidbody>();

		// AudioSourceが存在する時、SE再生
		if (audioSource != null)
		{
			audioSource.PlayOneShot(magnetSE);
		}

		isSnapping = true;
	}


	//--- 磁力オブジェクトが磁石を引き寄せるためのリストに登録 ---//
	private void RegisterToAllMagnets()
	{
		// タグを調べて該当のものがあれば追加
		// 球
		foreach (var sphere in GameObject.FindGameObjectsWithTag("MagObj_Sphere"))
		{
			if (sphere.GetComponent<SphereMagnetism>())
			{
				SphereMagnetism.Register(this);
			}
		}

		// キューブ
		foreach (var cube in GameObject.FindGameObjectsWithTag("MagObj_Cube"))
		{
			if (cube.GetComponent<CubeMagnetism>())
			{
				CubeMagnetism.Register(this);
			}
		}
		// 半キューブ
		foreach (var hCube in GameObject.FindGameObjectsWithTag("MagObj_HCube"))
		{
			if (hCube.GetComponent<HCubeMagnetism>())
			{
				HCubeMagnetism.Register(this);
			}
		}
	}

	private void UnregisterFromAllMagnets()
	{
		// こっちも追加の時と一緒でタグを利用する方式
		// 球
		foreach (var sphere in GameObject.FindGameObjectsWithTag("MagObj_Sphere"))
		{
			if (sphere.GetComponent<SphereMagnetism>())
			{
				SphereMagnetism.Unregister(this);
			}
		}

		// キューブ
		foreach (var cube in GameObject.FindGameObjectsWithTag("MagObj_Cube"))
		{
			if (cube.GetComponent<CubeMagnetism>())
			{
				CubeMagnetism.Unregister(this);
			}
		}
		// 半キューブ
		foreach (var hCube in GameObject.FindGameObjectsWithTag("MagObj_HCube"))
		{
			if (hCube.GetComponent<HCubeMagnetism>())
			{
				HCubeMagnetism.Unregister(this);
			}
		}
	}

	// スクリプトが削除される時に生存関係のフラグをfalseにしておく
	private void OnDestroy()
	{
		inPlayerMagArea = false;
		inObjMagArea = false;
	}

	// 磁力範囲内かどうかの取得
	public bool GetInMagnetismArea()
	{
		return inMagnetismArea;
	}

	// プレイヤーのマグネットによるスロー再生かどうかの取得
	public bool GetIsSlowPMag()
	{
		return isSlow_pMag;
	}

	// 磁力オブジェクトによるスロー再生かどうかの取得
	public bool GetIsSlowMagObj()
	{
		return isSlow_magObj;
	}

	// くっついたかどうかの取得
	public bool GetIsSnapping()
	{
		return isSnapping;
	}

	// Lの磁石が強化中かどうかの取得
	public bool GetIsAugmentingL()
	{
		return L_isAugmenting;
	}

	// Rの磁石が強化中かどうかの取得
	public bool GetIsAugmentingR()
	{
		return R_isAugmenting;
	}

	// プレイヤーの磁石同士が離れすぎる直前かどうか
	public bool GetIsFarAwayPMag()
	{
		return dangerFarAway_pMag;
	}
	// プレイヤーの磁石対磁力オブジェクトが離れすぎる直前かどうか
	public bool GetIsFarAwayMagObj()
	{
		return dangerFarAway_magObj;
	}
}



//void Awake()
//{
//	//--- 磁力・範囲のデバッグ用 ---//
//	// 磁石同士で各変数が一致しているかチェック、不一致なら実行できない

//	Magnetism mag1 = GameObject.Find("Magnet1").GetComponent<Magnetism>();
//	Magnetism mag2 = GameObject.Find("Magnet2").GetComponent<Magnetism>();

//	if (mag1.magnetismRange != mag2.magnetismRange || mag1.deadRange != mag2.deadRange ||
//		mag1.magnetism != mag2.magnetism || mag1.strongMagnetism != mag2.strongMagnetism ||
//		mag1.snapDistance != mag2.snapDistance)
//	{
//		if (mag1.magnetismRange != mag2.MagnetismRange)
//		{
//			Debug.LogError("Error：変数不一致　[magnetismRange]が一致していません　" +
//				"magnet1：" + mag1.magnetismRange + "　magnet2：" + mag2.magnetismRange);
//		}
//		if (mag1.deadRange != mag2.deadRange)
//		{
//			Debug.LogError("Error：変数不一致　[deadRange]が一致していません　" +
//				"magnet1：" + mag1.deadRange + "　magnet2：" + mag2.deadRange);
//		}
//		if (mag1.magnetism != mag2.magnetism)
//		{
//			Debug.LogError("Error：変数不一致　[magnetism]が一致していません　" +
//				"magnet1：" + mag1.magnetism + "　magnet2：" + mag2.magnetism);
//		}
//		if (mag1.strongMagnetism != mag2.strongMagnetism)
//		{
//			Debug.LogError("Error：変数不一致　[strongMagnetism]が一致していません　" +
//				"magnet1：" + mag1.strongMagnetism + "　magnet2：" + mag2.strongMagnetism);
//		}
//		if (mag1.snapDistance != mag2.snapDistance)
//		{
//			Debug.LogError("Error：変数不一致　[snapDistance]が一致していません　" +
//				"magnet1：" + mag1.snapDistance + "　magnet2：" + mag2.snapDistance);
//		}

//		//UnityEditor.EditorApplication.isPlaying = false;	// ゲームの実行を停止
//	}
//}
