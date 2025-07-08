//------------------------------------------------
// 本田洸都
//------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovePlayerR : MonoBehaviour
{
	public float moveSpeed = 5.0f;      // 移動速度
	public float rotationSpeed = 5.0f;  // 回転速度

	[Header("移動の基準となるカメラ")]
	public Transform cameraTransform;   // カメラのTransform

	[Header("移動させる紐オブジェクト")]
	public GameObject rope;

	[Header("移動させるUIオブジェクト")]
	public GameObject UI;
	[Header("UIの最大移動距離")]
	public float maxRadius = 1.0f;
	[Header("UIが元の位置に戻る速さ")]
	public float returnSpeed = 5.0f;
	private Vector3 basePosition = Vector3.zero;    // 移動の基準点

	private Rigidbody rb;                       // Rigidbody
	private Rigidbody ropeRb;                   // 紐のRigidbody
	private GameInputs inputs;                  // GameInputsクラス
	//private PlaySEAtRegularIntervals playSE;    // PlaySEAtRegularIntervalsコンポーネント
	private Animator animator;

	private Vector2 moveInputValue;     // スティックの入力を受け取る
	private Vector3 moveForward;        // カメラ基準の移動方向

	Pose pose;          // Poseの状態を受け取る

	// Start is called before the first frame update
	void Start()
	{
		// Poseオブジェクトを取得
		pose = GameObject.Find("Pose").GetComponent<Pose>();

		// Rigidbodyコンポーネントを取得
		rb = transform.parent.GetComponent<Rigidbody>();
		//// PlaySEAtRegularIntervalsコンポーネントを取得
		//playSE = GetComponent<PlaySEAtRegularIntervals>();
		// アニメーターを取得
		animator = GameObject.Find("Female_Idling").GetComponent<Animator>();
		// GameInputsクラスのインスタンスを作成
		inputs = new GameInputs();

		// Actionイベント登録
		inputs.PlayerR.Move.started += OnMove;
		inputs.PlayerR.Move.performed += OnMove;
		inputs.PlayerR.Move.canceled += OnMove;

		// InputActionを機能させるためには、有効化する必要がある
		inputs.Enable();
	}

	// Update is called once per frame
	void Update()
	{
		// Pose画面を表示しているなら
		if (pose.GetPose())
		{
			return;
		}

		// カメラの方向から、X-Z平面の単位ベクトルを取得
		Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
		Vector3 cameraRight = Vector3.Scale(cameraTransform.right, new Vector3(1, 0, 1)).normalized;
		// カメラの向きに合わせてスティックの傾きから移動方向と移動度を求める
		moveForward = cameraForward * moveInputValue.y + cameraRight * moveInputValue.x;
		// 移動させる
		rb.velocity = moveForward * moveSpeed;
		// PlayerControllerの位置を合わせる
		transform.position = rb.transform.position;

		// 移動方向がゼロベクトルでない時
		if (moveForward != Vector3.zero)
		{
			//playSE.enabled = true;      // SE再生スクリプトを有効化
			// キャラクターの向きを徐々に移動方向に向ける
			transform.forward = Vector3.Slerp(transform.forward, moveForward, rotationSpeed * Time.deltaTime);
		}
		//// 移動方向がゼロベクトルの時
		//else
		//{
		//    playSE.SetElapsedTime(0);   // SE再生スクリプトの経過時間をリセット
		//    playSE.SetPlayCnt(0);		// SE再生スクリプトの再生回数をリセット
		//    playSE.enabled = false;     // SE再生スクリプトを無効化
		//}

		//------ アニメーション切り替え ------// 
		if (rb.velocity == Vector3.zero) { animator.SetBool("moveFg", false); }  // アニメーションを待機状態に切り替え
		else { animator.SetBool("moveFg", true); }                               // アニメーションを移動状態に切り替え

		//------ UIを移動させる ------//
		// UIの移動の基準点を取得
		basePosition.x = UI.transform.parent.position.x;
		basePosition.y = UI.transform.position.y;
		basePosition.z = UI.transform.parent.position.z;
		// UIをスティックに合わせて動かす
		if (moveInputValue.sqrMagnitude > 0.001f)
		{
			Vector3 target = Vector3.zero;
			// -maxRadius～maxRadiusの間で範囲制限
			target.x = Mathf.Clamp(moveForward.x * maxRadius, -maxRadius, maxRadius);
			target.z = Mathf.Clamp(moveForward.z * maxRadius, -maxRadius, maxRadius);
			UI.transform.position = basePosition + target;
		}
		// 入力がない場合は基準点にゆっくり戻す
		else
		{
			UI.transform.position = Vector3.Lerp(UI.transform.position, basePosition, Time.deltaTime * returnSpeed);
		}
	}


	private void OnDestroy()
	{
		// 自身でインスタンス化したActionクラスはIDisposableを実装しているので、必ずDisposeする必要がある
		// inputsがnullでない時、解放する
		inputs?.Dispose();
	}

	private void OnMove(InputAction.CallbackContext context)
	{
		// Moveアクションの入力取得
		moveInputValue = context.ReadValue<Vector2>();
	}
}
