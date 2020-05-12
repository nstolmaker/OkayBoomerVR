using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class GameManagerLevel3 : MonoBehaviour
{
    [Tooltip("If you don't set this yourself, i will look for a gameobject called GoalWatcher with a GoalWatcher on it, and use that. That's usually what you want.")]
    public GoalWatcher goalWatcher;
    [Tooltip("The sound manager. Will look for a game object called 'SoundManager' if this is left empty")]
    public SoundManager soundManager;
    [Tooltip("The CharacterAudioSetup. Will look for a game object called 'CharacterAudioSetup' if this is left empty")]
    public CharacterAudioSetup characterAudioSetup;
    [SerializeField]
    public int startWatchingGoalsAfterNSeconds = 14;
    [SerializeField]
    public int stopWatchingGoalsAfterNSeconds = 75;
    [SerializeField]
    public int cutAt = 76;
    private float internalClock;
    public float gameTimestamp;
    [SerializeField]
    public int nextSceneID = 3;
    [SerializeField]
    public int goToNextSceneAt = 80;

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
        gameTimestamp = Time.time - internalClock;
        if ((Time.time - internalClock) == startWatchingGoalsAfterNSeconds)
        {
            Debug.Log(startWatchingGoalsAfterNSeconds + " seconds are up.");
            goalWatcher.StartWatching();
            PlayableDirector timeline = GameObject.Find("TimelineDataMeeting").GetComponent<PlayableDirector>();
            timeline.Play();

        }
        else if ((Time.time - internalClock) == stopWatchingGoalsAfterNSeconds)
        {
            Debug.Log(stopWatchingGoalsAfterNSeconds + " seconds are up.");
            SceneEnd();
        } 
        else if ((Time.time - internalClock) == cutAt)
        {
            // this id is based on the sequence which the ConeZone's are specified in the SoundManager. So if you change the order of them, you might have to re-set this ID number. Not the best solution, but it's fine for now.
            if (goalWatcher.GetGoalsPercent() > 0.50)
            {
                // they did alright, let them know and move them on to the next scene
                nextSceneID = 0;
                Debug.Log("You won!");
                soundManager.SetCharacterAudio(0, SFX.Sounds.DirectorSuccess1);
            } else
            {
                // they did terribly, yell at them and reload the scene
                soundManager.SetCharacterAudio(0, SFX.Sounds.DirectorFail3);
                internalClock = Time.time;
                nextSceneID = 0; // send them back to the beginning. If we want to reload the level, we have to reset the timeline and do some other stuff i think. // SceneManager.GetActiveScene().buildIndex;
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
