using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=================================================
// ����ҁ@�{�{�a��
//=================================================

public class Magnetism : MonoBehaviour
{
	[Header("plate(Magnet�̒���)�̐ݒ�")]
	public Magnetism targetMagnet;	// �΂ɂȂ鎥��
	public Transform myPlate;	// ������BottomPlate
	public Transform targetPlate;	// �������鑊���BottomPlate

	[Header("���́E�͈͂̐ݒ�")]
	public float magnetismRange= 10.0f;	// �����񂹍�������
	[SerializeField] private float deadRange = 1.0f;	// �߂Â�������Ƃ������A�̋���
	public float magnetism = 200.0f;	// ����
	public float strongMagnetism = 999.0f;	// ����
	public float snapDistance = 0.07f;		// ������������臒l

	//--- magnetismRange��deadRange�̐ݒ� ---//
	// �Ƃ肠��������2�����A�������̕ϐ��������悤�ɂ���ꍇ�́�
	// ��́upublic�v���u[SerializeField] private�v�ɂ��������Ł�
	// ���̂Ɠ������ϐ����̂Ƃ������ς��ĕt�������Ă��΂�����

	// �O�����珑�������ł��Ȃ��悤�ɂ���i�A�N�Z�X�͂ł���j
	// magnetismRange��AdjustMagnetism����̂ݏ��������ɂ���
	public float MagnetismRange { get; private set; }
	public float DeadRange => deadRange;

	public void SetMagnetismRange(float newRange,object caller)
	{
		if (caller is AdjustMagnetism)
		{
			magnetismRange = newRange;
		}
	}

	[Header("�Q�[���̐i�s�Ɋւ��t���O")]
	public bool inMagnetismArea = true;		// ���͔͈͓����ǂ���
	public bool isSnapping = false;		// �������Ă邩�ǂ���

	private Rigidbody rb;

	void Awake()
	{
		//--- ���́E�͈͂̃f�o�b�O�p ---//
		// ���Γ��m�Ŋe�ϐ�����v���Ă��邩�`�F�b�N�A�s��v�Ȃ���s�ł��Ȃ�

		GameObject magnet1 = GameObject.Find("magnet1");
		GameObject magnet2 = GameObject.Find("magnet2");
		Magnetism mag1 = magnet1.GetComponent<Magnetism>();
		Magnetism mag2 = magnet2.GetComponent<Magnetism>();

		if (mag1.magnetismRange != mag2.magnetismRange || mag1.deadRange != mag2.deadRange ||
			mag1.magnetism != mag2.magnetism || mag1.strongMagnetism != mag2.strongMagnetism ||
			mag1.snapDistance != mag2.snapDistance)
		{
			if (mag1.magnetismRange != mag2.MagnetismRange)
			{
				Debug.LogError("Error�F�ϐ��s��v�@[magnetismRange]����v���Ă��܂���@" +
					"magnet1�F" + mag1.magnetismRange + "�@magnet2�F" + mag2.magnetismRange);
			}
			if (mag1.deadRange != mag2.deadRange)
			{
				Debug.LogError("Error�F�ϐ��s��v�@[deadRange]����v���Ă��܂���@" +
					"magnet1�F" + mag1.deadRange + "�@magnet2�F" + mag2.deadRange);
			}
			if (mag1.magnetism != mag2.magnetism)
			{
				Debug.LogError("Error�F�ϐ��s��v�@[magnetism]����v���Ă��܂���@" +
					"magnet1�F" + mag1.magnetism + "�@magnet2�F" + mag2.magnetism);
			}
			if (mag1.strongMagnetism != mag2.strongMagnetism)
			{
				Debug.LogError("Error�F�ϐ��s��v�@[strongMagnetism]����v���Ă��܂���@" +
					"magnet1�F" + mag1.strongMagnetism + "�@magnet2�F" + mag2.strongMagnetism);
			}
			if (mag1.snapDistance != mag2.snapDistance)
			{
				Debug.LogError("Error�F�ϐ��s��v�@[snapDistance]����v���Ă��܂���@" +
					"magnet1�F" + mag1.snapDistance + "�@magnet2�F" + mag2.snapDistance);
			}

			UnityEditor.EditorApplication.isPlaying = false;	// �Q�[���̎��s���~
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

		myPlate.position = targetPlate.position;
	}


	// Update is called once per frame
	void Update()
	{
	}
}
