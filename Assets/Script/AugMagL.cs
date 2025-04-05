using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.InputSystem;

//=================================================
// 制作者　宮本和音
// ZL・ZRで磁力強化するスクリプト
//=================================================

public class AugMagL : MonoBehaviour
{
	[Header("PlayerLの磁石を設定")]
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
		float LValue = inputs.PlayerL.AugmentMag.ReadValue<float>();

		// 押されてる強さで磁力強化に入るかどうか分岐
		if (LValue > 0.5f)
		{
			AugmentPlayerLMagnetism();
		}
	}

	private void AugmentPlayerLMagnetism()
	{
		//Debug.Log("ZLキー押されてまーす");
	}

	private void OnDestroy()
	{
		inputs?.Dispose();
	}
}
