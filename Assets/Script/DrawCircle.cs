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
	public GameObject Circles;	// 円のグループ

	[Header("範囲表示の円")]
	public Transform magnetismCircle;	// 磁力範囲の方
	public Transform deadCircle;		// くっつく範囲の方

	[Header("表示基準：plate（もしくはオブジェクト自身）")]
	public GameObject baseObj;
	// プレイヤーの磁石の場合deadRangeはplateに追従する方が自然

	private Magnetism mag;
	private SphereMagnetism sMag;
	private AdjustMagnetism adjMag;

	// Start is called before the first frame update
	void Start()
	{
		adjMag = GameObject.Find("Main Camera").GetComponent<AdjustMagnetism>();

		if (TryGetComponent<Magnetism>(out mag)) { return; }
		// Magnetismがない(=磁力オブジェクトにアタッチされている)時だけSphereMagnetismを探す
		// 磁力オブジェクト（キューブ）を実装する時書き換えなきゃいけない
		// {}内にGetComponentを収めると{}出た時にNullになっちゃうので注意すること
		TryGetComponent<SphereMagnetism>(out sMag);
	}

	private void FixedUpdate()
	{
		UpdateCircles();
	}

	void UpdateCircles()
	{
		if (adjMag.Adjusted)
		{
			Circles.SetActive(false);	// 円を非表示にする
			return;	// 磁力調整終わるまでスルー
		}
		else
		{
			Circles.SetActive(true);	// 非表示解除
		}

		//--- 範囲表示の円 ---//
		// 磁力範囲
		if (magnetismCircle != null)
		{
			if (mag != null)	// magnetism付き（=プレイヤーの磁石）の場合
			{
				// サイズ更新（*1.1の理由：ないのがリアル範囲だけど視覚的にはこんな感じ）
				magnetismCircle.localScale = new Vector3(mag.magnetismRange * 1.1f, 0.01f, mag.magnetismRange * 1.1f);
			}
			else	// magnetismが付いてない（=磁力オブジェクト）の場合
			{
				magnetismCircle.localScale = new Vector3(sMag.MagnetismRange / 2, 0.01f, sMag.MagnetismRange / 2);
			}

			// 位置の追従＆回転を固定
			magnetismCircle.SetPositionAndRotation(this.transform.position, Quaternion.identity);
		}

		// くっつく範囲
		if (deadCircle != null)
		{
			if (mag != null)
			{
				deadCircle.localScale = new Vector3(mag.deadRange * 1.1f, 0.01f, mag.deadRange * 1.1f);
			}
			else
			{
				deadCircle.localScale = new Vector3(sMag.DeadRange * 1.2f, 0.01f, sMag.DeadRange * 1.2f);
			}

			deadCircle.SetPositionAndRotation(baseObj.transform.position, Quaternion.identity);
		}
	}
}
