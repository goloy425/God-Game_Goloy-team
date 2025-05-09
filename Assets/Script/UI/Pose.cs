using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pose : MonoBehaviour
{
    CanvasGroup Canvas;         // CanvasGroupコンポーネントを取得
    public Canvas pose;
    public Image targetImage;   // 操作するImage

    public Button[] myButton;

    int num = 1;

    bool _pose = false;

    // Start is called before the first frame update
    void Start()
    {
        Canvas = this.GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        // OPTIONボタン（PSコントローラーまたはXBOXコントローラー）の入力検出
        if (Input.GetKeyDown(KeyCode.JoystickButton9) || Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            _pose = !_pose;

            DisplayPose(_pose);
            num = 1;
        }
        

        if(_pose)
        {
            // 上ボタンの入力検出
            if (Input.GetKeyDown(KeyCode.JoystickButton13) || Input.GetAxis("DPadVertical") > 0)
            {
                myButton[num].GetComponent<Image>().color = Color.white;

                --num;
                num = Mathf.Clamp(num, 0, 3);

                myButton[num].GetComponent<Image>().color = Color.yellow;
            }

            // 下ボタンの入力検出
            if (Input.GetKeyDown(KeyCode.JoystickButton14) || Input.GetAxis("DPadVertical") < 0)
            {
                myButton[num].GetComponent<Image>().color = Color.white;

                ++num;
                num = Mathf.Clamp(num, 0, 3);

                myButton[num].GetComponent<Image>().color = Color.yellow;
            }



            // 〇ボタン（XBOXのBボタン）の入力検出
            if (Input.GetKeyDown(KeyCode.JoystickButton1))
            {
                myButton[num].onClick.AddListener(OnButtonClicked);
            }
        }

        Debug.Log(num);
    }

    void DisplayPose(bool pose)
    {
        if (pose)
        {
            Canvas.alpha = 1.0f;

            Color newColor = targetImage.color;
            newColor.a = 0.5f;
            targetImage.color = newColor;
        }
        else
        {
            Canvas.alpha = 0.0f;

            Color newColor = targetImage.color;
            newColor.a = 0.0f;
            targetImage.color = newColor;
        }
    }

    void OnButtonClicked()
    {
        switch (num)
        {
            case 0:
                SceneManager.LoadScene("MainMenu");
                break;

            case 1:
                Canvas.alpha = 0.0f;
                break;

            case 2:

                break;

            case 3:
                SceneManager.LoadScene("Title");
                break;
        }
    }
}
