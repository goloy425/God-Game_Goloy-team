using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 制作者　ゴロイ

[RequireComponent(typeof(MeshFilter))]
public class EnemyView : MonoBehaviour
{
    public float viewRadius = 0.3f; // 視野の半径
    public float viewAngle = 60f; // 視野角（60°)
    public int meshResolution = 10; // メッシュの解像度（滑らかさ）

    private MeshFilter meshFilter;
    private Mesh viewMesh;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        viewMesh = new Mesh();
        meshFilter.mesh = viewMesh;
    }

    void LateUpdate()
    {
        DrawViewCone();
    }

    void DrawViewCone()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;

        Vector3[] vertices = new Vector3[stepCount + 2];
        int[] triangles = new int[stepCount * 3];

        vertices[0] = Vector3.zero; // 扇形の中心（敵の位置）

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = -viewAngle / 2 + stepAngleSize * i;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            vertices[i + 1] = dir * viewRadius;
        }

        for (int i = 0; i < stepCount; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

}
