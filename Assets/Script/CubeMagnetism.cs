using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMagnetism : MonoBehaviour
{
	[Header("���́E�͈͂̐ݒ�")]
	public float magnetismRange = 10.0f;
	[SerializeField] private float deadRange = 1.0f;
	public float magnetism = 200.0f;
	public float strongMagnetism = 999.0f;

	[Header("�v���C���[�I�u�W�F�N�g��ݒ�")]
	public GameObject playerL;
	public GameObject playerR;

	[Header("�������Ă�L���[�u��ݒ�")]
	public GameObject cube1;
	public GameObject cube2;

	//--- ���΂̃��X�g�Ǘ� ---//
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
		//--- ���͂̈����񂹏��� ---//
		
	}

	void AttachToSurface(Magnetism magnet)
	{
		if (magnet.isSnapping) return;

		// ��������i��FixedJoint�̍쐬�j
		FixedJoint joint = magnet.gameObject.AddComponent<FixedJoint>();
		joint.connectedBody = GetComponent<Rigidbody>();

		// �ʒu���킹
		magnet.myPlate.position = transform.position;

		magnet.GetComponent<AudioSource>().PlayOneShot(magnet.magnetSE);    // SE�Đ�
	}
}
