using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=================================================
// 作成者：宮本和音
// 磁力オブジェクト（球）を運ぶ（動く？）スクリプト
//=================================================


public class MoveSphere : MonoBehaviour
{
	[Header("プレイヤーの磁石を設定")]
	public Transform magnet1;
	public Transform magnet2;

	private Rigidbody rb;
	private bool isCarrying = false;

	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		// 動きがまだ自然じゃないので改良していく
		if (isCarrying)
		{
			// 磁石2つの中間点を目指して動く感じ
			Vector3 targetPosition = (magnet1.position + magnet2.position) / 2f;
			Vector3 direction = targetPosition - transform.position;

			float distance = direction.magnitude;
			float speed = Mathf.Clamp(distance, 0.1f, 3.0f);	// 後ろ2つの数値の間で調整
			Vector3 velocity = direction.normalized * speed;

			rb.velocity = Vector3.Lerp(rb.velocity, velocity, Time.fixedDeltaTime * 10f);
		}
	}

	public void StartCarrying()
	{
		isCarrying = true;
		rb.useGravity = false;
		rb.angularDrag = 5.0f;
	}

	public void StopCarrying()
	{
		isCarrying = false;
		rb.useGravity = true;
		rb.angularDrag = 10.0f;
		rb.velocity = Vector3.zero;
	}
}
