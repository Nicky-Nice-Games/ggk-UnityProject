using System.Collections;
using System.Collections.Generic;

//script not mine!!! found here: https://discussions.unity.com/t/third-person-camera-movement-script/783511
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    public float speed = 5f;
    public float jumpForce = 5f;
    public Transform cam;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        MovePlayer();
        RotatePlayer();
        JumpPlayer();
    }

    void MovePlayer()
    {
       // rb.velocity = Vector3.zero;
        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) vertical = 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) vertical = -1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) horizontal = -1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) horizontal = 1f;

        Vector3 moveDirection = (cam.forward * vertical + cam.right * horizontal).normalized;
        moveDirection.y = 0f; // Prevent movement in the vertical direction

        transform.position += moveDirection * speed * Time.deltaTime;
    }

    void RotatePlayer()
    {
        if (Input.GetAxis("Mouse X") != 0)
        {
            transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Time.deltaTime * 100f);
        }
    }

    void JumpPlayer()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}