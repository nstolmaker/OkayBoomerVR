﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using System;

[Serializable]
public class SFX
{
    public enum Sounds
    {//%%%
        PickupMic, DropMic, Action, Cut, HoverGeneric, Correct,
        Dialog1, Dialog2, Dialog3, Slate,
        Mumble1, Mumble2, Mumble3
    };
}
public class SoundManager : MonoBehaviour
{

    /* there will be lots of sound effects and sound related stuff in this project. 
     * This seems like a good place to put some of that. My unity architecture isn't strong though
     * so if there's a preferred way, tell me! (Noah)
     * Also having a single game object that can manage audio state is useful since audio is tricky
     * */


    // internal variables
    [SerializeField][Tooltip("Used internally.")]
    private AudioSource audioSource;
    [SerializeField][Tooltip("Used internally.")]
    private AudioClip audioClip;
    [SerializeField][Tooltip("Used internally.")]
    public List<XRBaseInteractable> hoverTargets;

    // set these publicly
    [Tooltip("IMPORTANT! This is where we link up all the ConeZones for our characters. Increase the size to the number of 'talkers' you have, make sure they have SoundConeManager components on them, and then drag them into here to link them.")]
    public List<SoundConeManager> characters;
    [Tooltip("Set automagically. Audiosource we will attach to the director probably. Maybe should be in another script or just set in the editor.")]
    private AudioSource soundSource;
    [Tooltip("Used internally to determine the last time a sound effect was played. See soundEffectDebounce")]
    private float soundEffectDebounceLastTime;
    [Tooltip("Min time to wait before playing a sound effect again")]
    public float soundEffectDebounce = 0.75f;

    // %%% Audio Clip Definitions. Add to these.
    /* to add a new sound effect, you have to add it in SoundEffects.cs, and also like 4 places in this file. I've marked all the places with // %%% */
    public AudioClip PickupMic;
    public AudioClip DropMic;
    public AudioClip Action;
    public AudioClip Cut;
    public AudioClip HoverGeneric;
    public AudioClip Correct;
    public AudioClip Dialog1;
    public AudioClip Mumble1;
    public AudioClip Dialog2;
    public AudioClip Mumble2;
    public AudioClip Dialog3;
    public AudioClip Mumble3;
    public AudioClip Slate;

    public void Start()
    {
        // set the ids for characters that were defined
        for (int i = 0; i < characters.Count; i++)
        {
            //Debug.Log("Iterating over defined chars and settings ids: " + i);
            characters[i].charID = i;
            characters[i].Mumble(); // start mumbling here, otherwise if the object starts itself sometimes it happens before it has an index and errors.
        }
    }

    /* 
     * Use this function to play sounds. Right now it's not actually queueing them, but it's good to have a separation layer so we can do it later if we need
     */
    public void QueSound(SFX.Sounds clipName)
    {
        AudioClip resolvedAudioClip = ResolveSoundToClip(clipName);
        if (resolvedAudioClip)
        {
            PlaySoundEffect(resolvedAudioClip);
        }
    }

    public AudioClip ResolveSoundToClip(SFX.Sounds clipName)
    {
        AudioClip clip = null;
        // %%%
        switch (clipName)
        {
            case SFX.Sounds.PickupMic:
                clip = this.PickupMic;
                break;
            case SFX.Sounds.DropMic:
                clip = this.DropMic;
                break;
            case SFX.Sounds.Action:
                clip = this.Action;
                break;
            case SFX.Sounds.Cut:
                clip = this.Cut;
                break;
            case SFX.Sounds.HoverGeneric:
                clip = this.HoverGeneric;
                break;
            case SFX.Sounds.Correct:
                clip = this.Correct;
                break;
            case SFX.Sounds.Dialog1:
                clip = this.Dialog1;
                break;
            case SFX.Sounds.Dialog2:
                clip = this.Dialog2;
                break;
            case SFX.Sounds.Dialog3:
                clip = this.Dialog3;
                break;
            case SFX.Sounds.Mumble1:
                clip = this.Mumble1;
                break;
            case SFX.Sounds.Mumble2:
                clip = this.Mumble2;
                break;
            case SFX.Sounds.Mumble3:
                clip = this.Mumble3;
                break;
            case SFX.Sounds.Slate:
                clip = this.Slate;
                break;
        }
        if (clip)
        {
            return clip;
        }
        return null;
    }

    public void SetCharacterAudio(int charID, SFX.Sounds clipName)
    {
        AudioClip resolvedAudioClip = ResolveSoundToClip(clipName);
        // as long as it returns something
        if (resolvedAudioClip)
        {
            //Debug.Log("SetCharacterAudio| resolvedAudioClip successfully");
            // ensure the char exists
            if (characters[charID])
            {
                //Debug.Log("SetCharacterAudio| characters["+charID+"] exists");
                // the array is called characters, but it's actually an array of SoundConeManager's. So, we have to get the component that actually is making the sound, which is defined as it's talkyTalky gameObject.
                // The talkyTalky is usually an invisible sphere at the charactors mouth. Find that, and then get it's audioSource, which plays the sound.
                AudioSource charAudioSource = characters[charID].talkyTalky?.gameObject.GetComponent<AudioSource>();
                if (!charAudioSource) 
                {
                    // NO audio source yet exists. make one
                    characters[charID].talkyTalky.gameObject.AddComponent<AudioSource>();
                    charAudioSource.loop = true;
                    charAudioSource.playOnAwake = true;
                    charAudioSource.spatialBlend = 1;
                    charAudioSource.spread = 212;
                    charAudioSource.rolloffMode = AudioRolloffMode.Logarithmic; // cant set this to custom with scripting, so i guess we'll have to make sure we make these manually.
                    Debug.LogWarning("WARNING: Creating a new audio source with Logarithmic rollof on gameobject (" + characters[charID].talkyTalky.gameObject.name + ") because there isnt one. However, we cant set the volume rollOf mode in code, so you're better of making an audioSource on the talkyTalky yourself. ");
                }
                // set the audio Clip the defined clip.
                //Debug.Log("SetCharacterAudio| setting charAudioSource to resolvedAudioClip: "+resolvedAudioClip.name + "(clipName: "+ clipName.ToString()+ ")");
                if (charAudioSource.isPlaying)
                {
                    float trackPosition = charAudioSource.time;
                    //Debug.Log("SetCharacterAudio| already playing, so saving track position of " + trackPosition.ToString());
                    charAudioSource.clip = resolvedAudioClip;
                    charAudioSource.time = trackPosition;
                    charAudioSource.Play();
                } else
                {
                    // not playing, so just load it up and hit play.
                    charAudioSource.clip = resolvedAudioClip;
                    charAudioSource.Play();
                }

            } else
            {
                Debug.Log("Error in SoundManager.cs: Char with id " + charID + " not found.");
            }
        }
    }

    private void PlaySoundEffect(AudioClip audioClip)
    {
        if (audioSource = gameObject.GetComponent<AudioSource>())
        {
            audioSource.clip = audioClip;
            audioSource.PlayOneShot(audioClip);
        } else { 
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = false;
            audioSource.playOnAwake = false;

            audioSource.clip = audioClip;
            audioSource.PlayOneShot(audioClip);
        }
        
    }

 
    public void ResolveInteractionSounds(XRBaseInteractor controller)
    {

        // this stuff whole function doesn't get called until after the event handlers have already been done, 
        // so for hover, we have to check if it's happening right now. At that point we might as well bind the events

        // okay it seems like this only happens when we are in a hover state, and therefor target is null.

        controller.GetHoverTargets(hoverTargets);
        foreach (XRBaseInteractable target in hoverTargets)
        {
            //Debug.Log("hoverTarget: " + target.name);
            SoundEffects objectSFX = target?.GetComponent<SoundEffects>() ?? null;

            // %%%
            SFX.Sounds pickupSound = objectSFX.pickupSound;
            SFX.Sounds dropSound = objectSFX.dropSound;
            SFX.Sounds activateSound = objectSFX.activateSound;
            SFX.Sounds hoverSound = objectSFX.hoverSound;
            SFX.Sounds correctSound = objectSFX.correctSound;
            SFX.Sounds dialog1Sound = objectSFX.dialog1Sound;
            SFX.Sounds dialog2Sound = objectSFX.dialog2Sound;
            SFX.Sounds dialog3Sound = objectSFX.dialog3Sound;
            SFX.Sounds mumble1Sound = objectSFX.mumble1Sound;
            SFX.Sounds mumble2Sound = objectSFX.mumble2Sound;
            SFX.Sounds mumble3Sound = objectSFX.mumble3Sound;
            SFX.Sounds slateSound = objectSFX.slateSound;
            // we are hovering, so play the hover sound.
            if (objectSFX != null)
            {
                QueSound(objectSFX.hoverSound);
            }
         
            // now bind the rest of the sounds to this object that's currently hovered.
            controller.onHoverEnter.RemoveAllListeners();
            controller.onHoverExit.RemoveAllListeners();
            controller.onSelectEnter.RemoveAllListeners();
            controller.onSelectExit.RemoveAllListeners();

            // then, wire this up so when we drop it it knows what sound to play. Yay for closures.
            controller.onHoverExit.AddListener((XRBaseInteractable item) =>
            {
                if (item)
                {
                    //Debug.Log("onHoverExit: " + item.gameObject.name);
                    QueSound(hoverSound);
                    controller.onHoverEnter.RemoveAllListeners();
                    controller.onHoverExit.RemoveAllListeners();
                }
                else
                {
                    Debug.Log("onHoverExit of unknown item ");
                }
            });
            controller.onHoverEnter.AddListener((XRBaseInteractable item) =>
            {
                if (item)
                {
                    //Debug.Log("onHoverEnter: " + item.gameObject.name);
                    QueSound(hoverSound);
                }
                else
                {
                    Debug.Log("onHoverEnter of unknown item ");
                }
            });
            controller.onSelectEnter.AddListener((XRBaseInteractable item) =>
            {
                if (item)
                {
                    //Debug.Log("onSelectEnter item " + item.gameObject.name);
                    QueSound(pickupSound);
                } else
                {
                    Debug.Log("onSelectEnter unknown item ");
                }
            });
            controller.onSelectExit.AddListener((XRBaseInteractable item) =>
            {
                if (item)
                {
                    //Debug.Log("onSelectExit: " + item.gameObject.name);
                    QueSound(dropSound);
                    controller.onHoverEnter.RemoveAllListeners();
                    controller.onHoverExit.RemoveAllListeners();
                    controller.onSelectEnter.RemoveAllListeners();
                    controller.onSelectExit.RemoveAllListeners();
                }
                else
                {
                    Debug.Log("onSelectExit of unknown item ");
                }
            });
        }

    }


    // kind of a utility class. Should probably put this in the SoundManager when I do that migration.
    public bool CheckSoundEffectDebounce()
    {
        // if the current time is greater than the last time we played a sound + the defined minimum sound interval, then...
        if (Time.time > soundEffectDebounceLastTime + soundEffectDebounce)
        {
            soundEffectDebounceLastTime = Time.time;
            return true;
        }
        return false;
    }
}
