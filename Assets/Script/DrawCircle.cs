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
	public GameObject Circles;      // �~�̃O���[�v

	[Header("�͈͕\���̉~")]
	public Transform magnetismCircle;	// ���͔͈͂̕�
	public Transform deadCircle;		// �������͈͂̕�

	[Header("�\����Fplate�i�������̓I�u�W�F�N�g���g�j")]
	public GameObject baseObj;
	// �v���C���[�̎��΂̏ꍇdeadRange��plate�ɒǏ]����������R

	private Magnetism mag;
	private SphereMagnetism sMag;
	private AdjustMagnetism adjMag;

	// Start is called before the first frame update
	void Start()
	{
		adjMag = GameObject.Find("Main Camera").GetComponent<AdjustMagnetism>();
		mag = GetComponent<Magnetism>();

		if (mag != null) return;	// Magnetism������Ȃ�ȉ��̓X���[
		sMag = GetComponent<SphereMagnetism>();
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
			return;	// ���͒����I���܂ŃX���[
		}
		else
		{
			Circles.SetActive(true);	// ��\������
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
				magnetismCircle.localScale = new Vector3(sMag.MagnetismRange / 2, 0.01f, sMag.MagnetismRange / 2);
			}
			magnetismCircle.position = this.transform.position;		// �ʒu�̒Ǐ]
			magnetismCircle.rotation = Quaternion.identity; // ��]���Œ�
		}

		// �������͈�
		if (deadCircle != null)
		{
			if (mag != null)
			{
				deadCircle.localScale = new Vector3(mag.DeadRange * 1.1f, 0.01f, mag.DeadRange * 1.1f);
			}
			else
			{
				deadCircle.localScale = new Vector3(sMag.DeadRange * 1.2f, 0.01f, sMag.DeadRange * 1.2f);
			}
			deadCircle.position = baseObj.transform.position;
			deadCircle.rotation = Quaternion.identity;
		}
	}
}
