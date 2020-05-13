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
        gameTimestamp = (float)System.Math.Floor(Time.time - internalClock);
        if (gameTimestamp == startWatchingGoalsAfterNSeconds)
        {
            Debug.Log(startWatchingGoalsAfterNSeconds + " seconds are up.");
            goalWatcher.StartWatching();
            PlayableDirector timeline = GameObject.Find("TimelineDataMeeting").GetComponent<PlayableDirector>();
            timeline.Play();

        }
        else if (gameTimestamp == stopWatchingGoalsAfterNSeconds)
        {
            Debug.Log(stopWatchingGoalsAfterNSeconds + " seconds are up. *stopWatchingGoalsAfterNSeconds*");
            SceneEnd();
        } 
        else if (gameTimestamp == cutAt)
        {
            if (goalWatcher.GetGoalsPercent() > 0.50)
            {
                // this is the last level, so they won. We should have a score screen or credits or at least something.
                Debug.Log("They won!");
                nextSceneID = -1;
                // this id is based on the sequence which the ConeZone's are specified in the SoundManager. So if you change the order of them, you might have to re-set this ID number. Not the best solution, but it's fine for now.
                soundManager.SetCharacterAudio(0, SFX.Sounds.DirectorSuccess1);
            }
            else
            {
                // they did terribly, yell at them and reload the scene
                soundManager.SetCharacterAudio(0, SFX.Sounds.DirectorFail3);
                nextSceneID = SceneManager.GetActiveScene().buildIndex;
                Debug.Log("They did poorly. Setting nextSceneID to this scene: " + nextSceneID);
            }

        }
        else if (gameTimestamp == goToNextSceneAt)
        {
            //goToNextSceneAt
            if (nextSceneID > -1)
                SceneManager.LoadScene(nextSceneID);
        }
    }

    public void SceneEnd()
    {
        goalWatcher.StopWatching();
        Debug.Log("Your score was: " + goalWatcher.GetGoalsPercent());
    }
}
