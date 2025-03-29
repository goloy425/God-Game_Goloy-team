using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=================================================
// 作成者：宮本和音
// 磁力範囲を表示するスクリプト
//=================================================

public class DrawCircle : MonoBehaviour
{
	[Header("Circlesを設定（表示・非表示切り替え用）")]
	public GameObject Circles;      // 円のグループ

	[Header("範囲表示の円")]
	public Transform magnetismCircle;	// 磁力範囲の方
	public Transform deadCircle;        // くっつく範囲の方

	[Header("直下のplate")]
	public GameObject Plate;	// deadRangeはこっちに追従する方が自然

	private Magnetism mag;
	private AdjustMagnetism adjMag;

	// Start is called before the first frame update
	void Start()
	{
		adjMag = GameObject.Find("Main Camera").GetComponent<AdjustMagnetism>();
		mag = GetComponent<Magnetism>();
	}

	private void FixedUpdate()
	{
		UpdateCircles();
	}

	void UpdateCircles()
	{
		if (adjMag.Adjusted)
		{
			Circles.SetActive(false);   // 円を非表示にする
			return; // 磁力調整終わるまでスルー
		}
		else
		{
			Circles.SetActive(true);	// 非表示解除
		}

		//--- 範囲表示の円 ---//
		// 磁力範囲
		if (magnetismCircle != null)
		{
			// サイズ更新（*1.1の理由：ないのがリアル範囲だけど視覚的にはこんな感じ）
			magnetismCircle.localScale = new Vector3(mag.magnetismRange * 1.1f, 0.01f, mag.magnetismRange * 1.1f);
			magnetismCircle.position = this.transform.position;		// 位置の追従
			magnetismCircle.rotation = Quaternion.identity; // 回転を固定
		}

		// くっつく範囲
		if (deadCircle != null)
		{
			deadCircle.localScale = new Vector3(mag.DeadRange * 1.1f, 0.01f, mag.DeadRange * 1.1f);
			deadCircle.position = Plate.transform.position;
			deadCircle.rotation = Quaternion.identity;
		}
	}
}
