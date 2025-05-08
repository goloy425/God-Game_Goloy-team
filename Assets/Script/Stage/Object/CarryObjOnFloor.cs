using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//============================================================
// 作成者：宮本和音
// 移動床でプレイヤー・その他オブジェクトを運ぶスクリプト
//============================================================

public class CarryObjOnFloor : MonoBehaviour
{
	// 床には上に乗ってることを判別するためのコライダー(isTriggerをオン)とプレイヤーのめりこみを防ぐ用のコライダー
	// 計2つのコライダーをアタッチしておくこと

	void OnTriggerEnter(Collider other)
	{
		// 乗ってきたオブジェクトのタグが指定のものなら子オブジェクト化
		if (other.CompareTag("Player") || other.CompareTag("MagObj_Sphere") || other.CompareTag("MagObj_HCube"))
		{
			other.transform.SetParent(this.transform);
		}
	}

	void OnTriggerExit(Collider other)
	{
		// 上から出て行ったオブジェクトのタグが指定のものなら親子関係を解除
		if (other.CompareTag("Player") || other.CompareTag("MagObj_Sphere") || other.CompareTag("MagObj_HCube"))
		{
			other.transform.SetParent(null);
		}
	}
}
