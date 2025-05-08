using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

//=================================================
// 作成者：宮本和音
// 移動床のスクリプト
//=================================================

// 備忘録　移動床の作り方
// 用意するもの：MovingFloor（prefab）、移動エリアを示す平たいキューブ、Waypointと名付けた空オブジェクト最低2つ
// ①Waypointは移動床の通る座標を示すもの、座標調整気を付けること
// ②移動エリアの座標調整してNavigationタブでBake→移動エリアの生成
// ！ 移動エリアのサイズはWaypointの位置を覆えるくらいのサイズにすること
// ！ 座標調整する場合は移動エリアを動かしたあとBakeし直さないと反映されない
// ③InspecterタブでWaypointの登録すれば動き出すはず　あとは細かく座標の調整、プレイヤーが引っ掛からない位置

public class MovingFloor : MonoBehaviour
{
	[Header("シーン中のWaypointを設定")]
	public Transform[] waypArray;

	[Header("始点・終点で止まる時間")]
	public float stayTime = 2.0f;
	private float stayTimer;    // 計測用

	[Header("経由点がある時：経由点でも止めるかどうか")]
	public bool stopAtViaP = false;

	private float stayTime_ViaP = 0.05f;	// 経由点がある場合、経由点で止まる時間
											// （1回止まらないと終点に向けて弧を描くみたいに丸く進むのでキモい）

	private NavMeshAgent nma;
	private int currentWaypNum;		// 目的とするWaypointを管理する

	private bool isViaP = false;	// 経由点があるかどうか
	private bool isGoing = false;	// 配列管理　数字が進んでるかどうか

	// Start is called before the first frame update
	void Start()
	{
		// NavMeshAgentコンポーネントを取得
		nma = GetComponent<NavMeshAgent>();

		// waypArrayの要素数が3以上＝始点・終点以外にwaypointがある
		if (waypArray.Length >= 3)
		{
			isViaP = true;
			isGoing = true;
		}
	}

	// Update is called once per frame
	void Update()
	{
		this.transform.rotation = Quaternion.identity;	// 角度固定

		// 目的地点までの距離が目的地の手前までの距離以下になったら
		if (nma.remainingDistance <= nma.stoppingDistance)
		{
			if (isViaP)	// 曲がり角がある場合(0→1→2→1→0→1→…)
			{
				// 始点でも終点でもない時
				if (currentWaypNum != 0 && currentWaypNum != waypArray.Length - 1)
				{
					stayTimer += Time.deltaTime;

					if (stopAtViaP)		// 経由点で
					{
						if (stayTimer < stayTime) { return; }	// 規定の時間以内なら後の処理（次の目的地の設定）をスルー
						else { stayTimer = 0.0f; }  // タイマーをリセット
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
					// 上と違ってタイマーのリセットをここでやるとタイマーが2回通っちゃう
				}

				if (isGoing)	// 前進中なら数字を進めていく
				{
					if (currentWaypNum < (waypArray.Length - 1))
					{
						currentWaypNum++;
						stayTimer = 0.0f;
					}
					else
					{
						isGoing = false;	// 後退中に切り替え
						return;
					}
				}
				else	// 後退中なら数字を戻していく
				{
					if (currentWaypNum > 0)
					{
						currentWaypNum--;
						stayTimer = 0.0f;
					}
					else
					{
						isGoing = true;		// 前進中に切り替え
						return;
					}
				}

				// 目的地セット
				nma.SetDestination(waypArray[currentWaypNum].position);
			}

			else	// 始点と終点しかない場合(0⇔1)
			{
				stayTimer += Time.deltaTime;

				// 目的地に止まって一定時間経ったら次の目的地を設定する
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

					// 目的地セット
					nma.SetDestination(waypArray[currentWaypNum].position);
				}
			}
		}
	}
}
