using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCameraController : MonoBehaviour
{
    public Transform target1; // 対象物１
    public Transform target2; // 対象物２
    public float minSize = 5f; // 最小サイズ
    public float maxSize = 15f; // 最大サイズを15に制限
    public float zoomSpeed = 5f; // 拡大速度
    public float padding = 2f; // 対象物間の距離に余裕を持たせるための調節値
    public Vector3 initialOffset = new Vector3(0, 5, -10); // カメラの初期位置を指定し、ズームによって変化させる基本的なオフセット値

    private Camera cam;
    private Vector3 dynamicOffset;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        cam.orthographic = true;
        dynamicOffset = initialOffset;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target1 == null || target2 == null) return;

        Vector3 midPoint = (target1.position + target2.position) / 2f;
        float distance = Vector3.Distance(target1.position, target2.position) + padding;

        float requiredSize = CalculateRequiredSize(distance);
        requiredSize = Mathf.Clamp(requiredSize, minSize, maxSize); // minSize以上を保証

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, requiredSize, Time.deltaTime * zoomSpeed);

        float zoomFactor = Mathf.Clamp01((cam.orthographicSize - minSize) / (maxSize - minSize));
        dynamicOffset = new Vector3(initialOffset.x, initialOffset.y, Mathf.Lerp(initialOffset.z, initialOffset.z * 2f, zoomFactor));

        transform.position = midPoint + dynamicOffset;
    }

    float CalculateRequiredSize(float distance)
    {
        float aspectRatio = (float)Screen.width / Screen.height;
        float requiredSizeByHeight = (distance / 2f);
        float requiredSizeByWidth = (distance / (2f * aspectRatio));

        return Mathf.Max(requiredSizeByHeight, requiredSizeByWidth);
    }
}
