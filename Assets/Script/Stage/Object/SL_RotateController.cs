using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//===========================================================
// 作成者：宮本和音
// 一定の角度でぐるぐる回り続けるスクリプト
//===========================================================

public class SL_RotateController : MonoBehaviour
{
	[Header("回転の角度（調整に関してはコメントを見てほしい）")]
	// もし数値を大きくするなら長さが足りない場合がある
	// その場合Spotlight_rotateのScale.yを伸ばせば長くなる
	// 同時に細くもなるのでScale.x、Scale.zも適切に調整してやること（xとzは同値にすること）
	// そのあとSpotlight_rotateの子オブジェクトのSpot Light→Spot Angleも調整すること
	public float rotationRadius = 30.0f;

	[Header("動く速度（角度/秒）")]
	public float rotationSpeed = 1f;

	private Vector3 centerAngle = new Vector3(0, 0, 0);		// 回転軸の角度
	private float timer = 0f;

	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		// 経過時間を加算（ぐるぐる回るための基準）
		timer += Time.deltaTime * rotationSpeed;

		// xとzを使って円運動っぽい回転を作る
		float x = centerAngle.x + Mathf.Sin(timer) * rotationRadius;
		float z = centerAngle.z + Mathf.Cos(timer) * rotationRadius;

		// 回転を適用（yは固定）
		transform.localEulerAngles = new Vector3(x, centerAngle.y, z);
	}
}
