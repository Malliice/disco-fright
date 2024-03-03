using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BeatManager : MonoBehaviour
{
    [Header("Game Managing")]
    public bool gameActive;
    public int scoring;
    public int scoringStep;
    public int Scoring
    {
        get => scoring;
        set
        {
            scoring = value;
            float remainder = scoring % scoringStep;
            print(remainder);
            if (remainder == 0)
            {
                Upgrade();
            }
        }
    }
    
    [Header("Music")]
    public int bpm;
    public List<AudioSource> audioSources;
    public int currentAudioSource;
    private float beatDuration;
    [SerializeField] private float timer;
    private List<BeatController> beatControllers;
    
    
    [Header("Monsters")] 
    [SerializeField] private float monsterTimeToSpawn;
    private float monsterTimer;
    private List<MonsterController> monsters;
    [SerializeField] private GameObject monsterPrefab;

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
        Scoring = 1;
        beatControllers = new List<BeatController>(FindObjectsOfType<BeatController>());
        monsters = new List<MonsterController>();
        
        foreach (var source in audioSources) source.volume = 0;
        audioSources[0].volume = 1;
        
        gameActive = true;
        gameOverScreen.SetActive(false);
    }

    private void Update()
    {
        if (!gameActive)
            return;
        
        beatDuration = 60f / bpm;
        timer -= Time.deltaTime;
        //quand on arrive au beat:
        if (timer <= 0)
        {
            timer = beatDuration;
            foreach(var beat in beatControllers) beat.Beat();
            Sanity--;
        }

        if (Input.anyKeyDown)
        {
            OnClick();
        }
        
        MonsterSpawning();
    }

    #region Controls
    
    private void OnClick()
    {
        if (timer <= beatDuration * 1 / 8)
        {
            Sanity += 2;
            Scoring += 5;
            Feedback(0);
        }
        else if (timer <= beatDuration * 4 / 8)
        {
            Sanity += 1;
            Scoring += 1;
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

    void Upgrade()
    {
        currentAudioSource++;
        if (currentAudioSource < audioSources.Count) audioSources[currentAudioSource].volume = 1;
    }

    #endregion

    #region GameFEEL
    
    private void Feedback(int successDegree)
    {
        GameObject feedback = Instantiate(feedbacks[successDegree], Vector3.zero, quaternion.identity);
        Destroy(feedback, beatDuration);
    }

    #endregion

    #region UI

    private void UpdateUi()
    {
        sliderSan.value = Sanity / sanityMax;
    }

    private void GameOver()
    {
        Cursor.visible = true;
        gameActive = false;
        gameOverScreen.SetActive(true);
    }

    #endregion
    
    void MonsterSpawning()
    {
        if (Sanity >= sanityMax / 2)
            return;

        monsterTimer += Time.deltaTime;
        if (monsterTimer >= monsterTimeToSpawn)
        {
            monsterTimer = 0;
            if(RandomSpawning())
                OnSpawnMonster();
        }
    }

    bool RandomSpawning()
    {
        int r = Random.Range(0, 5);
        return r <= 1; //on a une chance sur 2 qu'un monstre spawn
    }

    void OnSpawnMonster()
    {
        MonsterController newMonster = Instantiate(monsterPrefab, Random.insideUnitSphere, quaternion.identity)
            .GetComponent<MonsterController>();
        
        monsters.Add(newMonster);
        beatControllers.Add(newMonster);

        //clears itself from a list on death
        newMonster.deathAction = null;
        newMonster.deathAction += (beat, monster) =>
        {
            monsters.Remove(monster);
            beatControllers.Remove(beat);
        };
    }
}
