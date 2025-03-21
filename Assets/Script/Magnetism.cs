using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ����ҁ@�{�{�a��

public class Magnetism : MonoBehaviour
{
	[Header("plate(Magnet�̒���)�̐ݒ�")]
	public Transform myPlate;	// ������BottomPlate
	public Transform targetPlate;	// �������鑊���BottomPlate

	[Header("���́E�͈͂̐ݒ�")]
	public float magnetismRange = 10.0f;	// �����񂹍�������
	public float deadRange = 1.0f;	// �߂Â�������Ƃ������A�̋���
	public float magnetism = 200.0f;	// ����
	public float strongMagnetism = 999.0f;	// �߂Â����������̎��́i�����j
	public float snapDistance = 0.07f;	// ������������臒l

	private bool isSnapping = false;	// �������Ă邩�ǂ���
	private Rigidbody rb;

	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		isSnapping = false;
	}


	void FixedUpdate()
	{
		// �������v�Z
		float distance = Vector3.Distance(myPlate.position, targetPlate.position);
		Vector3 direction = (targetPlate.position - myPlate.position).normalized;

		if (distance < magnetismRange)	// ���͔͈͓��Ȃ������
		{
			float force = (distance < deadRange) ? strongMagnetism : magnetism;
			rb.AddForce(direction * force, ForceMode.Acceleration);
		}

		if (distance < snapDistance)	// �ڋ߂�������Ƃ�����
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

		// ��������i��FixedJoint�̍쐬�j
		FixedJoint joint = gameObject.AddComponent<FixedJoint>();
		joint.connectedBody = targetPlate.GetComponentInParent<Rigidbody>();
	}

	// Update is called once per frame
	void Update()
	{
	}
}
