using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

//=================================================
// �쐬�ҁF�{�{�a��
// �ړ����̃X�N���v�g
//=================================================

public class MovingFloor : MonoBehaviour
{
	[Header("�V�[������Waypoint��ݒ�")]
	public Transform[] waypArray;

	[Header("�n�_�E�I�_�Ŏ~�܂鎞��")]
	public float stayTime = 2.0f;
	private float stayTimer;	// �v���p

	private NavMeshAgent nma;
	public int currentWaypNum;

	public bool isCorner = false;	// �p�����邩�ǂ���
	public bool isGoing = false;	// �z��Ǘ��@�������i��ł邩�ǂ���

	// Start is called before the first frame update
	void Start()
	{
		// NavMeshAgent�R���|�[�l���g���擾
		nma = GetComponent<NavMeshAgent>();

		// waypArray�̗v�f����3�ȏぁ�n�_�E�I�_�ȊO��waypoint������
		if (waypArray.Length >= 3)
		{
			isCorner = true;
			isGoing = true;
		}

		if (isCorner)
		{
			nma.autoBraking = false;
		}
	}

	// Update is called once per frame
	void Update()
	{
		this.transform.rotation = Quaternion.identity;	// �p�x�Œ�

		// �ړI�n�_�܂ł̋������ړI�n�̎�O�܂ł̋����ȉ��ɂȂ�����
		if (nma.remainingDistance <= nma.stoppingDistance)
		{
			if (isCorner)	// �Ȃ���p������ꍇ(0��1��2��1��0��1���c)
			{
				if (isGoing)
				{
					if (currentWaypNum < (waypArray.Length - 1))
					{
						currentWaypNum++;
					}
					else
					{
						isGoing = false;
						return;
					}
				}
				else
				{
					if (currentWaypNum > 0)
					{
						currentWaypNum--;
					}
					else
					{
						isGoing = true;
						return;
					}
				}

				nma.SetDestination(waypArray[currentWaypNum].position);
			}

			else	// �n�_�ƏI�_�����Ȃ��ꍇ(0��1)
			{
				stayTimer += Time.deltaTime;

				// ��莞�Ԍo�����玟�̖ړI�n��ݒ肷��
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

					nma.SetDestination(waypArray[currentWaypNum].position);
				}
			}
		}
	}
}
