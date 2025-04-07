using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMagnetism : MonoBehaviour
{
	[Header("磁力・範囲の設定")]
	public float magnetismRange = 10.0f;
	[SerializeField] private float deadRange = 1.0f;
	public float magnetism = 200.0f;
	public float strongMagnetism = 999.0f;

	[Header("プレイヤーオブジェクトを設定")]
	public GameObject playerL;
	public GameObject playerR;

	[Header("くっついてるキューブを設定")]
	public GameObject cube1;
	public GameObject cube2;

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
		
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	private void FixedUpdate()
	{
		//--- 磁力の引き寄せ処理 ---//
		
	}

	void AttachToSurface(Magnetism magnet)
	{
		if (magnet.isSnapping) return;

		// くっつける（＝FixedJointの作成）
		FixedJoint joint = magnet.gameObject.AddComponent<FixedJoint>();
		joint.connectedBody = GetComponent<Rigidbody>();

		// 位置合わせ
		magnet.myPlate.position = transform.position;

		magnet.GetComponent<AudioSource>().PlayOneShot(magnet.magnetSE);    // SE再生
	}
}
