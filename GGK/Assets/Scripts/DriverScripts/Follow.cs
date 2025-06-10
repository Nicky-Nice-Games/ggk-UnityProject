using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Follow : MonoBehaviour
{
    public Camera camera;
    public float speed;
    Vector3 lastPosition;
    Vector3 spawnPosition;

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;

        spawnPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //store in a value how much position changed, then check if its large enough, if it is then move the camera farther back according to how fast you are going

        speed = Vector3.Distance(transform.position, lastPosition) / Time.deltaTime;

        if (speed > 0.5f)
        {
            SetCamera(speed);
        }

        lastPosition = transform.position;
    }

    public void SetCamera(float speed)
    {
        camera.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z - speed * 0.1f);
        camera.fieldOfView = Mathf.Clamp(camera.fieldOfView + speed, 60f, 90f);
    }
}
