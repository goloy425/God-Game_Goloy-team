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
	public Transform deadCircle;        // �������͈͂̕�

	[Header("������plate")]
	public GameObject Plate;	// deadRange�͂������ɒǏ]����������R

	private Magnetism mag;
	private AdjustMagnetism adjMag;

	// Start is called before the first frame update
	void Start()
	{
		adjMag = GameObject.Find("Main Camera").GetComponent<AdjustMagnetism>();
		mag = GetComponent<Magnetism>();
	}

	private void FixedUpdate()
	{
		UpdateCircles();
	}

	void UpdateCircles()
	{
		if (adjMag.Adjusted)
		{
			Circles.SetActive(false);   // �~���\���ɂ���
			return; // ���͒����I���܂ŃX���[
		}
		else
		{
			Circles.SetActive(true);	// ��\������
		}

		//--- �͈͕\���̉~ ---//
		// ���͔͈�
		if (magnetismCircle != null)
		{
			// �T�C�Y�X�V�i*1.1�̗��R�F�Ȃ��̂����A���͈͂����ǎ��o�I�ɂ͂���Ȋ����j
			magnetismCircle.localScale = new Vector3(mag.magnetismRange * 1.1f, 0.01f, mag.magnetismRange * 1.1f);
			magnetismCircle.position = this.transform.position;		// �ʒu�̒Ǐ]
			magnetismCircle.rotation = Quaternion.identity; // ��]���Œ�
		}

		// �������͈�
		if (deadCircle != null)
		{
			deadCircle.localScale = new Vector3(mag.DeadRange * 1.1f, 0.01f, mag.DeadRange * 1.1f);
			deadCircle.position = Plate.transform.position;
			deadCircle.rotation = Quaternion.identity;
		}
	}
}
