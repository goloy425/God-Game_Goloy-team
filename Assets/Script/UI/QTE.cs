using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QTE : MonoBehaviour
{
    public TMP_Text[] QTETexts;

    private List<int> Actions = new List<int>();

    enum Buttons
    {
        A,
        B,
        X,
        Y
    };

    int num = 0;

    private GameInputs inputs;

    bool now = false;
    bool old = false;

    // Start is called before the first frame update
    void Start()
    {
        inputs = new GameInputs();
        inputs.Enable();
;
        SetQTE();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(now!=old)
        {
            return;
        }

        switch(QTETexts[num].text)
        {
            case "A":
                if(inputs.QTE.A.IsPressed())
                {
                    PushedQTE();

                    ++num;
                }
                break;

            case "B":
                if(inputs.QTE.B.IsPressed())
                {
                    PushedQTE();

                    ++num;
                }
                break;

            case "X":
                if(inputs.QTE.X.IsPressed())
                {
                    PushedQTE();

                    ++num;
                }
                break;

            case "Y":
                if(inputs.QTE.Y.IsPressed())
                {
                    PushedQTE();

                    ++num;
                }
                break;

        }
    }

    void SetQTE()
    {
        int maxNum = 4;

        for (int i = 0; i < maxNum; ++i)
        {
            int randNum = Random.Range(0, 3);
                
            switch(randNum)
            {
                case 0:
                    QTETexts[i].text = "A";
                    break;

                case 1:
                    QTETexts[i].text = "B";
                    break;

                case 2:
                    QTETexts[i].text = "X";
                    break;

                case 3:
                    QTETexts[i].text = "Y";
                    break;

                default:
                    break;
            }
        }
    }

    void PushedQTE()
    {
        Color color = new Color();
        color.a = 0.5f;
        QTETexts[num].color = color;
    }
}
