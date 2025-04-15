using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

//=================================================
// 作成者：宮本和音
// キューブをふたつに割るスクリプト
//=================================================


public class SplitCube : MonoBehaviour
{
	[Header("プレイヤーオブジェクトを設定")]
	public GameObject playerL;
	public GameObject playerR;

	[Header("くっついてるキューブを設定")]
	public GameObject cube1;
	public GameObject cube2;

	[Header("キューブが分割できるかどうか")]
	public bool canSplit;
	public bool splited;	// 分割したらこっちがtrueになる

	[Header("キューブ分割に関係する数値")]
	public float requiredDistance = 2.0f;	// 磁石がある程度離れてるか
	public float alignThreshold = 180.0f;   // プレイヤー磁石とCubeの位置がほぼ一直線ならOK

	// 分離判定に使う用
	private Vector3 initialL;
	private Vector3 initialR;
	public bool isTracking = false;

	// 分離後にFixedJointを解除する用
	private FixedJoint jointL;
	private FixedJoint jointR;

	// 分離後に磁力をオンにする用
	private HCubeMagnetism cMag1;
	private HCubeMagnetism cMag2;

	// 磁力強化フラグをそれぞれ取得する用
	private AugMagL magL_Aug;
	private AugMagR magR_Aug;

	// DeadRangeを取得・設定する用
	private GameObject magnet1;
	private GameObject magnet2;
	private Magnetism mag1;
	private Magnetism mag2;
	private float oridinalDRange;

	// Start is called before the first frame update
	void Start()
	{
		playerL.TryGetComponent<AugMagL>(out magL_Aug);
		playerR.TryGetComponent<AugMagR>(out magR_Aug);

		magnet1 = GameObject.Find("Magnet1");
		magnet2 = GameObject.Find("Magnet2");
		mag1 = magnet1.GetComponent<Magnetism>();
		mag2 = magnet2.GetComponent<Magnetism>();

		jointL = cube1.GetComponent<FixedJoint>();
		jointR = cube2.GetComponent<FixedJoint>();
		cMag1 = cube1.GetComponent<HCubeMagnetism>();
		cMag2 = cube2.GetComponent<HCubeMagnetism>();

		oridinalDRange = mag1.deadRange;	// 本来のdeadRangeを保存しておく
	}

	// Update is called once per frame
	void Update()
	{
		//--- キューブを分割できるかどうかの判定 ---//
		if (!isTracking && magL_Aug.isAugmenting && magR_Aug.isAugmenting)  // どっちも強化状態になってるかどうか確認
		{
			canSplit = true;
			isTracking = true;
			initialL = playerL.transform.position;
			initialR = playerR.transform.position;

			// 両方強化中はプレイヤーの磁石同士でくっつかないようにするためdeadRangeを0にしておく
			mag1.SetDeadRange(0.0f, this);
			mag2.SetDeadRange(0.0f, this);
		}
		else if(!magL_Aug.isAugmenting || !magR_Aug.isAugmenting)
		{
			isTracking = false;
			canSplit = false;

			mag1.SetDeadRange(oridinalDRange, this);
			mag2.SetDeadRange(oridinalDRange, this);
		}
	}

	void FixedUpdate()
	{
		if (!canSplit || !isTracking || splited) return;

		float movedL = Vector3.Distance(initialL, playerL.transform.position);
		float movedR = Vector3.Distance(initialR, playerR.transform.position);

		Vector3 toLeft = magnet1.transform.position - transform.position;
		Vector3 toRight = magnet2.transform.position - transform.position;

		float angle = Vector3.Angle(toLeft, -toRight);
		Debug.Log("movedL:" + movedL + " movedR" + movedR);

		if (movedL > requiredDistance && movedR > requiredDistance && Mathf.Abs(angle - 180f) > alignThreshold)
		{
			BreakCube();
		}
	}

	void BreakCube()
	{
		splited = true;

		// キューブのFixedJointを解除する
		Destroy(jointL);
		Destroy(jointR);

		// 半キューブの磁力スクリプトをアクティブ化
		cMag1.enabled = true;
		cMag2.enabled = true;

		Debug.Log("キューブまっぷたつ！");
		this.gameObject.SetActive(false);
	}
}
