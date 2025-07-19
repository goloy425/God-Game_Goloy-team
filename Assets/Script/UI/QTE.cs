using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

// QTEの表示とコマンド入力

public class QTE : MonoBehaviour
{
    CanvasGroup canvas;

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

    CheckFound cfL;
    CheckFound cfR;

    bool clear = false;
    bool reset = false;

    // Start is called before the first frame update
    void Start()
    {
        canvas = this.GetComponent<CanvasGroup>();
        canvas.alpha = 0.0f;

        cfR = GameObject.Find("PlayerR").GetComponent<CheckFound>();
        cfL = GameObject.Find("PlayerL").GetComponent<CheckFound>();

        inputs = new GameInputs();
        inputs.Enable();

    }

    // Update is called once per frame
    void Update()
    {
        if (!cfR.GetHit())
        {
            canvas.alpha = 0.0f;
            clear = false;
            return;
        }
        else
        {
            canvas.alpha = 1.0f;
        }
        if (num == 4)
        {
            num = 0;
            clear = true;
            reset = false;
        }

        if (!reset)
        {
            SetQTE();
            reset = true;
        }

        switch (QTETexts[num].text)
        {
            case "A":
                if (inputs.QTE.A.WasPerformedThisFrame())
                {
                    PushedQTE();

                    ++num;
                }
                break;

            case "B":
                if (inputs.QTE.B.WasPerformedThisFrame())
                {
                    PushedQTE();

                    ++num;
                }
                break;

            case "X":
                if (inputs.QTE.X.WasPerformedThisFrame())
                {
                    PushedQTE();

                    ++num;
                }
                break;

            case "Y":
                if (inputs.QTE.Y.WasPerformedThisFrame())
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

            switch (randNum)
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
            Color color = new Color(1.0f,1.0f,1.0f,1.0f);
            QTETexts[i].color = color;

        }
    }

    void PushedQTE()
    {
        Color color = new Color();
        color.a = 0.5f;
        QTETexts[num].color = color;
    }

    public bool GetStop()
    {
        if (cfR.GetHit() || cfL.GetHit())
        {
            return true;
        }
        return false;
    }

    public bool GetClear()
    {
        return clear;
    }
}
