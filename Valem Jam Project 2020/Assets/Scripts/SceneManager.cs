using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SceneManager : MonoBehaviour
{
    [SerializeField]
    PlayableDirector playableDirector;
    [SerializeField]
    float timeAmount;
    [SerializeField]
    AudioSource source;
    [SerializeField]
    AudioClip clip;

    void Start()
    {
        //GET CLIP FOR START GAME AND START TIMER
        source.clip = clip;
        source.Play();
        StartCoroutine(soundCheckTimer());
    }

    public void Play()
    {
        playableDirector.Play();
    }

    IEnumerator soundCheckTimer()
    {

        yield return new WaitForSeconds(source.clip.length);
        float time = timeAmount;
        print("START TIMER");

        while(time > 0)
        {
            yield return new WaitForSeconds(1);
            time--; 
            print("time: " + time);
        }

        StopAllCoroutines();
        print("END TIMER");
        Play();
    }
}