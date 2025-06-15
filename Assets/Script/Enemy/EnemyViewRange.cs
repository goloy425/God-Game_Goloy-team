using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

//==========================================================
// 作成者：宮本和音
// 敵の視野範囲を管理するスクリプト
//==========================================================

public class EnemyViewRange : MonoBehaviour
{
	public float viewRange = 10.0f;
	public bool playerDetected;		// プレイヤー発見

	[Header("プレイヤーを設定（順不同）")]
	public Object[] Player;

	private void OnTriggerStay(Collider col)
	{
		if (!col.CompareTag("Player")) return;

		playerDetected = false;

		// 視線の開始位置（敵の目の高さ）
		Vector3 eyePosition = transform.position + Vector3.up * 0.01f;

		// プレイヤーのポイント（頭・足元・肩）
		Vector3[] targetPoints = new Vector3[]
		{
				col.transform.position + Vector3.up * 1.55f,	// 頭
				col.transform.position - Vector3.up * 1.0f,		// 足元
				col.transform.position + Vector3.up * 0.5f + col.transform.right * 0.43f,	// 右肩
				col.transform.position + Vector3.up * 0.5f - col.transform.right * 0.43f	// 左肩
		};

		// レイをプレイヤーのポイントに飛ばす処理
		foreach (Vector3 target in targetPoints)
		{
			Vector3 dir = (target - eyePosition).normalized;
			Debug.DrawRay(eyePosition, dir * viewRange, Color.red);

			if (Physics.Raycast(eyePosition, dir, out RaycastHit hit, viewRange))
			{
				if (hit.collider.CompareTag("Player"))
				{
					playerDetected = true;
					break;	// 1本でも当たったら発見
				}
			}
		}
	}
}
