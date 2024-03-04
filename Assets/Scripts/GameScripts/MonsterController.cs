using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : BeatController
{
    private BeatManager beatManager;
    private SpriteRenderer spriteRenderer;
    private Color initColor;
    [SerializeField] private float returnSpeed;
    public Action<BeatController,MonsterController> deathAction;
    public Action dmgAction;

    public Vector2 direction = Vector2.zero;
    public float speed = 1;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        initColor = spriteRenderer.color;
    }

    public void Initialize(BeatManager beat)
    {
        beatManager = beat;
    }
    
    private void Update()
    {
        spriteRenderer.color = Color.Lerp(spriteRenderer.color, initColor, Time.deltaTime * returnSpeed);
        
        if(Mathf.Abs(transform.position.x) > 9 || Mathf.Abs(transform.position.y) > 5)
            Death();
        
        transform.Translate(direction * (Time.deltaTime * speed));
    }

    public override void Beat()
    {
        var color = spriteRenderer.color;
        var col = new Color(color.r, color.g, color.b, 0);
        spriteRenderer.color = col;
    }

    void Death()
    {
        deathAction.Invoke(this, this);
        Destroy(gameObject);
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            dmgAction.Invoke();
            Death();
        }
    }
}
