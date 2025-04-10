using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeMove : MonoBehaviour
{
    public float moveSlopeSpeed = 5.0f;
    public float maxSlopeAngle = 60.0f;

    bool colSlope = false;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        // キャラクターの移動方向を取得
        Vector3 moveDirection = rb.velocity.normalized;

        //if (colSlope)
        {
            MoveOnSlope(moveDirection);
        }
    }

    void MoveOnSlope(Vector3 moveDirection)
    {
        RaycastHit hit;

        // キャラクターの足元にRayを飛ばす
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 10f)||
            Physics.Raycast(transform.position - new Vector3(0,1,0), moveDirection, out hit, 1f))
        {
            Debug.DrawRay(transform.position, Vector3.down  * 10, Color.red);
            Debug.DrawRay(transform.position - new Vector3(0, 1, 0), moveDirection * 1, Color.blue);
            // 斜面の法線を取得
            Vector3 normal = hit.normal;

            //Vector3 pos = transform.position;
            //transform.position.Set(pos.x, pos.y+1, pos.z);

            // 斜面の角度を計算
            float slopeAngle = Vector3.Angle(Vector3.up, normal);

            if (slopeAngle <= 0.1f)
            {
                return;
            }
            rb.velocity = Vector3.zero;

            // 移動方向を斜面に沿わせる
            Vector3 slopeDirection = Vector3.ProjectOnPlane(moveDirection, normal);
            Debug.Log(slopeDirection);

            transform.position += (slopeDirection * moveSlopeSpeed * Time.deltaTime);

            //rb.MovePosition(transform.position + slopeDirection * moveSlopeSpeed /** Time.deltaTime*/);

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Slope"))
            colSlope = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Slope"))
            colSlope = false;
    }
}

