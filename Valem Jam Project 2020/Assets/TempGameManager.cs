using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempGameManager : MonoBehaviour
{
    [Tooltip("If you don't set this yourself, i will look for a gameobject called GoalWatcher with a GoalWatcher on it, and use that. That's usually what you want.")]
    public GoalWatcher goalWatcher;
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

        internalClock = Time.time;
        goalWatcher.StartWatching();
    }

    
    void FixedUpdate()
    {
        if ((Time.time - internalClock) == 20)
        {
            Debug.Log("20 seconds are up.");
            SceneEnd();
        }
    }

    public void SceneEnd()
    {
        goalWatcher.StopWatching();
        Debug.Log("Your score was: " + goalWatcher.GetGoalsPercent());
    }

}
