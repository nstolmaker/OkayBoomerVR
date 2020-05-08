using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayStartGameSound : MonoBehaviour
{
    private AudioSource startGameSource;
    public AudioClip startGameSound;

    // Start is called before the first frame update
    void Start()
    {
        startGameSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGameSound()
    {
        startGameSource.PlayOneShot(startGameSound);
    }
}
