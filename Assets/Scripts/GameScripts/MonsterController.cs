using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : BeatController
{
    private SpriteRenderer spriteRenderer;
    private Color initColor;
    [SerializeField] private float returnSpeed;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        initColor = spriteRenderer.color;
    }

    private void Update()
    {
        spriteRenderer.color = Color.Lerp(spriteRenderer.color, initColor, Time.deltaTime * returnSpeed);
    }

    public override void Beat()
    {
        var color = spriteRenderer.color;
        var col = new Color(color.r, color.g, color.b, 0);
        spriteRenderer.color = col;
    }
}
