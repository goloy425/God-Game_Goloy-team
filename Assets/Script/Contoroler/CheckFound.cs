using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// スポットライトとの接触を管理

public class CheckFound : MonoBehaviour
{
    bool found = false;

    QTE qte;

    // Start is called before the first frame update
    void Start()
    {
        qte = GameObject.Find("QTE").GetComponent<QTE>();
    }

    // Update is called once per frame
    void Update()
    {
        if(qte.GetClear())
        {
            found = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag.ToString()=="SpotLight")
        {
            found = true;
        }
    }

    public bool GetHit()
    {
        return found;
    }
}
