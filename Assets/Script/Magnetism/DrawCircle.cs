using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=================================================
// �쐬�ҁF�{�{�a��
// ���͔͈͂�\������X�N���v�g
//=================================================

public class DrawCircle : MonoBehaviour
{
	[Header("Circles��ݒ�i�\���E��\���؂�ւ��p�j")]
	public GameObject Circles;  // �~�̃O���[�v

	[Header("�͈͕\���̉~")]
	public Transform magnetismCircle;   // ���͔͈͂̕�
	public Transform deadCircle;        // �������͈͂̕�

	[Header("�\����Fplate�i�������̓I�u�W�F�N�g���g�j")]
	public GameObject baseObj;
	// ��deadRange�͊�{�I�ɃI�u�W�F�N�g�̒��S���琶���Ă�������R
	// �ł��v���C���[�̎��΂̏ꍇ��plate�ɒǏ]����������R

	[Header("����ݒ�")]
	public GameObject floor;

	// �e�I�u�W�F�N�g�̎��̓X�N���v�g
	private Magnetism mag;
	private SphereMagnetism sMag;
	private CubeMagnetism cMag;

	private HCubeMagnetism hcMag;
	private bool isHCube = false;	// ���L���[�u���ǂ����i�����O�͉~��\���j

	// ���̑��X�N���v�g
	private AdjustMagnetism adjMag;		// ���͒���
	private SplitCube sCube;		// �L���[�u����
	private SwitchCircle circle;        // �~�̕\���E��\���ؑ� 

	// ���W�����p�̕ϐ��Y
	Ray ray;
	RaycastHit hit;

	private float rayDistance = 10f;    // ���C���΂�����

	[Header("���C���[�Fground��ݒ�")]
	public LayerMask ground;		// ���C�𓖂Ă�Ώۂ̃��C���[


	// Start is called before the first frame update
	void Start()
	{
		adjMag = GameObject.Find("Main Camera").GetComponent<AdjustMagnetism>();
		circle= GameObject.Find("Main Camera").GetComponent<SwitchCircle>();

		//if (GameObject.Find("Connecter") != null)
		//{
		//	GameObject.Find("Connecter").TryGetComponent<SplitCube>(out sCube);
		//}

		// �e���􂷂�I�u�W�F�N�g���Ƃ�SplitCube�X�N���v�g���擾
		// ���􂷂�I�u�W�F�N�g����������ƕ����ɔ͈͂��\������Ȃ��Ȃǂ̃o�O���������̂Œǉ����܂���
		// �R�l�N�^�[�̎��͂��̂܂܎擾
		if (gameObject.name == "Connecter")
		{
			gameObject.TryGetComponent<SplitCube>(out sCube);
		}
		// ���L���[�u�̎��̓R�l�N�^�[����擾
		if (gameObject.name == "MagObj_split1" || gameObject.name == "MagObj_split2")
		{
			if (transform.parent != null)
			{
				transform.parent.Find("Connecter").TryGetComponent<SplitCube>(out sCube);
			}
		}

		// Magnetism���A�^�b�`����Ă���i���v���C���[�̎��΂ł���j�ꍇ
		if (TryGetComponent<Magnetism>(out mag)){ return; }

		// ��̓I�u�W�F�N�g�̃^�O�ɂ���ĕ���
		if (gameObject.CompareTag("MagObj_Sphere"))
		{
			TryGetComponent<SphereMagnetism>(out sMag);
		}
		else if (gameObject.CompareTag("MagObj_Cube"))
		{
			TryGetComponent<CubeMagnetism>(out cMag);
		}
		else if (gameObject.CompareTag("MagObj_HCube"))
		{
			TryGetComponent<HCubeMagnetism>(out hcMag);
			isHCube = true;
		}
	}

	private void FixedUpdate()
	{
		UpdateCircles();    // �ʒu�E�p�x�̍X�V
	}

	void UpdateCircles()
	{ 
		//--- �\�������̕��� ---//
		if (adjMag.Adjusted)
		{
			Circles.SetActive(false);	// �~���\���ɂ���
			return;		// ���͒����I���܂ŃX���[
		}
		else
		{
			if (!isHCube)	// ���L���[�u�łȂ���Δ�\������
			{
				Circles.SetActive(true);
			}
			else if (sCube != null)
			{
				if (sCube.splited)
				{
					Circles.SetActive(true);
				}
			}
		}

		// SwitchCircle�ɂ��\���E��\���ؑ�
		if (circle.isInactive)
		{
			Circles.SetActive(false);
		}
		else
		{
			if (!isHCube)	// ���L���[�u�łȂ���Δ�\������
			{
				Circles.SetActive(true);
			}
		}

		//--- �~�̍X�V ---//	���͔͈͍X�V�̒���return�𐳏�ɒʂ����߂ɕʊ֐��ɂ���
		// ���͔͈�
		if (magnetismCircle != null)
		{
			UpdateMagCircle();
		}

		// �������͈�
		if (deadCircle != null)
		{
			UpdateDeadCircle();
		}
	}

	//=================================================
	// ���͔͈͂̍X�V
	//=================================================
	public void UpdateMagCircle()
	{
		if (mag != null)	// magnetism�t���i=�v���C���[�̎��΁j�̏ꍇ
		{
			// �T�C�Y�X�V�i*1.2�̗��R�F�Ȃ��̂����A���͈͂����ǎ��o�I�ɂ͂���Ȋ����j
			magnetismCircle.localScale = new Vector3(mag.magnetismRange * 1.2f, 0.01f, mag.magnetismRange * 1.2f);

			// ���C�����������_��肿����Ə��position�Ƃ���
			if (Physics.Raycast(ray, out hit, rayDistance, ground))
			{
				float posY = hit.point.y + 0.06f;

				// �ʒu�̒Ǐ]����]���Œ�
				magnetismCircle.SetPositionAndRotation(new Vector3(hit.point.x, posY, hit.point.z), Quaternion.identity);
			}
			return;
		}
		else	// magnetism���t���ĂȂ��i=���̓I�u�W�F�N�g�j�̏ꍇ
		{
			ray = new Ray(this.transform.position, Vector3.down);   // �I�u�W�F�N�g�̒����Ƀ��C�𓊂���

			if (sMag != null)	// ��
			{
				magnetismCircle.localScale = new Vector3(sMag.MagnetismRange * 1.2f, 0.01f, sMag.MagnetismRange * 1.2f);
			}
			else if (!sCube.splited && cMag != null)	// �L���[�u
			{
				magnetismCircle.localScale = new Vector3(cMag.MagnetismRange * 2, 0.01f, cMag.MagnetismRange * 2);
			}
			else if (sCube.splited && hcMag != null)	// ���L���[�u
			{
				magnetismCircle.localScale = new Vector3(hcMag.MagnetismRange * 2, 0.01f, hcMag.MagnetismRange * 2);
			}
		}

		if (Physics.Raycast(ray, out hit, rayDistance, ground))
		{
            float posY = hit.point.y + 0.02f;

            // �ʒu�̒Ǐ]����]���Œ�
            magnetismCircle.SetPositionAndRotation(new Vector3(hit.point.x, posY, hit.point.z), Quaternion.identity);
        }
	}


	//=================================================
	// �������͈͂̍X�V
	//=================================================
	private void UpdateDeadCircle()
	{
		ray = new Ray(baseObj.transform.position, Vector3.down);

		if (mag != null)
		{
			// �T�C�Y�X�V
			deadCircle.localScale = new Vector3(mag.deadRange, 0.01f, mag.deadRange);

			// ���C�����������_��position�Ƃ���
			if (Physics.Raycast(ray, out hit, rayDistance, ground))
			{
				float posY = hit.point.y + 0.1f;

				// �ʒu�̒Ǐ]����]���Œ�
				deadCircle.SetPositionAndRotation(new Vector3(hit.point.x, posY, hit.point.z), Quaternion.identity);
			}
			return;
		}
		else
		{
			if (sMag != null)	// ��
			{
				deadCircle.localScale = new Vector3(sMag.DeadRange * 3.0f, 0.01f, sMag.DeadRange * 3.0f);
			}
			else if (!sCube.splited && cMag != null)	// �L���[�u
			{
				deadCircle.localScale = new Vector3(cMag.DeadRange * 4.0f, 0.01f, cMag.DeadRange * 4.0f);
			}
			else if (sCube.splited && hcMag != null)	// ���L���[�u
			{
				deadCircle.localScale = new Vector3(hcMag.DeadRange * 1.5f, 0.01f, hcMag.DeadRange * 1.5f);
			}
		}

		if (Physics.Raycast(ray, out hit, rayDistance, ground))
		{
			float posY = hit.point.y + 0.1f;

			// �ʒu�̒Ǐ]����]���Œ�
			deadCircle.SetPositionAndRotation(new Vector3(hit.point.x, posY, hit.point.z), Quaternion.identity);
		}
	}
}
