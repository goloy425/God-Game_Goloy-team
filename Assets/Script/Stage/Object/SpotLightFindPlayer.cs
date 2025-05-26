using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UIElements;

//=================================================
// 作成者：宮本和音
// スポットライトがプレイヤーを見つけた時の処理
//=================================================

public class SpotLightFindPlayer : MonoBehaviour
{
	private Renderer coneRenderer;
	private Light spotLight;

	private Color defaultConeColor;
	private Color defaultLightColor;

	public bool playerDetected;	// プレイヤーを発見した時に立てるフラグ

	// Start is called before the first frame update
	void Start()
	{
		// 各コンポーネントを取得
		coneRenderer = GetComponent<Renderer>();
		spotLight = GameObject.Find("Spot Light").GetComponent<Light>();

		// 元々の色を保存
		defaultConeColor = coneRenderer.material.GetColor("_EmissionColor");
		defaultLightColor = spotLight.color;
	}

	// Update is called once per frame
	void Update()
	{
		if (playerDetected)
		{
			Color alertColor = new Color(1f, 0.2f, 0.2f);

			// ConeのHDR色
			coneRenderer.material.SetColor("_EmissionColor", alertColor * 0.8f);

			// SpotLightの色
			spotLight.color = alertColor;
		}
		else
		{
			// 元の色に戻す
			coneRenderer.material.SetColor("_EmissionColor", defaultConeColor);
			spotLight.color = defaultLightColor;
		}
	}


	private void OnTriggerStay(Collider col)
	{
		if (col.CompareTag("Player"))
		{
			playerDetected = true;
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if(col.CompareTag("Player"))
		{
			playerDetected = false;
		}
	}
}
