using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

            txtScore.text = Scoring.ToString();
            
            float remainder = scoring % scoringStep;
            print(remainder);
            if (remainder == 0)
            {
                Upgrade();
            }
        }
    }

    private PlayerController player;
    
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
    private GameObject lastFeedback;
    
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
    [SerializeField] private Button bttnRestart;
    [SerializeField] private TextMeshProUGUI txtScore;
    
    private void Awake()
    {
        //tempo init
        beatDuration = 60f / bpm;
        Sanity = sanityMax / 2;
        Scoring = 1;
        
        //references
        beatControllers = new List<BeatController>(FindObjectsOfType<BeatController>());
        monsters = new List<MonsterController>();
        player = FindObjectOfType<PlayerController>();
        
        //audio
        foreach (var source in audioSources) source.volume = 0;
        audioSources[0].volume = 1;
        
        //launches game
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
        Destroy(lastFeedback);
        lastFeedback = feedback;
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
        bttnRestart.Select();
        gameOverScreen.SetActive(true);
    }

    #endregion

    #region Monster Management

    
    void MonsterSpawning()
    {
        if (Sanity >= sanityMax)
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
        int r = Random.Range(0, 4);
        return r <= 2; //spawn chance = 3/4
    }

    void OnSpawnMonster()
    {
        MonsterController newMonster = Instantiate(monsterPrefab, Random.insideUnitSphere, quaternion.identity)
            .GetComponent<MonsterController>();

        newMonster.Initialize(this);
        newMonster.direction = player.transform.position - newMonster.transform.position;
        monsters.Add(newMonster);
        beatControllers.Add(newMonster);

        //clears itself from a list on death
        newMonster.deathAction = null;
        newMonster.deathAction += (beat, monster) =>
        {
            monsters.Remove(monster);
            beatControllers.Remove(beat);
        };
        //reduces sanity on damage
        newMonster.dmgAction = null;
        newMonster.dmgAction += () =>
        {
            Sanity--;
        };
    }

    #endregion
}
