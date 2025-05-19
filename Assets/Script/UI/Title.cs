using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    public Image titleBack;
    public float backAlpha = 0.5f;
    public Button start;

    // Start is called before the first frame update
    void Start()
    {
        Color color = titleBack.color;
        color.a = backAlpha;
        titleBack.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.JoystickButton2))
        {
            start.onClick.Invoke();
        }
    }

    void OnButtonClick()
    {

    }
}
