using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//============================================================
// �쐬�ҁF�{�{�a��
// �ړ����Ńv���C���[�E���̑��I�u�W�F�N�g���^�ԃX�N���v�g
//============================================================

public class CarryObjOnFloor : MonoBehaviour
{
	// ���ɂ͏�ɏ���Ă邱�Ƃ𔻕ʂ��邽�߂̃R���C�_�[(isTrigger���I��)�ƃv���C���[�̂߂肱�݂�h���p�̃R���C�_�[
	// �v2�̃R���C�_�[���A�^�b�`���Ă�������

	void OnTriggerEnter(Collider other)
	{
		// ����Ă����I�u�W�F�N�g�̃^�O���w��̂��̂Ȃ�q�I�u�W�F�N�g��
		if (other.CompareTag("Player") || other.CompareTag("MagObj_Sphere") || other.CompareTag("MagObj_HCube"))
		{
			other.transform.SetParent(this.transform);
		}
	}

	void OnTriggerExit(Collider other)
	{
		// �ォ��o�čs�����I�u�W�F�N�g�̃^�O���w��̂��̂Ȃ�e�q�֌W������
		if (other.CompareTag("Player") || other.CompareTag("MagObj_Sphere") || other.CompareTag("MagObj_HCube"))
		{
			other.transform.SetParent(null);
		}
	}
}
