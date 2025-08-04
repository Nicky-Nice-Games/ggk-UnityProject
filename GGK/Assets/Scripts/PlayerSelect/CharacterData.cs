using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerKart
{
    Freddie,
    Gizmo,
    Sally,
    Jamster,
    Jamie,
    Sammy,
}

public class CharacterData : MonoBehaviour
{
    public static CharacterData Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public Sprite characterSprite; // In the future replace with GameObject that holds character model or replace with an enum to fetch a reference to an array
    //public PlayerKart character;
    //public GameObject characterModelSelected;
    public string characterName;
    public Color characterColor;
}
