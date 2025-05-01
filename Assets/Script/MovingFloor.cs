using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

//=================================================
// 作成者：宮本和音
// 移動床のスクリプト
//=================================================

public class MovingFloor : MonoBehaviour
{
	[Header("シーン中のWaypointを設定")]
	public Transform[] waypArray;

	[Header("始点・終点で止まる時間")]
	public float stayTime = 2.0f;
	private float stayTimer;	// 計測用

	private NavMeshAgent nma;
	public int currentWaypNum;

	public bool isCorner = false;	// 角があるかどうか
	public bool isGoing = false;	// 配列管理　数字が進んでるかどうか

	// Start is called before the first frame update
	void Start()
	{
		// NavMeshAgentコンポーネントを取得
		nma = GetComponent<NavMeshAgent>();

		// waypArrayの要素数が3以上＝始点・終点以外にwaypointがある
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
		this.transform.rotation = Quaternion.identity;	// 角度固定

		// 目的地点までの距離が目的地の手前までの距離以下になったら
		if (nma.remainingDistance <= nma.stoppingDistance)
		{
			if (isCorner)	// 曲がり角がある場合(0→1→2→1→0→1→…)
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

			else	// 始点と終点しかない場合(0⇔1)
			{
				stayTimer += Time.deltaTime;

				// 一定時間経ったら次の目的地を設定する
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
