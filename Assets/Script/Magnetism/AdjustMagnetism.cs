using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//=======================================================================
// 作成者　宮本和音
// 磁石の初期位置が磁力範囲外の時、最初の一瞬範囲を広げて戻すスクリプト
// アタッチする場合は必ず「Main Camera」にすること
//=======================================================================

public class AdjustMagnetism : MonoBehaviour
{
	[Header("プレイヤーの磁石（順不同）")]
	public Magnetism magnet1;
	public Magnetism magnet2;

	private bool adjusted = false;
	public bool Adjusted=>adjusted;

	private float originalMag;
	private float adjustTime = 0.3f;	// この秒数待ってから磁力範囲を戻す

	// Start is called before the first frame update
	void Start()
	{
		string currentScene = SceneManager.GetActiveScene().name;   // 現在のシーン名を取得

        //--- 適用するシーンを増やす場合はここに追記していく ---//
        // （switch文だとうまくいかなかったのでif文にしてある）

        //--- メインステージ ---//

        if (currentScene == "Stage1")
        {
            originalMag = magnet1.magnetismRange;

            magnet1.SetMagnetismRange(15.0f, this);
            magnet2.SetMagnetismRange(15.0f, this);
            adjusted = true;
        }
        else if (currentScene == "Stage2")
		{
			originalMag = magnet1.magnetismRange;

			magnet1.SetMagnetismRange(15.0f, this);
			magnet2.SetMagnetismRange(15.0f, this);
			adjusted = true;
		}
		else if (currentScene == "Stage3")
		{
			originalMag = magnet1.magnetismRange;

			magnet1.SetMagnetismRange(15.0f, this);
			magnet2.SetMagnetismRange(15.0f, this);
			adjusted = true;
		}
		else if (currentScene == "Stage4")
		{
			originalMag = magnet1.magnetismRange;

			magnet1.SetMagnetismRange(20.0f, this);
			magnet2.SetMagnetismRange(20.0f, this);
			adjusted = true;
		}

		//--- テンプレ　コピペして使ってね ---//
		//else if (currentScene == "")
		//{
		//    originalMag = magnet1.magnetismRange;

		//    magnet1.SetMagnetismRange(15.0f, this);
		//    magnet2.SetMagnetismRange(15.0f, this);
		//    adjusted = true;
		//}


		//--- テスト用シーン ---//
		else if (currentScene == "SampleScene")
		{
			originalMag = magnet1.magnetismRange;   // 本来の磁力範囲を保存しておく

			magnet1.SetMagnetismRange(10.0f, this);
			magnet2.SetMagnetismRange(10.0f, this);
			adjusted = true;    // 調整完了
		}
		else if (currentScene == "TestScene")
		{
			originalMag = magnet1.magnetismRange;

			magnet1.SetMagnetismRange(20.0f, this);
			magnet2.SetMagnetismRange(20.0f, this);
			adjusted = true;
		}
	}

	private void Update()
	{ 
		if (adjusted)	// 1回だけ処理
		{
			StartCoroutine(RevertMagnetism());
		}
	}

	private IEnumerator RevertMagnetism()
	{
		yield return new WaitForSeconds(adjustTime);
		magnet1.SetMagnetismRange(originalMag, this);
		magnet2.SetMagnetismRange(originalMag, this);
		adjusted = false;
	}
}


// 自分用メモ
// 最初はコルーチンを使用していたが「adjustedどこも使ってへんで！」てエラーが消えなかったので↓
// Updateでadjustedちゃんと使ってあげる方式に変更