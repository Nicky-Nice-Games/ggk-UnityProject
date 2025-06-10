using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCharacter : MonoBehaviour
{
    // References
    public GameManager gameManager;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Get GameManger and Camera
        gameManager = GameObject.FindAnyObjectByType<GameManager>();

        // Load saved selected character
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = gameManager.characterSprite;
        spriteRenderer.color = gameManager.characterColor;
    }
}
