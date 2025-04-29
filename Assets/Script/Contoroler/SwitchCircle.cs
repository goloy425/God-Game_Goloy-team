using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=======================================================================
// 作成者：宮本和音
// ×キー（上下左右4つあるボタンの下のやつ）で各Circleを非表示にする
//=======================================================================

public class SwitchCircle : MonoBehaviour
{
	[Header("非表示フラグ（デバッグ用）")]
	public bool isInactive;		// これがtrueならシーン中のCircleを全部非アクティブにする

	private GameInputs inputs;  // GameInputsクラス

	private bool prevFg;
	private bool nowFg;

	// Start is called before the first frame update
	void Start()
	{
		inputs = new GameInputs();
		inputs.Enable();
	}

	// Update is called once per frame
	void Update()
	{
		bool key = inputs.OtherKey.InactiveCircle.IsPressed();  // キー入力取得
		nowFg = key;	// フラグ反映

		// キー入力でフラグの切り替え
		if (nowFg && !prevFg)
		{
			if (isInactive)
			{
				isInactive = false;
			}
			else
			{
				isInactive = true;
			}
		}

		prevFg = nowFg;		// フラグ更新
	}

	private void OnDestroy()
	{
		inputs?.Dispose();
	}
}
