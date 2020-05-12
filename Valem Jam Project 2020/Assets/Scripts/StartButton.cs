using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class StartButton : MonoBehaviour
{
    public GameObject buttonForStarting;
    public PlayableDirector tutorialTimeline;

    
    void Start()
    {
        if (!tutorialTimeline)
        {
            tutorialTimeline = GameObject.Find("TutorialTimeline").GetComponent<PlayableDirector>();
        }
        if (!tutorialTimeline)
        {
            Debug.LogError("Error in StartButton.cs | Couldn't find timeline for tutorial, and none was specified. Start button won't work and you won't be able to play the game. Name your timeline gameobject TutorialTimeline, or specify it manually.");
        }
    }

    public void StartTimelineForTutorialDirector()
    {
        tutorialTimeline.Play();
    }


    //When the Primitive collides with the walls, it will reverse direction
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered the button. starting stuff");
        StartTimelineForTutorialDirector();
    }

}
