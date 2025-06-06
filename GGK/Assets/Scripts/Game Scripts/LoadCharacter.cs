using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCharacter : MonoBehaviour
{
    // References
    public GameManager gameManager;
    public Camera playerCamera;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Get GameManger and Camera
        gameManager = GameObject.FindAnyObjectByType<GameManager>();
        playerCamera = GameObject.FindAnyObjectByType<Camera>();

        // Load selected character
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = gameManager.characterSprite;
        spriteRenderer.color = gameManager.characterColor;
    }

    void Update()
    {
        // Character Sprites faces the camera
        // transform.up = playerCamera.transform.position - transform.position;
        // transform.forward = -playerCamera.transform.up;
    }
}
