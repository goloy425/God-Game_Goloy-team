using System.Collections;
using System.Collections.Generic;
using Unity.Profiling.LowLevel.Unsafe;
using Unity.VisualScripting;
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
	public float deadRange = 1.0f;	// �߂Â�������Ƃ������A�̋���
	public float magnetism = 200.0f;	// ����
	public float strongMagnetism = 999.0f;	// ����
	public float snapDistance = 0.07f;		// ������������臒l

	//--- magnetismRange��deadRange�̐ݒ� ---//
	// �Ƃ肠��������2�����A�������̕ϐ��������悤�ɂ���ꍇ�́�
	// ��́upublic�v���u[SerializeField] private�v�ɂ��������Ł�
	// ���̂Ɠ������ϐ����̂Ƃ������ς��ĕt�������Ă��΂�����

	// ����̃X�N���v�g�������ĊO�����珑�������ł��Ȃ��悤�ɂ���i�A�N�Z�X�͂ł���j
	public float MagnetismRange { get; private set; }
	public float DeadRange { get; private set; }

	// magnetismRange��AdjustMagnetism����̂ݏ��������ɂ���
	public void SetMagnetismRange(float newRange,object caller)
	{
		if (caller is AdjustMagnetism)
		{
			magnetismRange = newRange;
		}
	}

	// deadRange��SphereMagnetism����̂ݏ��������ɂ���
	public void SetDeadRange(float newRange,object caller)
	{
		if (caller is SphereMagnetism)
		{
			deadRange = newRange;
		}
	}

	[Header("�Q�[���̐��ۂɊւ��t���O")]
	public bool inMagnetismArea = true;		// ��������̎��͔͈͓����ǂ���
	// ���́u���͔͈͓����ǂ����̃t���O�v2���ǂ���Ƃ�false�ɂȂ����灪��false�ɂȂ遁�Q�[���I�[�o�[
	public bool isSnapping = false;		// �������Ă邩�ǂ���
	// ���ꂪtrue�ɂȂ遁���΂���������Ƃ����������Q�[���I�[�o�[

	[Header("���͔͈͓����ǂ����̃t���O")]
	public bool inPlayerMagArea = true;		// �v���C���[�̎��΂̎��͔͈͓����ǂ���
	public bool inObjMagArea = true;		// �I�u�W�F�N�g�̎��͔͈͓����ǂ���

	[Header("���΂�������������SE")]
	public AudioClip magnetSE;
	private AudioSource audioSource;

	private GameManager gameManager;

	private Rigidbody rb;

	// ������Ԃ��ǂ����m�F���邽�߂̂��
	private AugMagL playerL;
	private AugMagR playerR;

	private bool L_isAugmenting;
	private bool R_isAugmenting;

	void Awake()
	{
		//--- ���́E�͈͂̃f�o�b�O�p ---//
		// ���Γ��m�Ŋe�ϐ�����v���Ă��邩�`�F�b�N�A�s��v�Ȃ���s�ł��Ȃ�

		Magnetism mag1 = GameObject.Find("Magnet1").GetComponent<Magnetism>();
		Magnetism mag2 = GameObject.Find("Magnet2").GetComponent<Magnetism>();

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

			//UnityEditor.EditorApplication.isPlaying = false;	// �Q�[���̎��s���~
		}
	}

	void OnEnable()
	{
		// ���΃��W�X�g���Ƀv���C���[�̎��΂�ǉ�����
		RegisterToAllMagnets();
	}

	void OnDisable()
	{
		// ���΃��W�X�g�����N���A����
		UnregisterFromAllMagnets();
	}

	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

		// ���ۂɊւ��t���O�����������Ă���
		isSnapping = false;
		inMagnetismArea = true;

		// �����t���O�̎擾
		playerL = GameObject.Find("PlayerL_Controller").GetComponent<AugMagL>();
		playerR = GameObject.Find("PlayerR_Controller").GetComponent<AugMagR>();
	}

	private void Update()
	{
		// �t���O�̏󋵂͏�ɍX�V���Ă���
		L_isAugmenting = playerL.isAugmenting;
		R_isAugmenting = playerR.isAugmenting;
	}

	void FixedUpdate()
	{
		//--- ���͔͈͓����O������ɔ��肷�� ---//
		if (inPlayerMagArea || inObjMagArea)
		{
			inMagnetismArea = true;
		}
		else
		{
			inMagnetismArea = false;

			gameManager.SetGameOverFg(true);    // �Q�[���I�[�o�[�ɂ���
		}

		//--- �v���C���[�̎��Γ��m�̈����񂹏��� ---//
		// �������v�Z
		float distance = Vector3.Distance(myPlate.position, targetPlate.position);
		Vector3 direction = (targetPlate.position - myPlate.position).normalized;

		// �v���C���[���Ƃ̔���
		// �A�^�b�`����Ă���I�u�W�F�N�g��magnet1(true)��magnet2(false)������
		bool isSelfL = gameObject.name == "magnet1";
		// �������������Ă��邩���肪�������Ă��邩����
		bool isSelfAugmenting = isSelfL ? L_isAugmenting : R_isAugmenting;

		// �Е��̎��΂Ɉ����񂹂���̂͋������łȂ�������
		if (distance < magnetismRange && !isSelfAugmenting)
		{
			inPlayerMagArea = true;
			float force = (distance < deadRange) ? strongMagnetism : magnetism;
			rb.AddForce(direction * force, ForceMode.Acceleration);
		}
		else
		{
			inPlayerMagArea = false;
		}

		// �����������F������������Ԃɂ��鎞�͖���
		if (distance < snapDistance && !L_isAugmenting && !R_isAugmenting)
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

		// AudioSource�����݂��鎞�ASE�Đ�
		if(audioSource != null)
		{
			audioSource.PlayOneShot(magnetSE);
		}

		gameManager.SetGameOverFg(true);	// �Q�[���I�[�o�[�ɂ���
	}


	//--- ���̓I�u�W�F�N�g�����΂������񂹂邽�߂̃��X�g�ɓo�^ ---//
	private void RegisterToAllMagnets()
	{
		// �^�O�𒲂ׂĊY���̂��̂�����Βǉ�
		// ��
		foreach (var sphere in GameObject.FindGameObjectsWithTag("MagObj_Sphere"))
		{
			if (sphere.GetComponent<SphereMagnetism>())
			{
				SphereMagnetism.Register(this);
			}
		}

		// �L���[�u
		foreach (var cube in GameObject.FindGameObjectsWithTag("MagObj_Cube"))
		{
			if (cube.GetComponent<CubeMagnetism>())
			{
				CubeMagnetism.Register(this);
			}
		}
		// ���L���[�u
		foreach (var hCube in GameObject.FindGameObjectsWithTag("MagObj_HCube"))
		{
			if (hCube.GetComponent<HCubeMagnetism>())
			{
				HCubeMagnetism.Register(this);
			}
		}
	}

	private void UnregisterFromAllMagnets()
	{
		// ���������ǉ��̎��ƈꏏ�Ń^�O�𗘗p�������
		// ��
		foreach (var sphere in GameObject.FindGameObjectsWithTag("MagObj_Sphere"))
		{
			if (sphere.GetComponent<SphereMagnetism>())
			{
				SphereMagnetism.Unregister(this);
			}
		}

		// �L���[�u
		foreach (var cube in GameObject.FindGameObjectsWithTag("MagObj_Cube"))
		{
			if (cube.GetComponent<CubeMagnetism>())
			{
				CubeMagnetism.Unregister(this);
			}
		}
		// ���L���[�u
		foreach (var hCube in GameObject.FindGameObjectsWithTag("MagObj_HCube"))
		{
			if (hCube.GetComponent<HCubeMagnetism>())
			{
				HCubeMagnetism.Unregister(this);
			}
		}
	}
}
