using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//===========================================================
// 作成者
// 山中迅人
// 
// 2025/3/21 
// 更新　山中迅人
//
//===========================================================

public class Rope : MonoBehaviour
{
    // 節の数を決める
    public GameObject[] vertices = new GameObject[15];
    LineRenderer line;

    // Start is called before the first frame update
    void Start()
    {
        // LineRendererを取得
        line = GetComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Unlit/Color"));
        line.positionCount = vertices.Length;

        foreach (GameObject v in vertices)
        {
            v.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        int idx = 0;
        foreach (GameObject v in vertices)
        {
            line.SetPosition(idx, v.transform.position);
            idx++;
        }
    }
}
