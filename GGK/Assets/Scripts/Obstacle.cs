using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float xRange = 5.0f;
    [SerializeField] private float yRange = 0.0f;
    [SerializeField] private float zRange = 0.0f;
    [SerializeField] private AudioSource AudioClip;
    
    [SerializeField] private bool paused = false;
    [SerializeField] private bool moving = true;
    [SerializeField] private bool slowDown = true;

    private Vector3 startPosition;
    private float timeElapsed;

    private GameObject player;

    void Start()
    {
        startPosition = transform.position;
        player = GameObject.FindWithTag("Player");

        string newTag = "slowDown";

        //if marked as slow then when the player collides the speed will temporarily change via the playerMove script
        //have to add "slowDown" in the tags within the Unity editor
        if (slowDown)
        {
            gameObject.tag = newTag;
        }
    }

    void Update()
    {
        if (paused) return;

        if (moving)
        {
            timeElapsed += Time.deltaTime * speed;
            float x = startPosition.x + Mathf.Sin(timeElapsed) * xRange;
            float y = startPosition.y + Mathf.Sin(timeElapsed) * yRange;
            float z = startPosition.z + Mathf.Sin(timeElapsed) * zRange;

            transform.position = new Vector3(x, y, z);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && AudioClip != null)
        {
            StartCoroutine(PlayAudioForSeconds(3f)); // Adjust time as needed
        }
    }

    private IEnumerator PlayAudioForSeconds(float duration)
    {
        AudioClip.Play();
        yield return new WaitForSeconds(3);
        AudioClip.Stop();
    }
}