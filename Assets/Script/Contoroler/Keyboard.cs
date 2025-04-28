using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//=================================================
// 作成者：宮本和音
// 内職用　キーボードで操作できる関数（移動だけ）
//=================================================

public class Keyboard : MonoBehaviour
{
	[Header("移動の基準となるカメラ")]
	public Transform cameraTransform;	// カメラのTransform

	[Header("移動させる紐オブジェクト")]
	public GameObject rope;

	[Header("移動速度")]
	public float moveSpeed = 5.0f;      // 移動速度

	private Rigidbody rb;

	private bool isPlayerL;
	private Vector3 moveForward;	// カメラ基準の移動方向

	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody>();

		// 自身がPlayerLかどうか判断
		isPlayerL = gameObject.name == "PlayerL_Controller";
	}

	// Update is called once per frame
	void Update()
	{
		float horizontal = 0f;
		float vertical = 0f;

		if (isPlayerL)	// PlayerLならWASDキーで移動
		{
			if (Input.GetKey(KeyCode.W)) { vertical += 1f;
				Debug.Log("移動中です");
			}
			if (Input.GetKey(KeyCode.S)) { vertical -= 1f; }
			if (Input.GetKey(KeyCode.A)) { horizontal -= 1f; }
			if (Input.GetKey(KeyCode.D)) { horizontal += 1f; }
		}
		else	// PlayerRなら矢印キーで移動
		{
			if (Input.GetKey(KeyCode.UpArrow)) { vertical += 1f; }
			if (Input.GetKey(KeyCode.DownArrow)) { vertical -= 1f; }
			if (Input.GetKey(KeyCode.LeftArrow)) { horizontal -= 1f; }
			if (Input.GetKey(KeyCode.RightArrow)) { horizontal += 1f; }
		}

		// カメラの方向から、X-Z平面の単位ベクトルを取得
		Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
		Vector3 cameraRight = Vector3.Scale(cameraTransform.right, new Vector3(1, 0, 1)).normalized;

		// 入力値とカメラ向きから移動方向を決定
		moveForward = (cameraForward * vertical + cameraRight * horizontal).normalized;
	}


	private void FixedUpdate()
	{
		// 移動させる
		rb.velocity = moveForward * moveSpeed;

		// 紐の位置を合わせる
		rope.transform.position = this.transform.position;
	}
}
