using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseResume : MonoBehaviour
{
    public GameObject poseObj;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Resume()
    {
        poseObj.GetComponent<CanvasGroup>().alpha = 0.0f;
    }
}
