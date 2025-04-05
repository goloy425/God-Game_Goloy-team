using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.InputSystem;

//=================================================
// ����ҁ@�{�{�a��
// ZL�EZR�Ŏ��͋�������X�N���v�g
//=================================================

public class AugMagL : MonoBehaviour
{
	[Header("PlayerL�̎��΂�ݒ�")]
	public Magnetism magnet;

	private GameInputs inputs;		// GameInputs�N���X

	// Start is called before the first frame update
	void Start()
	{
		inputs = new GameInputs();
		inputs.Enable();
	}

	// Update is called once per frame
	void Update()
	{
		// �{�^����������Ă鋭���̎擾
		float LValue = inputs.PlayerL.AugmentMag.ReadValue<float>();

		// ������Ă鋭���Ŏ��͋����ɓ��邩�ǂ�������
		if (LValue > 0.5f)
		{
			AugmentPlayerLMagnetism();
		}
	}

	private void AugmentPlayerLMagnetism()
	{
		//Debug.Log("ZL�L�[������Ă܁[��");
	}

	private void OnDestroy()
	{
		inputs?.Dispose();
	}
}
