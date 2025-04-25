using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.InputSystem;

//=================================================
// 作成者：宮本和音
// 磁力強化（L）　ZLの方
//=================================================

public class AugMagL : MonoBehaviour
{
	[Header("PlayerLの磁石を設定")]
	public GameObject magnet;

	[Header("フラグ：強化中")]
	public bool isAugmenting;   // 強化中かどうかのフラグ

	[Header("磁力強化SE")]
	public AudioClip audioClip;

	private AudioSource audioSource;
	private float timer = 0.0f; // 再生秒数
	private int playCnt = 0;	// 再生回数

	private GameInputs inputs;	// GameInputsクラス
	private Magnetism mag;	// magnetのMagnetismを取得する用

	// 色チェンジ用変数
	private Renderer circleRenderer;
	private Color defaultColor;
	private Color poweredColor = Color.green;   // 強化中の色

	// Start is called before the first frame update
	void Start()
	{
		inputs = new GameInputs();
		inputs.Enable();

		magnet.TryGetComponent<Magnetism>(out mag);

		// 磁力範囲オブジェクトを取得
		Transform circle = GameObject.Find("PlayerL").transform.Find("Circles/MagnetismCircle");

		if (circle != null)
		{
			circle.TryGetComponent<Renderer>(out circleRenderer);

			if (circleRenderer != null)
			{
				// オブジェクト個別のマテリアルインスタンスを使うように明示
				circleRenderer.material = new Material(circleRenderer.material);
				defaultColor = circleRenderer.material.color;
			}
		}

		audioSource = GetComponent<AudioSource>();	// AudioSource取得
	}

	// Update is called once per frame
	void Update()
	{
		// ボタンを押されてる強さの取得
		float LValue = inputs.PlayerL.AugmentMag.ReadValue<float>();

        // オブジェクトの磁力範囲内にいる時、一定以上の強さでキーが押されたら磁力強化
        if (LValue > 0.3f && mag.inObjMagArea)
        {
            AugmentPlayerLMagnetism();
			PlaySE();
		}
		else
		{
			ResetPlayerLMagnet();   // 色やら何やらを元に戻す
		}
	}

	private void AugmentPlayerLMagnetism()
	{
		Color temp = poweredColor;
		temp.a = defaultColor.a + 0.1f;	// 不透明度を若干上げる
		circleRenderer.material.color = temp;

		isAugmenting = true;
	}

	private void ResetPlayerLMagnet()
	{
		circleRenderer.material.color = defaultColor;	// 色を元に戻す
		isAugmenting = false;

        timer = 0.0f;	// タイマーをリセット
		playCnt = 0;	// 再生回数をリセット
    }

	private void OnDestroy()
	{
		inputs?.Dispose();
	}

	// SEを再生終了後にループ再生
	private void PlaySE()
	{
		timer += Time.deltaTime;	// 再生からの経過時間

		// 最初に一回再生
		if(playCnt == 0)
		{
            audioSource.PlayOneShot(audioClip);
			playCnt++;
        }

		// 再生終了後にループ再生
		if(timer >= audioClip.length)
		{
			audioSource.PlayOneShot(audioClip);
			timer = 0.0f;   // タイマーをリセット
        }
	}
}
