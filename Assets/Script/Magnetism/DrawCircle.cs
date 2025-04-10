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
	// ↑deadRangeは基本的にオブジェクトの中心から生えてる方が自然
	// でもプレイヤーの磁石の場合はplateに追従する方が自然
	
	private Magnetism mag;
	private SphereMagnetism sMag;
	private CubeMagnetism cMag;
	
	private HCubeMagnetism hcMag;
	private bool isHCube = false;	// 半キューブかどうか（割れる前は円非表示）

	private AdjustMagnetism adjMag;
	private SplitCube sCube;

	// Start is called before the first frame update
	void Start()
	{
		adjMag = GameObject.Find("Main Camera").GetComponent<AdjustMagnetism>();
		TryGetComponent<SplitCube>(out sCube);

		// Magnetismがアタッチされている（＝プレイヤーの磁石である）場合
		if (TryGetComponent<Magnetism>(out mag)) { return; }

		// 後はオブジェクトのタグによって分岐
		if (gameObject.CompareTag("MagObj_Sphere"))
		{
			TryGetComponent<SphereMagnetism>(out sMag);
		}
		else if (gameObject.CompareTag("MagObj_Cube"))
		{
			TryGetComponent<CubeMagnetism>(out cMag);
		}
		else if (gameObject.CompareTag("MagObj_HCube"))
		{
			TryGetComponent<HCubeMagnetism>(out hcMag);
			isHCube = true;
		}
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
			return;		// 磁力調整終わるまでスルー
		}
		else
		{
			if (!isHCube)	// キューブ（割れる前）でなければ非表示解除
			{
				Circles.SetActive(true);
			}
			else if (sCube != null)
			{
				if (sCube.splited)
				{
					Circles.SetActive(true);
				}
			}

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
				if (sMag != null)	// 球
				{
					magnetismCircle.localScale = new Vector3(sMag.MagnetismRange * 1.2f, 0.01f, sMag.MagnetismRange * 1.2f);
				}
				else if (cMag != null)	// キューブ
				{
					magnetismCircle.localScale = new Vector3(cMag.MagnetismRange * 2, 0.01f, cMag.MagnetismRange * 2);
				}
				else if (hcMag != null)	// 半キューブ
				{
					magnetismCircle.localScale = new Vector3(hcMag.MagnetismRange, 0.01f, hcMag.MagnetismRange);
				}
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
				if (sMag != null)	// 球
				{
					deadCircle.localScale = new Vector3(sMag.DeadRange * 1.2f, 0.01f, sMag.DeadRange * 1.2f);
				}
				else if (cMag != null)	// キューブ
				{
					deadCircle.localScale = new Vector3(cMag.DeadRange * 4.0f, 0.01f, cMag.DeadRange * 4.0f);
				}
				else if (hcMag != null) // 半キューブ
				{
					magnetismCircle.localScale = new Vector3(hcMag.DeadRange, 0.01f, hcMag.DeadRange);
				}
			}

			deadCircle.SetPositionAndRotation(baseObj.transform.position, Quaternion.identity);
		}
	}
}
