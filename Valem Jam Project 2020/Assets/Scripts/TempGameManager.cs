using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempGameManager : MonoBehaviour
{
    [Tooltip("If you don't set this yourself, i will look for a gameobject called GoalWatcher with a GoalWatcher on it, and use that. That's usually what you want.")]
    public GoalWatcher goalWatcher;
    [Tooltip("The sound manager. Will look for a game object called 'SoundManager' if this is left empty")]
    public SoundManager soundManager;
    [Tooltip("The CharacterAudioSetup. Will look for a game object called 'CharacterAudioSetup' if this is left empty")]
    public CharacterAudioSetup characterAudioSetup;
    [Tooltip("The HandSummon. Will look for a game object called 'SummonManager' if this is left empty")]
    public HandSummon handSummon;
    [SerializeField]
    public int lengthOfTrack = 26;
    [SerializeField]
    public int showTutorialHandsAtTime = 6;
    public float internalClock;
    public float gameTimestamp;

    void Awake()
    {
        if (!goalWatcher)
        {
            goalWatcher = GameObject.Find("GoalWatcher").GetComponent<GoalWatcher>();
        }
        if (!goalWatcher)
        {
            Debug.LogError("Error in TempGameManager | Unable to find a goalWatcher. Scoring won't work.");
        }
        if (!soundManager)
        {
            soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        }
        if (!soundManager)
        {
            Debug.LogError("Error in TempGameManager | Unable to find the soundmanager. This breaks the sound timeline feature.");
        }
        if (!characterAudioSetup)
        {
            characterAudioSetup = GameObject.Find("CharacterAudioSetup").GetComponent<CharacterAudioSetup>();
        }
        if (!characterAudioSetup)
        {
            Debug.LogError("Error in TempGameManager | Unable to find the characterAudioSetup. This breaks the sound timeline feature.");
        }
        if (!handSummon)
        {
            handSummon = GameObject.Find("SummonManager").GetComponent<HandSummon>();
        }
        if (!handSummon)
        {
            Debug.LogError("Error in TempGameManager | Unable to find the SummonManager. This breaks the demo of hand summoning, and people might not know how to do it!");
        }

    }

    void Start()
    {
        internalClock = Time.time;
        characterAudioSetup.StartAllCharacterAudioTracks();
    }
    
    void FixedUpdate()
    {
        gameTimestamp = (float)System.Math.Floor(Time.time - internalClock);

        if (gameTimestamp == lengthOfTrack)
        {
            Debug.Log(lengthOfTrack+" seconds are up.");
            SceneManager.LoadScene(1);
        }
        else if (gameTimestamp == showTutorialHandsAtTime)
        {
            handSummon.ShowTutorial();
        }
        else if (gameTimestamp == showTutorialHandsAtTime + 3)
        {
            handSummon.HideTutorial();
        }
        
    }

    public void SceneEnd()
    {
        goalWatcher.StopWatching();
        Debug.Log("Your score was: " + goalWatcher.GetGoalsPercent());
    }
}
