using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCircleEnemy : MonoBehaviour
{
    public Transform centerPoint;
    public float speed = 20f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // ’†S“X‚ğ²‚É‚µ‚Ä‰~‰^“®
        transform.RotateAround(centerPoint.position, Vector3.up, speed * Time.deltaTime);
    }
}
