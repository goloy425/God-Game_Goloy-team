using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

//=================================================
// �쐬�ҁF�{�{�a��
// �L���[�u���ӂ��Ɋ���X�N���v�g
//=================================================


public class SplitCube : MonoBehaviour
{
	[Header("�v���C���[�I�u�W�F�N�g��ݒ�")]
	public GameObject playerL;
	public GameObject playerR;

	[Header("�������Ă�L���[�u��ݒ�")]
	public GameObject cube1;
	public GameObject cube2;

	[Header("�L���[�u�������ł��邩�ǂ���")]
	public bool canSplit;
	public bool splited;	// ���������炱������true�ɂȂ�

	[Header("�L���[�u�����Ɋ֌W���鐔�l")]
	public float requiredDistance = 2.0f;	// ���΂�������x����Ă邩
	public float alignThreshold = 180.0f;   // �v���C���[���΂�Cube�̈ʒu���قڈ꒼���Ȃ�OK

	// ��������Ɏg���p
	private Vector3 initialL;
	private Vector3 initialR;
	public bool isTracking = false;

	// �������FixedJoint����������p
	private FixedJoint jointL;
	private FixedJoint jointR;

	// ������Ɏ��͂��I���ɂ���p
	private HCubeMagnetism cMag1;
	private HCubeMagnetism cMag2;

	// ���͋����t���O�����ꂼ��擾����p
	private AugMagL magL_Aug;
	private AugMagR magR_Aug;

	// DeadRange���擾�E�ݒ肷��p
	private GameObject magnet1;
	private GameObject magnet2;
	private Magnetism mag1;
	private Magnetism mag2;
	private float oridinalDRange;

	// Start is called before the first frame update
	void Start()
	{
		playerL.TryGetComponent<AugMagL>(out magL_Aug);
		playerR.TryGetComponent<AugMagR>(out magR_Aug);

		magnet1 = GameObject.Find("Magnet1");
		magnet2 = GameObject.Find("Magnet2");
		mag1 = magnet1.GetComponent<Magnetism>();
		mag2 = magnet2.GetComponent<Magnetism>();

		jointL = cube1.GetComponent<FixedJoint>();
		jointR = cube2.GetComponent<FixedJoint>();
		cMag1 = cube1.GetComponent<HCubeMagnetism>();
		cMag2 = cube2.GetComponent<HCubeMagnetism>();

		oridinalDRange = mag1.deadRange;	// �{����deadRange��ۑ����Ă���
	}

	// Update is called once per frame
	void Update()
	{
		//--- �L���[�u�𕪊��ł��邩�ǂ����̔��� ---//
		if (!isTracking && magL_Aug.isAugmenting && magR_Aug.isAugmenting)  // �ǂ�����������ԂɂȂ��Ă邩�ǂ����m�F
		{
			canSplit = true;
			isTracking = true;
			initialL = playerL.transform.position;
			initialR = playerR.transform.position;

			// �����������̓v���C���[�̎��Γ��m�ł������Ȃ��悤�ɂ��邽��deadRange��0�ɂ��Ă���
			mag1.SetDeadRange(0.0f, this);
			mag2.SetDeadRange(0.0f, this);
		}
		else if(!magL_Aug.isAugmenting || !magR_Aug.isAugmenting)
		{
			isTracking = false;
			canSplit = false;

			mag1.SetDeadRange(oridinalDRange, this);
			mag2.SetDeadRange(oridinalDRange, this);
		}
	}

	void FixedUpdate()
	{
		if (!canSplit || !isTracking || splited) return;

		float movedL = Vector3.Distance(initialL, playerL.transform.position);
		float movedR = Vector3.Distance(initialR, playerR.transform.position);

		Vector3 toLeft = magnet1.transform.position - transform.position;
		Vector3 toRight = magnet2.transform.position - transform.position;

		float angle = Vector3.Angle(toLeft, -toRight);
		Debug.Log("movedL:" + movedL + " movedR" + movedR);

		if (movedL > requiredDistance && movedR > requiredDistance && Mathf.Abs(angle - 180f) > alignThreshold)
		{
			BreakCube();
		}
	}

	void BreakCube()
	{
		splited = true;

		// �L���[�u��FixedJoint����������
		Destroy(jointL);
		Destroy(jointR);

		// ���L���[�u�̎��̓X�N���v�g���A�N�e�B�u��
		cMag1.enabled = true;
		cMag2.enabled = true;

		Debug.Log("�L���[�u�܂��Ղ��I");
		this.gameObject.SetActive(false);
	}
}
