using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSettings : MonoBehaviour
{
    public GameObject pose;
    public GameObject settings;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Settings()
    {
        pose.GetComponent<CanvasGroup>().alpha = 0.0f;
        settings.GetComponent<CanvasGroup>().alpha = 1.0f;
    }
}
