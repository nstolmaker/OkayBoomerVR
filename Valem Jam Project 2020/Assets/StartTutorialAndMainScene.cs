using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StartTutorialAndMainScene : MonoBehaviour
{
    public GameObject buttonForStarting;
    public PlayableDirector tutorialTimeline;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void StartTimelineForTutorialDirector()
    {
        Debug.Log("Your mum");
        tutorialTimeline.Play();
    }
}
