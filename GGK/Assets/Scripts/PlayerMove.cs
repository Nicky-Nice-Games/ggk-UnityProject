using System.Collections;
using System.Collections.Generic;

//script reffed here: https://discussions.unity.com/t/third-person-camera-movement-script/783511
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private Transform cam;
    [SerializeField] private Rigidbody rb;

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
        float horizontal = 0f;
        float vertical = 0f;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift); // Check if sprint key is held

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) vertical = 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) vertical = -1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) horizontal = -1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) horizontal = 1f;

        float currentSpeed = isSprinting ? speed * 2.0f : speed; // Sprinting doubles the speed

        Vector3 moveDirection = (cam.forward * vertical + cam.right * horizontal).normalized;
        moveDirection.y = 0f; // Prevent movement in the vertical direction

        transform.position += moveDirection * currentSpeed * Time.deltaTime;
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

    //If collision is one of the slow down obstacles (need to add the "slowDown" tag within the Unity editor)
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("slowDown"))
        {
            speed /= 2;
            Debug.Log("slowing down");
            StartCoroutine(SpeedBackUp(3f)); // Adjust time as needed

        }
    }
    private IEnumerator SpeedBackUp(float duration)
    {
        yield return new WaitForSeconds(duration);
        speed *= 2;
        Debug.Log("speeding back up");

    }


}