using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

//=================================================
// �쐬�ҁF�{�{�a��
// �ړ����̃X�N���v�g
//=================================================

// ���Y�^�@�ړ����̍���
// �p�ӂ�����́FMovingFloor�iprefab�j�A�ړ��G���A�������������L���[�u�AWaypoint�Ɩ��t������I�u�W�F�N�g�Œ�2��
// �@Waypoint�͈ړ����̒ʂ���W���������́A���W�����C��t���邱��
// �A�ړ��G���A�̍��W��������Navigation�^�u��Bake���ړ��G���A�̐���
// �I �ړ��G���A�̃T�C�Y��Waypoint�̈ʒu�𕢂��邭�炢�̃T�C�Y�ɂ��邱��
// �I ���W��������ꍇ�͈ړ��G���A�𓮂���������Bake�������Ȃ��Ɣ��f����Ȃ�
// �BInspecter�^�u��Waypoint�̓o�^����Γ����o���͂��@���Ƃׂ͍������W�̒����A�v���C���[�������|����Ȃ��ʒu

public class MovingFloor : MonoBehaviour
{
	[Header("�V�[������Waypoint��ݒ�")]
	public Transform[] waypArray;

	[Header("�n�_�E�I�_�Ŏ~�܂鎞��")]
	public float stayTime = 2.0f;
	private float stayTimer;    // �v���p

	[Header("�o�R�_�����鎞�F�o�R�_�ł��~�߂邩�ǂ���")]
	public bool stopAtViaP = false;

	private float stayTime_ViaP = 0.05f;	// �o�R�_������ꍇ�A�o�R�_�Ŏ~�܂鎞��
											// �i1��~�܂�Ȃ��ƏI�_�Ɍ����Čʂ�`���݂����Ɋۂ��i�ނ̂ŃL�����j

	private NavMeshAgent nma;
	private int currentWaypNum;		// �ړI�Ƃ���Waypoint���Ǘ�����

	private bool isViaP = false;	// �o�R�_�����邩�ǂ���
	private bool isGoing = false;	// �z��Ǘ��@�������i��ł邩�ǂ���

	// Start is called before the first frame update
	void Start()
	{
		// NavMeshAgent�R���|�[�l���g���擾
		nma = GetComponent<NavMeshAgent>();

		// waypArray�̗v�f����3�ȏぁ�n�_�E�I�_�ȊO��waypoint������
		if (waypArray.Length >= 3)
		{
			isViaP = true;
			isGoing = true;
		}
	}

	// Update is called once per frame
	void Update()
	{
		this.transform.rotation = Quaternion.identity;	// �p�x�Œ�

		// �ړI�n�_�܂ł̋������ړI�n�̎�O�܂ł̋����ȉ��ɂȂ�����
		if (nma.remainingDistance <= nma.stoppingDistance)
		{
			if (isViaP)	// �Ȃ���p������ꍇ(0��1��2��1��0��1���c)
			{
				// �n�_�ł��I�_�ł��Ȃ���
				if (currentWaypNum != 0 && currentWaypNum != waypArray.Length - 1)
				{
					stayTimer += Time.deltaTime;

					if (stopAtViaP)		// �o�R�_��
					{
						if (stayTimer < stayTime) { return; }	// �K��̎��Ԉȓ��Ȃ��̏����i���̖ړI�n�̐ݒ�j���X���[
						else { stayTimer = 0.0f; }  // �^�C�}�[�����Z�b�g
					}
					else
					{
						if (stayTimer < stayTime_ViaP) { return; }
						else { stayTimer = 0.0f; }
					}
				}
				else
				{
					stayTimer += Time.deltaTime;
					if (stayTimer < stayTime) { return; }
					// ��ƈ���ă^�C�}�[�̃��Z�b�g�������ł��ƃ^�C�}�[��2��ʂ����Ⴄ
				}

				if (isGoing)	// �O�i���Ȃ琔����i�߂Ă���
				{
					if (currentWaypNum < (waypArray.Length - 1))
					{
						currentWaypNum++;
						stayTimer = 0.0f;
					}
					else
					{
						isGoing = false;	// ��ޒ��ɐ؂�ւ�
						return;
					}
				}
				else	// ��ޒ��Ȃ琔����߂��Ă���
				{
					if (currentWaypNum > 0)
					{
						currentWaypNum--;
						stayTimer = 0.0f;
					}
					else
					{
						isGoing = true;		// �O�i���ɐ؂�ւ�
						return;
					}
				}

				// �ړI�n�Z�b�g
				nma.SetDestination(waypArray[currentWaypNum].position);
			}

			else	// �n�_�ƏI�_�����Ȃ��ꍇ(0��1)
			{
				stayTimer += Time.deltaTime;

				// �ړI�n�Ɏ~�܂��Ĉ�莞�Ԍo�����玟�̖ړI�n��ݒ肷��
				if(stayTimer > stayTime) 
				{
					stayTimer = 0f;

					if (currentWaypNum == 0)
					{
						currentWaypNum = 1;
					}
					else
					{
						currentWaypNum = 0;
					}

					// �ړI�n�Z�b�g
					nma.SetDestination(waypArray[currentWaypNum].position);
				}
			}
		}
	}
}
