using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseUISwitch : MonoBehaviour
{

    [Header("êÿÇËë÷Ç¶å≥ÅEêÿÇËë÷Ç¶êÊÇÃUI")]
    public GameObject resumeNormal;
    public GameObject resumeGlow;
    public GameObject retryNormal;
    public GameObject retryGlow;
    public GameObject titleNormal;
    public GameObject titleGlow;



    //--- Resume ---//
    public void Switch_ResumeGlow()
    {
        resumeNormal.SetActive(false);
        resumeGlow.SetActive(true);
    }
    public void Switch_ResumeNormal()
    {
        resumeGlow.SetActive(false);
        resumeNormal.SetActive(true);
    }

    //--- Retry ---//
    public void Switch_RetryGlow()
    {
        retryNormal.SetActive(false);
        retryGlow.SetActive(true);
    }
    public void Switch_RetryNormal()
    {
        retryGlow.SetActive(false);
        retryNormal.SetActive(true);
    }

    //--- ReturnToTitle ---//
    public void Switch_TitleGlow()
    {
        titleNormal.SetActive(false);
        titleGlow.SetActive(true);
    }
    public void Switch_TitleNormal()
    {
        titleGlow.SetActive(false);
        titleNormal.SetActive(true);
    }
}
