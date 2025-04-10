using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=================================================
// �쐬�ҁF�{�{�a��
// ���͋����iR�j�@ZR�̕�
//=================================================

public class AugMagR : MonoBehaviour
{
	[Header("PlayerR�̎��΂�ݒ�")]
	public GameObject magnet;

	[Header("�t���O�F������")]
	public bool isAugmenting;	// ���������ǂ����̃t���O

	private GameInputs inputs;	// GameInputs�N���X
	private Magnetism mag;	// magnet��Magnetism���擾����p

	// �F�`�F���W�p�ϐ�
	private Renderer circleRenderer;
	private Color defaultColor;
	private Color poweredColor = Color.green;	// �������̐F

	// Start is called before the first frame update
	void Start()
	{
		inputs = new GameInputs();
		inputs.Enable();

		magnet.TryGetComponent<Magnetism>(out mag);

		// ���͔͈̓I�u�W�F�N�g���擾
		Transform circle = this.transform.Find("Circles/MagnetismCircle");

		if (circle != null)
		{
			circle.TryGetComponent<Renderer>(out circleRenderer);

			if (circleRenderer != null)
			{
				// �I�u�W�F�N�g�ʂ̃}�e���A���C���X�^���X���g���悤�ɖ���
				circleRenderer.material = new Material(circleRenderer.material);
				defaultColor = circleRenderer.material.color;
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		// �{�^����������Ă鋭���̎擾
		float RValue = inputs.PlayerR.AugmentMag.ReadValue<float>();

		// �I�u�W�F�N�g�̎��͔͈͓��ɂ��鎞�A���ȏ�̋����ŃL�[�������ꂽ�玥�͋���
		if (RValue > 0.3f && mag.inObjMagArea)
		{
			AugmentPlayerRMagnetism();
		}
		else
		{
			ResetPlayerRMagnet();   // �F��牽�������ɖ߂�
		}
	}

	private void AugmentPlayerRMagnetism()
	{
		Color temp = poweredColor;
		temp.a = defaultColor.a + 0.1f;	// �s�����x���኱�グ��
		circleRenderer.material.color = temp;

		isAugmenting = true;
	}

	private void ResetPlayerRMagnet()
	{
		circleRenderer.material.color = defaultColor;   // �F�����ɖ߂�
		isAugmenting = false;
	}


	private void OnDestroy()
	{
		inputs?.Dispose();
	}
}
