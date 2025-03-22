using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=================================================
// ����ҁ@�{�{�a��
//=================================================

public class Magnetism : MonoBehaviour
{
	[Header("plate(Magnet�̒���)�̐ݒ�")]
	public Magnetism targetMagnet;  // �΂ɂȂ鎥��
	public Transform myPlate;   // ������BottomPlate
	public Transform targetPlate;   // �������鑊���BottomPlate

	[Header("���́E�͈͂̐ݒ�")]
	[SerializeField] private float magnetismRange = 10.0f;  // �����񂹍�������
	[SerializeField] private float deadRange = 1.0f;    // �߂Â�������Ƃ������A�̋���
	public float magnetism = 200.0f;    // ����
	public float strongMagnetism = 999.0f;  // ����
	public float snapDistance = 0.07f;      // ������������臒l

	//--- magnetismRange��deadRange�̐ݒ� ---//
	// �Ƃ肠��������2�����A�������̕ϐ��������悤�ɂ���ꍇ�́�
	// ��́upublic�v���u[SerializeField] private�v�ɂ��������Ł�
	// ���̂Ɠ������ϐ����̂Ƃ������ς��ĕt�������Ă��΂�����

	// �O�����珑�������ł��Ȃ��悤�ɂ���i�A�N�Z�X�͂ł���j
	public float MagnetismRange => magnetismRange;
	public float DeadRange => deadRange;

	[Header("�Q�[���̐i�s�Ɋւ��t���O")]
	public bool inMagnetismArea = true;     // ���͔͈͓����ǂ���
	public bool isSnapping = false;     // �������Ă邩�ǂ���

	private Rigidbody rb;

	void Awake()
	{
		//--- ���́E�͈͂̃f�o�b�O�p ---//
		// ���Γ��m�Ŋe�ϐ�����v���Ă��邩�`�F�b�N�A�s��v�Ȃ���s�ł��Ȃ�
		if (magnetismRange != targetMagnet.magnetismRange || deadRange != targetMagnet.deadRange ||
			magnetism != targetMagnet.magnetism || strongMagnetism != targetMagnet.strongMagnetism ||
			snapDistance != targetMagnet.snapDistance)
		{
			if (magnetismRange != targetMagnet.MagnetismRange)
			{
				Debug.LogError("Error�F�ϐ��s��v�@magnetismRange����v���Ă��܂���");
			}
			else if (deadRange != targetMagnet.deadRange)
			{
				Debug.LogError("Error�F�ϐ��s��v�@deadRange����v���Ă��܂���");
			}
			else if (magnetism != targetMagnet.magnetism)
			{
				Debug.LogError("Error�F�ϐ��s��v�@magnetism����v���Ă��܂���");
			}
			else if (strongMagnetism != targetMagnet.strongMagnetism)
			{
				Debug.LogError("Error�F�ϐ��s��v�@strongMagnetism����v���Ă��܂���");
			}
			else if (snapDistance != targetMagnet.snapDistance)
			{
				Debug.LogError("Error�F�ϐ��s��v�@snapDistance����v���Ă��܂���");
			}

			UnityEditor.EditorApplication.isPlaying = false;    // �Q�[���̎��s���~
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		isSnapping = false;
		inMagnetismArea = true;
	}

	void FixedUpdate()
	{
		// �������v�Z
		float distance = Vector3.Distance(myPlate.position, targetPlate.position);
		Vector3 direction = (targetPlate.position - myPlate.position).normalized;

		if (distance < magnetismRange) // ���͔͈͓��Ȃ������
		{
			inMagnetismArea = true;
			float force = (distance < deadRange) ? strongMagnetism : magnetism;
			rb.AddForce(direction * force, ForceMode.Acceleration);
		}
		else
		{
			inMagnetismArea = false;
		}

		if (distance < snapDistance)    // �ڋ߂�������Ƃ�����
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

		myPlate.position = targetPlate.position;
	}

	// Update is called once per frame
	void Update()
	{
	}
}
