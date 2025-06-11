using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadRandomCharacter : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public List<Sprite> sprites;
    public List<Color> colors;

    // Start is called before the first frame update
    void Start()
    {
        // Load random character
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Count)];
        spriteRenderer.color = colors[Random.Range(0, colors.Count)];
    }
}
