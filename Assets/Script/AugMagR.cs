using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugMagR : MonoBehaviour
{
	[Header("PlayerRの磁石を設定")]
	public Magnetism magnet;

	private GameInputs inputs;		// GameInputsクラス

	// Start is called before the first frame update
	void Start()
	{
		inputs = new GameInputs();
		inputs.Enable();
	}

	// Update is called once per frame
	void Update()
	{
		// ボタンを押されてる強さの取得
		float RValue = inputs.PlayerR.AugmentMag.ReadValue<float>();

		// 押されてる強さで磁力強化に入るかどうか分岐
		if (RValue > 0.5f)
		{
			AugmentPlayerRMagnetism();
		}

	}

	private void AugmentPlayerRMagnetism()
	{
		Debug.Log("ZRキー押されてまーす");
	}

	private void OnDestroy()
	{
		inputs?.Dispose();
	}
}
