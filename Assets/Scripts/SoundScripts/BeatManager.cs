using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class BeatManager : MonoBehaviour
{
    public bool gameActive;
    
    public int bpm;
    private float beatDuration;
    private float timer;
    private List<BeatController> beatControllers;

    [Header("VFXs")]
    [SerializeField] private List<GameObject> feedbacks;
    
    [Header("Sanity")]
    private float sanity;
    private float Sanity
    {
        get => sanity;
        set
        {
            sanity = value;
            //if sanity is higher than its max, it doesn't maxes out
            if (sanity > sanityMax) sanity = sanityMax;
            if (sanity <= 0) GameOver();
            UpdateUi();
        }
    }
    [SerializeField] private float sanityMax;

    [Header("UI")]
    [SerializeField] private Slider sliderSan;
    [SerializeField] private GameObject gameOverScreen;
    
    private void Awake()
    {
        beatDuration = 60f / bpm;
        Sanity = sanityMax / 2;
        beatControllers = new List<BeatController>(FindObjectsOfType<BeatController>());
        gameActive = true;
        gameOverScreen.SetActive(false);
    }

    private void Update()
    {
        if (!gameActive)
            return;
        
        beatDuration = 60f / bpm;
        timer -= Time.deltaTime;
        //quand on arrive au beat
        if (timer <= 0)
        {
            print("Beat !");
            timer = beatDuration;
            foreach(var beat in beatControllers) beat.Beat();
        }

        if (Input.anyKeyDown)
        {
            OnClick();
        }
        
    }

    private void OnClick()
    {
        if (timer <= beatDuration * 1 / 8)
        {
            Sanity += 2;
            Feedback(0);
        }
        else if (timer <= beatDuration * 3 / 8)
        {
            Sanity += 1;
            Feedback(1);
        }
        else if (timer <= beatDuration * 6 / 8)
        {
            Sanity -= 1;
            Feedback(2);
        }
        else
        {
            Sanity -= 2;
            Feedback(3);
        }
    }

    private void Feedback(int successDegree)
    {
        GameObject feedback = Instantiate(feedbacks[successDegree], Vector3.zero, quaternion.identity);
        Destroy(feedback, beatDuration);
    }

    private void UpdateUi()
    {
        sliderSan.value = Sanity / sanityMax;
    }

    private void GameOver()
    {
        gameActive = false;
        gameOverScreen.SetActive(true);
    }
}
