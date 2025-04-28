using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Experimental.GraphView.GraphView;

//=================================================
// 作成者：宮本和音
// 半キューブ（R）を動かすスクリプト
//=================================================

public class MoveHCubeR : MonoBehaviour
{
	[Header("プレイヤーオブジェクトを設定")]
	public GameObject playerR;

	[Header("磁石の登録")]
	public Transform magnet2;

	public bool isCarryingR = false;
	private Rigidbody rb;

	// 磁力強化フラグをそれぞれ取得する用
	private AugMagR magR_Aug;
	
    private PlaySEAtRegularIntervals playSE;    // PlaySEAtRegularIntervalsコンポーネント

	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		playerR.TryGetComponent<AugMagR>(out magR_Aug);

        playSE = GetComponent<PlaySEAtRegularIntervals>();
    }

	private void Update()
	{
		if (magR_Aug.isAugmenting)
		{
			StartCarryingR();
		}
		else
		{
			StopCarryingR();
		}
	}


	private void FixedUpdate()
	{
		if (isCarryingR)
		{
			Vector3 direction = (magnet2.position - transform.position);
			float distance = direction.magnitude;
			float speed = Mathf.Clamp(distance, 0.1f, 3.0f);

			Vector3 velocity = direction.normalized * speed;
			rb.velocity = Vector3.Lerp(rb.velocity, velocity, Time.fixedDeltaTime * 10f);
		}
	}

	// --- 持ち上げ開始 --- //
	public void StartCarryingR()
	{
		isCarryingR = true;
		rb.useGravity = false;
		rb.angularDrag = 5f;

        playSE.enabled = true;      // SE再生スクリプトを有効化
    }

	// --- 持ち上げ終了 --- //
	public void StopCarryingR()
	{
		isCarryingR = false;
		rb.useGravity = true;
		rb.angularDrag = 10f;
		rb.velocity = Vector3.zero;

        playSE.SetElapsedTime(0);   // SE再生スクリプトの経過時間をリセット
        playSE.SetPlayCnt(0);		// SE再生スクリプトの再生回数をリセット
        playSE.enabled = false;     // SE再生スクリプトを無効化
    }
}
