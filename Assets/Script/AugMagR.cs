using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugMagR : MonoBehaviour
{
	[Header("PlayerR�̎��΂�ݒ�")]
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
		float RValue = inputs.PlayerR.AugmentMag.ReadValue<float>();

		// ������Ă鋭���Ŏ��͋����ɓ��邩�ǂ�������
		if (RValue > 0.5f)
		{
			AugmentPlayerRMagnetism();
		}

	}

	private void AugmentPlayerRMagnetism()
	{
		Debug.Log("ZR�L�[������Ă܁[��");
	}

	private void OnDestroy()
	{
		inputs?.Dispose();
	}
}
