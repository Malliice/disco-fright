using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseController : BeatController
{
    private Vector3 returnSize;
    [SerializeField] private float returnSpeed;
    [SerializeField] private float pulseSize;

    private void Awake()
    {
        returnSize = transform.localScale;
    }

    
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, returnSize, Time.deltaTime * returnSpeed);
    }

    public override void Beat()
    {
        //pulse
        transform.localScale = returnSize * pulseSize;
    }
}
