using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedTunnel : MonoBehaviour
{
    public float scrollSpeed = 5.5f;
    private Renderer rend;
    private Vector2 offset;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        offset.x += Time.deltaTime * scrollSpeed;
        rend.material.SetTextureOffset("_MainTex", offset);
    }
}
