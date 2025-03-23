using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// ����ҁ@�{�{�a��

// ���U���̋����𒲐����鎞�̖ڈ�
// 0.002���ŏ��l ����ȏ�Ⴍ����ƐU�����Ȃ��i�R���g���[���[�ɂ���č�����H�������j
// 1.0���ő�l�i���Ԃ�j�������`�`�Ȃ苭�������̂ł����Ɛ��l�������������傤�ǂ����A�Ƃ������肪����ǂ��Ȃ�

public class Vibration : MonoBehaviour
{
	[Header("�v���C���[�̎���(���s��)")]
	public Magnetism magnet1;
	public Magnetism magnet2;

	[Header("�`�F�b�N�ŐU���I�t")]
	public bool notVibration = false;   // �f�o�b�O���U�����������Ȃ�����`�F�b�N

	private Gamepad gamepad;
	private Coroutine vibrationCoroutine;

	// Start is called before the first frame update
	void Start()
	{
		gamepad = Gamepad.current;

		if (gamepad == null) { return; }		// �R���g���[���[���Ȃ����̓X���[
		else { gamepad.SetMotorSpeeds(0, 0); }	// �N�����ɓ�̐U�����N����̂�}��

		//--- �����ɑ����Ă����Ε����̐U�����R���g���[���ł���H ---//
		if (!notVibration)
		{
			vibrationCoroutine = StartCoroutine(Vibration_MagnetDistance());	// ���΂̋����ɉ������U��
		}
	}


	//=================================================
	// ���΂̋����ɉ����ĐU��������֐�
	//=================================================
	IEnumerator Vibration_MagnetDistance()
	{
		while (true)
		{
			float distance = Vector3.Distance(magnet1.transform.position, magnet2.transform.position);	// ���΂̋���

			float minDistance = magnet1.DeadRange;
			float maxDistance = magnet1.MagnetismRange;
			float vibStrength;  // �U���̋���
			float vibInterval;  // �U���̊Ԋu

			//--- ���l�̕���ύX����Ȃ�R�R ---//
			// �U���̋���
			float minVibStrength = 0.002f;	// ����
			float maxVibStrength = 0.03f;	// �߂�

			// �U���̊Ԋu
			float minVibInterval = 0.7f;	// �߂�
			float maxVibInterval = 1.5f;	// ����


			if (distance <= minDistance)	// �߂����遨�ő�U��
			{
				vibStrength = maxVibStrength;
				vibInterval = minVibInterval;

				if (magnet1.GetComponent<Magnetism>().isSnapping)	// ����������U���؂�
				{
					vibStrength = 0.0f;
				}
			}
			else if (distance >= maxDistance)	// �������遨�U���Ȃ�
			{
				vibStrength = 0.0f;
				vibInterval = 0.0f;
			}
			else	// �͈͓��������ɉ����ĐU�����x�ƊԊu��ς���
			{
				float t = (distance - minDistance) / (maxDistance - minDistance);
				vibStrength = Mathf.Lerp(maxVibStrength, minVibStrength, t);
				vibInterval = Mathf.Lerp(minVibInterval, maxVibInterval, t);
			}

			// �R���g���[���[��U��������
			gamepad.SetMotorSpeeds(vibStrength, vibStrength);
			yield return new WaitForSeconds(0.05f);
			gamepad.SetMotorSpeeds(0.0f, 0.0f);
			yield return new WaitForSeconds(vibInterval);
		}
	}

	void OnDestroy()
	{
		if (gamepad != null)
		{
			gamepad.SetMotorSpeeds(0, 0);	// �Q�[���I�����ɐU�����~�߂�
		}
	}
}
