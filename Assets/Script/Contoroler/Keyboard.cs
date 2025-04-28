using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//=================================================
// �쐬�ҁF�{�{�a��
// ���E�p�@�L�[�{�[�h�ő���ł���֐��i�ړ������j
//=================================================

public class Keyboard : MonoBehaviour
{
	[Header("�ړ��̊�ƂȂ�J����")]
	public Transform cameraTransform;	// �J������Transform

	[Header("�ړ�������R�I�u�W�F�N�g")]
	public GameObject rope;

	[Header("�ړ����x")]
	public float moveSpeed = 5.0f;      // �ړ����x

	private Rigidbody rb;

	private bool isPlayerL;
	private Vector3 moveForward;	// �J������̈ړ�����

	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody>();

		// ���g��PlayerL���ǂ������f
		isPlayerL = gameObject.name == "PlayerL_Controller";
	}

	// Update is called once per frame
	void Update()
	{
		float horizontal = 0f;
		float vertical = 0f;

		if (isPlayerL)	// PlayerL�Ȃ�WASD�L�[�ňړ�
		{
			if (Input.GetKey(KeyCode.W)) { vertical += 1f;
				Debug.Log("�ړ����ł�");
			}
			if (Input.GetKey(KeyCode.S)) { vertical -= 1f; }
			if (Input.GetKey(KeyCode.A)) { horizontal -= 1f; }
			if (Input.GetKey(KeyCode.D)) { horizontal += 1f; }
		}
		else	// PlayerR�Ȃ���L�[�ňړ�
		{
			if (Input.GetKey(KeyCode.UpArrow)) { vertical += 1f; }
			if (Input.GetKey(KeyCode.DownArrow)) { vertical -= 1f; }
			if (Input.GetKey(KeyCode.LeftArrow)) { horizontal -= 1f; }
			if (Input.GetKey(KeyCode.RightArrow)) { horizontal += 1f; }
		}

		// �J�����̕�������AX-Z���ʂ̒P�ʃx�N�g�����擾
		Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
		Vector3 cameraRight = Vector3.Scale(cameraTransform.right, new Vector3(1, 0, 1)).normalized;

		// ���͒l�ƃJ������������ړ�����������
		moveForward = (cameraForward * vertical + cameraRight * horizontal).normalized;
	}


	private void FixedUpdate()
	{
		// �ړ�������
		rb.velocity = moveForward * moveSpeed;

		// �R�̈ʒu�����킹��
		rope.transform.position = this.transform.position;
	}
}
