using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=================================================
// �쐬�ҁF�{�{�a��
// ���̓I�u�W�F�N�g�i���j���^�ԁi�����H�j�X�N���v�g
//=================================================


public class MoveSphere : MonoBehaviour
{
	[Header("�v���C���[�̎��΂�ݒ�")]
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
		// �������܂����R����Ȃ��̂ŉ��ǂ��Ă���
		if (isCarrying)
		{
			// ����2�̒��ԓ_��ڎw���ē�������
			Vector3 targetPosition = (magnet1.position + magnet2.position) / 2f;
			Vector3 direction = targetPosition - transform.position;

			float distance = direction.magnitude;
			float speed = Mathf.Clamp(distance, 0.1f, 3.0f);	// ���2�̐��l�̊ԂŒ���
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
