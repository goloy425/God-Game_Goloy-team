using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCameraController : MonoBehaviour
{
    public Transform target1;
    public Transform target2;
    public float minSize = 5f;
    public float maxSize = 15f; // ç≈ëÂÉTÉCÉYÇ15Ç…êßå¿
    public float zoomSpeed = 5f;
    public float padding = 2f;
    public Vector3 initialOffset = new Vector3(0, 5, -10);

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
        requiredSize = Mathf.Clamp(requiredSize, minSize, maxSize); // minSizeà»è„Çï€èÿ

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
