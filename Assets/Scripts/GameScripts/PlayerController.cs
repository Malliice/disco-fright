using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Camera cam;
    public Vector2 bounds = new Vector2(8, 4);
    public float speed = 1;

    private void Awake()
    {
        cam = Camera.main;
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector3 newDir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        transform.Translate(newDir * (Time.deltaTime * speed));

        if (transform.position.x > bounds.x)
            transform.position = new Vector2(bounds.x, transform.position.y);
        if (transform.position.x < -bounds.x)
            transform.position = new Vector2(-bounds.x, transform.position.y);
        if (transform.position.y > bounds.y)
            transform.position = new Vector2(transform.position.x, bounds.y);
        if (transform.position.y < -bounds.y)
            transform.position = new Vector2(transform.position.x, -bounds.y);
    }
}
