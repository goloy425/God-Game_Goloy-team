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
	public GameObject Circles;  // 円のグループ

	[Header("範囲表示の円")]
	public Transform magnetismCircle;   // 磁力範囲の方
	public Transform deadCircle;        // くっつく範囲の方

	[Header("表示基準：plate（もしくはオブジェクト自身）")]
	public GameObject baseObj;
	// ↑deadRangeは基本的にオブジェクトの中心から生えてる方が自然
	// でもプレイヤーの磁石の場合はplateに追従する方が自然

	[Header("床を設定")]
	public GameObject floor;

	// 各オブジェクトの磁力スクリプト
	private Magnetism mag;
	private SphereMagnetism sMag;
	private CubeMagnetism cMag;

	private HCubeMagnetism hcMag;
	private bool isHCube = false;	// 半キューブかどうか（割れる前は円非表示）

	// その他スクリプト
	private AdjustMagnetism adjMag;		// 磁力調整
	private SplitCube sCube;		// キューブ分割
	private SwitchCircle circle;        // 円の表示・非表示切替 

	// 座標調整用の変数ズ
	Ray ray;
	RaycastHit hit;

	private float rayDistance = 10f;    // レイを飛ばす距離

	[Header("レイヤー：groundを設定")]
	public LayerMask ground;		// レイを当てる対象のレイヤー


	// Start is called before the first frame update
	void Start()
	{
		adjMag = GameObject.Find("Main Camera").GetComponent<AdjustMagnetism>();
		circle= GameObject.Find("Main Camera").GetComponent<SwitchCircle>();

		//if (GameObject.Find("Connecter") != null)
		//{
		//	GameObject.Find("Connecter").TryGetComponent<SplitCube>(out sCube);
		//}

		// 各分裂するオブジェクトごとのSplitCubeスクリプトを取得
		// 分裂するオブジェクトが複数あると分裂後に範囲が表示されないなどのバグがあったので追加しました
		// コネクターの時はそのまま取得
		if (gameObject.name == "Connecter")
		{
			gameObject.TryGetComponent<SplitCube>(out sCube);
		}
		// 半キューブの時はコネクターから取得
		if (gameObject.name == "MagObj_split1" || gameObject.name == "MagObj_split2")
		{
			if (transform.parent != null)
			{
				transform.parent.Find("Connecter").TryGetComponent<SplitCube>(out sCube);
			}
		}

		// Magnetismがアタッチされている（＝プレイヤーの磁石である）場合
		if (TryGetComponent<Magnetism>(out mag)){ return; }

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
		UpdateCircles();    // 位置・角度の更新
	}

	void UpdateCircles()
	{ 
		//--- 表示条件の分岐 ---//
		if (adjMag.Adjusted)
		{
			Circles.SetActive(false);	// 円を非表示にする
			return;		// 磁力調整終わるまでスルー
		}
		else
		{
			if (!isHCube)	// 半キューブでなければ非表示解除
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

		// SwitchCircleによる表示・非表示切替
		if (circle.isInactive)
		{
			Circles.SetActive(false);
		}
		else
		{
			if (!isHCube)	// 半キューブでなければ非表示解除
			{
				Circles.SetActive(true);
			}
		}

		//--- 円の更新 ---//	磁力範囲更新の中のreturnを正常に通すために別関数にした
		// 磁力範囲
		if (magnetismCircle != null)
		{
			UpdateMagCircle();
		}

		// くっつく範囲
		if (deadCircle != null)
		{
			UpdateDeadCircle();
		}
	}

	//=================================================
	// 磁力範囲の更新
	//=================================================
	public void UpdateMagCircle()
	{
		if (mag != null)	// magnetism付き（=プレイヤーの磁石）の場合
		{
			// サイズ更新（*1.2の理由：ないのがリアル範囲だけど視覚的にはこんな感じ）
			magnetismCircle.localScale = new Vector3(mag.magnetismRange * 1.2f, 0.01f, mag.magnetismRange * 1.2f);

			// レイが当たった点よりちょっと上をpositionとする
			if (Physics.Raycast(ray, out hit, rayDistance, ground))
			{
				float posY = hit.point.y + 0.06f;

				// 位置の追従＆回転を固定
				magnetismCircle.SetPositionAndRotation(new Vector3(hit.point.x, posY, hit.point.z), Quaternion.identity);
			}
			return;
		}
		else	// magnetismが付いてない（=磁力オブジェクト）の場合
		{
			ray = new Ray(this.transform.position, Vector3.down);   // オブジェクトの直下にレイを投げる

			if (sMag != null)	// 球
			{
				magnetismCircle.localScale = new Vector3(sMag.MagnetismRange * 1.2f, 0.01f, sMag.MagnetismRange * 1.2f);
			}
			else if (!sCube.splited && cMag != null)	// キューブ
			{
				magnetismCircle.localScale = new Vector3(cMag.MagnetismRange * 2, 0.01f, cMag.MagnetismRange * 2);
			}
			else if (sCube.splited && hcMag != null)	// 半キューブ
			{
				magnetismCircle.localScale = new Vector3(hcMag.MagnetismRange * 2, 0.01f, hcMag.MagnetismRange * 2);
			}
		}

		if (Physics.Raycast(ray, out hit, rayDistance, ground))
		{
            float posY = hit.point.y + 0.02f;

            // 位置の追従＆回転を固定
            magnetismCircle.SetPositionAndRotation(new Vector3(hit.point.x, posY, hit.point.z), Quaternion.identity);
        }
	}


	//=================================================
	// くっつく範囲の更新
	//=================================================
	private void UpdateDeadCircle()
	{
		ray = new Ray(baseObj.transform.position, Vector3.down);

		if (mag != null)
		{
			// サイズ更新
			deadCircle.localScale = new Vector3(mag.deadRange, 0.01f, mag.deadRange);

			// レイが当たった点をpositionとする
			if (Physics.Raycast(ray, out hit, rayDistance, ground))
			{
				float posY = hit.point.y + 0.1f;

				// 位置の追従＆回転を固定
				deadCircle.SetPositionAndRotation(new Vector3(hit.point.x, posY, hit.point.z), Quaternion.identity);
			}
			return;
		}
		else
		{
			if (sMag != null)	// 球
			{
				deadCircle.localScale = new Vector3(sMag.DeadRange * 3.0f, 0.01f, sMag.DeadRange * 3.0f);
			}
			else if (!sCube.splited && cMag != null)	// キューブ
			{
				deadCircle.localScale = new Vector3(cMag.DeadRange * 4.0f, 0.01f, cMag.DeadRange * 4.0f);
			}
			else if (sCube.splited && hcMag != null)	// 半キューブ
			{
				deadCircle.localScale = new Vector3(hcMag.DeadRange * 1.5f, 0.01f, hcMag.DeadRange * 1.5f);
			}
		}

		if (Physics.Raycast(ray, out hit, rayDistance, ground))
		{
			float posY = hit.point.y + 0.1f;

			// 位置の追従＆回転を固定
			deadCircle.SetPositionAndRotation(new Vector3(hit.point.x, posY, hit.point.z), Quaternion.identity);
		}
	}
}
