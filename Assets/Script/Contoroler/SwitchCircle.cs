using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=======================================================================
// �쐬�ҁF�{�{�a��
// �~�L�[�i�㉺���E4����{�^���̉��̂�j�ŊeCircle���\���ɂ���
//=======================================================================

public class SwitchCircle : MonoBehaviour
{
	[Header("��\���t���O�i�f�o�b�O�p�j")]
	public bool isInactive;		// ���ꂪtrue�Ȃ�V�[������Circle��S����A�N�e�B�u�ɂ���

	private GameInputs inputs;  // GameInputs�N���X

	private bool prevFg;
	private bool nowFg;

	// Start is called before the first frame update
	void Start()
	{
		inputs = new GameInputs();
		inputs.Enable();
	}

	// Update is called once per frame
	void Update()
	{
		bool key = inputs.OtherKey.InactiveCircle.IsPressed();  // �L�[���͎擾
		nowFg = key;	// �t���O���f

		// �L�[���͂Ńt���O�̐؂�ւ�
		if (nowFg && !prevFg)
		{
			if (isInactive)
			{
				isInactive = false;
			}
			else
			{
				isInactive = true;
			}
		}

		prevFg = nowFg;		// �t���O�X�V
	}

	private void OnDestroy()
	{
		inputs?.Dispose();
	}
}
