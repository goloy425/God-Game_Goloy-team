using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pose : MonoBehaviour
{
    CanvasGroup Canvas;

    bool _pose = false;

    // Start is called before the first frame update
    void Start()
    {
        Canvas = this.GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.JoystickButton9))
        {
            _pose = !_pose;

            DisplayPose(_pose);
        }

    }

    void DisplayPose(bool pose)
    {
        if (pose)
        {
            Canvas.alpha = 1.0f;
        }
        else
        {
            Canvas.alpha = 0.0f;
        }
    }
}
