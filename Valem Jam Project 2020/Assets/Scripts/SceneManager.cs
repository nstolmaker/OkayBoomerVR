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
        timeAmount = (float)playableDirector.duration;
        StartCoroutine(startRound());
    }

    public void Play()
    {
        playableDirector.Play();
    }

    IEnumerator startRound()
    {
        yield return new WaitForSeconds(2);
        source.clip = clip;
        source.Play();
        Play();

        yield return new WaitForSeconds(source.clip.length);
        float time = timeAmount;
        print("START TIMER");

        while(time >= timeAmount)
        {
            yield return new WaitForSeconds(1);
            time = Time.fixedDeltaTime; 
            print("time: " + time);
        }

        StopAllCoroutines();
        print("END TIMER");
        Play();
    }
}