using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCameraController : MonoBehaviour
{
    public Transform target1; // �Ώە��P
    public Transform target2; // �Ώە��Q
    public float minSize = 5f; // �ŏ��T�C�Y
    public float maxSize = 15f; // �ő�T�C�Y��15�ɐ���
    public float zoomSpeed = 5f; // �g�呬�x
    public float padding = 2f; // �Ώە��Ԃ̋����ɗ]�T���������邽�߂̒��ߒl
    public Vector3 initialOffset = new Vector3(0, 5, -10); // �J�����̏����ʒu���w�肵�A�Y�[���ɂ���ĕω��������{�I�ȃI�t�Z�b�g�l

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
        requiredSize = Mathf.Clamp(requiredSize, minSize, maxSize); // minSize�ȏ��ۏ�

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
