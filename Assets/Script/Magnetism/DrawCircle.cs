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
	public GameObject Circles;	// �~�̃O���[�v

	[Header("�͈͕\���̉~")]
	public Transform magnetismCircle;	// ���͔͈͂̕�
	public Transform deadCircle;		// �������͈͂̕�

	[Header("�\����Fplate�i�������̓I�u�W�F�N�g���g�j")]
	public GameObject baseObj;
	// ��deadRange�͊�{�I�ɃI�u�W�F�N�g�̒��S���琶���Ă�������R
	// �ł��v���C���[�̎��΂̏ꍇ��plate�ɒǏ]����������R
	
	private Magnetism mag;
	private SphereMagnetism sMag;
	private CubeMagnetism cMag;
	
	private HCubeMagnetism hcMag;
	private bool isHCube = false;	// ���L���[�u���ǂ����i�����O�͉~��\���j

	private AdjustMagnetism adjMag;
	private SplitCube sCube;

	// Start is called before the first frame update
	void Start()
	{
		adjMag = GameObject.Find("Main Camera").GetComponent<AdjustMagnetism>();
		TryGetComponent<SplitCube>(out sCube);

		// Magnetism���A�^�b�`����Ă���i���v���C���[�̎��΂ł���j�ꍇ
		if (TryGetComponent<Magnetism>(out mag)) { return; }

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
		UpdateCircles();
	}

	void UpdateCircles()
	{
		if (adjMag.Adjusted)
		{
			Circles.SetActive(false);	// �~���\���ɂ���
			return;		// ���͒����I���܂ŃX���[
		}
		else
		{
			if (!isHCube)	// �L���[�u�i�����O�j�łȂ���Δ�\������
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

		//--- �͈͕\���̉~ ---//
		// ���͔͈�
		if (magnetismCircle != null)
		{
			if (mag != null)	// magnetism�t���i=�v���C���[�̎��΁j�̏ꍇ
			{
				// �T�C�Y�X�V�i*1.1�̗��R�F�Ȃ��̂����A���͈͂����ǎ��o�I�ɂ͂���Ȋ����j
				magnetismCircle.localScale = new Vector3(mag.magnetismRange * 1.1f, 0.01f, mag.magnetismRange * 1.1f);
			}
			else	// magnetism���t���ĂȂ��i=���̓I�u�W�F�N�g�j�̏ꍇ
			{
				if (sMag != null)	// ��
				{
					magnetismCircle.localScale = new Vector3(sMag.MagnetismRange * 1.2f, 0.01f, sMag.MagnetismRange * 1.2f);
				}
				else if (cMag != null)	// �L���[�u
				{
					magnetismCircle.localScale = new Vector3(cMag.MagnetismRange * 2, 0.01f, cMag.MagnetismRange * 2);
				}
				else if (hcMag != null)	// ���L���[�u
				{
					magnetismCircle.localScale = new Vector3(hcMag.MagnetismRange, 0.01f, hcMag.MagnetismRange);
				}
			}

			// �ʒu�̒Ǐ]����]���Œ�
			magnetismCircle.SetPositionAndRotation(this.transform.position, Quaternion.identity);
		}

		// �������͈�
		if (deadCircle != null)
		{
			if (mag != null)
			{
				deadCircle.localScale = new Vector3(mag.deadRange * 1.1f, 0.01f, mag.deadRange * 1.1f);
			}
			else
			{
				if (sMag != null)	// ��
				{
					deadCircle.localScale = new Vector3(sMag.DeadRange * 1.2f, 0.01f, sMag.DeadRange * 1.2f);
				}
				else if (cMag != null)	// �L���[�u
				{
					deadCircle.localScale = new Vector3(cMag.DeadRange * 4.0f, 0.01f, cMag.DeadRange * 4.0f);
				}
				else if (hcMag != null) // ���L���[�u
				{
					magnetismCircle.localScale = new Vector3(hcMag.DeadRange, 0.01f, hcMag.DeadRange);
				}
			}

			deadCircle.SetPositionAndRotation(baseObj.transform.position, Quaternion.identity);
		}
	}
}
