using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerLevel1 : MonoBehaviour
{
    [Tooltip("If you don't set this yourself, i will look for a gameobject called GoalWatcher with a GoalWatcher on it, and use that. That's usually what you want.")]
    public GoalWatcher goalWatcher;
    [Tooltip("The sound manager. Will look for a game object called 'SoundManager' if this is left empty")]
    public SoundManager soundManager;
    [Tooltip("The CharacterAudioSetup. Will look for a game object called 'CharacterAudioSetup' if this is left empty")]
    public CharacterAudioSetup characterAudioSetup;
    [SerializeField]
    public int startWatchingGoalsAfterNSeconds = 30;
    public int stopWatchingGoalsAfterNSeconds = 48;
    public int startErnieTrackAt = 15;
    public int startSrgtTrackAt = 15;
    public int cutAt = 50;
    private float internalClock;
    public float gameTimestamp;
    public int nextSceneID = 2;
    public int goToNextSceneAt = 60;

    void Start()
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

        internalClock = Time.time;
        characterAudioSetup.StartAllCharacterAudioTracks();
    }

    
    void FixedUpdate()
    {
        gameTimestamp = (float)System.Math.Floor(Time.time - internalClock);

            if ((Time.time - internalClock) == startWatchingGoalsAfterNSeconds)
        {
            Debug.Log(startWatchingGoalsAfterNSeconds + " seconds are up.");
            goalWatcher.StartWatching();

        }
        else if (gameTimestamp == stopWatchingGoalsAfterNSeconds)
        {
            Debug.Log(stopWatchingGoalsAfterNSeconds + " seconds are up.");
            SceneEnd();
        }
        else if (gameTimestamp == startErnieTrackAt)
        {
            //startErnieTrackAt
        }
        else if (gameTimestamp == startSrgtTrackAt)
        {
            //startSrgtTrackAt
        }
        else if (gameTimestamp == cutAt)
        {
            // this id is based on the sequence which the ConeZone's are specified in the SoundManager. So if you change the order of them, you might have to re-set this ID number. Not the best solution, but it's fine for now.
            if (goalWatcher.GetGoalsPercent() > 0.50)
            {
                // they did alright, let them know and move them on to the next scene
                nextSceneID = 2;
                soundManager.SetCharacterAudio(2, SFX.Sounds.DirectorSuccess1);
            } else
            {
                // they did terribly, yell at them and reload the scene
                soundManager.SetCharacterAudio(2, SFX.Sounds.DirectorFail1);
                nextSceneID = SceneManager.GetActiveScene().buildIndex;
            }
            
        }
        else if ((Time.time - internalClock) == goToNextSceneAt)
        {
            //goToNextSceneAt
            SceneManager.LoadScene(nextSceneID);
        }
    }

    public void SceneEnd()
    {
        goalWatcher.StopWatching();
        Debug.Log("Your score was: " + goalWatcher.GetGoalsPercent());
    }
}
