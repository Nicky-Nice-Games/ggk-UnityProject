using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCharacter : MonoBehaviour
{
    // References
    private CharacterData characterData;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Get GameManger and Camera
        characterData = FindAnyObjectByType<CharacterData>();

        // Load saved selected character
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = characterData.characterSprite;
        spriteRenderer.color = characterData.characterColor;
    }
}
