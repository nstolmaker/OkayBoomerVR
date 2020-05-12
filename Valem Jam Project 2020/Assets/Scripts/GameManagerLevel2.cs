using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerLevel2 : MonoBehaviour
{
    [Tooltip("If you don't set this yourself, i will look for a gameobject called GoalWatcher with a GoalWatcher on it, and use that. That's usually what you want.")]
    public GoalWatcher goalWatcher;
    [Tooltip("The sound manager. Will look for a game object called 'SoundManager' if this is left empty")]
    public SoundManager soundManager;
    [Tooltip("The CharacterAudioSetup. Will look for a game object called 'CharacterAudioSetup' if this is left empty")]
    public CharacterAudioSetup characterAudioSetup;
    [SerializeField]
    public int startWatchingGoalsAfterNSeconds = 15;
    public int stopWatchingGoalsAfterNSeconds = 128;
    public int startErnieTrackAt = 15;
    public int startHanzTrackAt = 15;
    public int startBigPapaTrackAt = 15;
    private float internalClock;

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
        if ((Time.time - internalClock) == startWatchingGoalsAfterNSeconds)
        {
            Debug.Log(startWatchingGoalsAfterNSeconds + " seconds are up.");
            //SceneManager.LoadScene(1);

            goalWatcher.StartWatching();

        } else if ((Time.time - internalClock) == stopWatchingGoalsAfterNSeconds)
        {
            Debug.Log(stopWatchingGoalsAfterNSeconds+" seconds are up.");
            SceneEnd();
        }
        else if ((Time.time - internalClock) == startErnieTrackAt)
        {
            //startErnieTrackAt
        } else if ((Time.time - internalClock) == startHanzTrackAt)
        {
            //startHanzTrackAt
        }
        else if ((Time.time - internalClock) == startBigPapaTrackAt)
        {
            //startBigPapaTrackAt
        }
    }

    public void SceneEnd()
    {
        goalWatcher.StopWatching();
        Debug.Log("Your score was: " + goalWatcher.GetGoalsPercent());
    }
}
