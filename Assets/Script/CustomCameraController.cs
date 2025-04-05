using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCameraController : MonoBehaviour
{
    public Transform target1;    // 対象物１
    public Transform target2;    // 対象物２
    public float minSize = 5f;   // 最小サイズ
    public float maxSize = 15f;  // 最大サイズを15に制限
    public float zoomSpeed = 5f; // 拡大速度
    public float padding = 2f;   // 対象物間の距離に余裕を持たせるための調節値
    public Vector3 initialOffset = new Vector3(0, 5, -10); // カメラの初期位置を指定し、ズームによって変化させる基本的なオフセット値

    private Camera cam;            // メインカメラへの参照を保持
    private Vector3 dynamicOffset; // 動的に変化するカメラオフセットを保持

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;             // シーン内のメインカメラを取得
        cam.orthographic = true;       // カメラを正射影モードに設定
        dynamicOffset = initialOffset; // 初期オフセットをセット
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target1 == null || target2 == null) return; // 対象物が設定されていない場合、処理を終了する

        Vector3 midPoint = (target1.position + target2.position) / 2f; // 2つの対象物の中間点を計算
        float distance = Vector3.Distance(target1.position, target2.position) + padding; // 対象物間の距離を計算し、余裕分を追加

        float requiredSize = CalculateRequiredSize(distance); // 必要なカメラサイズを計算
        requiredSize = Mathf.Clamp(requiredSize, minSize, maxSize); // 必要サイズが最小・最大サイズの範囲内に収まるように制限

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, requiredSize, Time.deltaTime * zoomSpeed); // 現在のカメラサイズから必要なサイズにスムーズに補間

        float zoomFactor = Mathf.Clamp01((cam.orthographicSize - minSize) / (maxSize - minSize)); // 現在のズームレベルを0〜1の範囲で計算
        dynamicOffset = new Vector3(initialOffset.x, initialOffset.y, Mathf.Lerp(initialOffset.z, initialOffset.z * 2f, zoomFactor)); // ズームレベルに応じてカメラの奥行き方向のオフセットを動的に調整

        transform.position = midPoint + dynamicOffset; // カメラを中間点とオフセットを考慮した位置に移動
    }

    // 必要なカメラサイズを計算する補助関数
    float CalculateRequiredSize(float distance)
    {
        float aspectRatio = (float)Screen.width / Screen.height;     // 画面のアスペクト比を計算
        float requiredSizeByHeight = (distance / 2f);                // 高さ方向で必要なサイズを計算
        float requiredSizeByWidth = (distance / (2f * aspectRatio)); // 幅方向で必要なサイズを計算

        return Mathf.Max(requiredSizeByHeight, requiredSizeByWidth); // 高さ方向と幅方向のうち、より大きなサイズを返す

    }
}
