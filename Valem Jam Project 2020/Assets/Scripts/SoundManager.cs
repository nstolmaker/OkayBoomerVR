using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using System;

[Serializable]
public class SFX
{
    public enum Sounds
    {
        PickupMic, DropMic, Action, Cut, HoverGeneric, Correct
    };
}
public class SoundManager : MonoBehaviour
{

    /* there will be lots of sound effects and sound related stuff in this project. 
     * This seems like a good place to put some of that. My unity architecture isn't strong though
     * so if there's a preferred way, tell me! (Noah)
     * Also having a single game object that can manage audio state is useful since audio is tricky
     * */

    /* to add a new sound effect, you have to add it in SoundEffects.cs, and also like 4 places in this file. I've marked all the places with // %%% */


// internal variables
[SerializeField][Tooltip("Used internally.")]
private AudioSource audioSource;
[SerializeField][Tooltip("Used internally.")]
private AudioClip audioClip;

// set these publicly
// %%%
public AudioClip PickupMic;
public AudioClip DropMic;
public AudioClip Action;
public AudioClip Cut;
public AudioClip HoverGeneric;
public AudioClip Correct;



/* 
 * Use this function to play sounds. Right now it's not actually queueing them, but it's good to have a separation layer so we can do it later if we need
 */
    public void QueSound(SFX.Sounds clipName)
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
        }
        if (clip)
        {
            PlaySoundEffect(clip);
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

        XRBaseInteractable target = controller?.selectTarget ?? null;
        if (target != null) {
            SoundEffects objectSFX = target?.GetComponent<SoundEffects>() ?? null;
            // they just started, or are still holding something, so we can use the target
            //Debug.Log("Called ResolveInteractionSounds on : " + target.gameObject.name);

            // %%%
            SFX.Sounds pickupSound = objectSFX.pickupSound;
            SFX.Sounds dropSound = objectSFX.dropSound;
            SFX.Sounds activateSound = objectSFX.activateSound;
            SFX.Sounds hoverSound = objectSFX.hoverSound;
            SFX.Sounds correctSound = objectSFX.correctSound;

            controller.onHoverEnter.RemoveAllListeners();
            controller.onHoverExit.RemoveAllListeners();
            controller.onSelectEnter.RemoveAllListeners();
            controller.onSelectExit.RemoveAllListeners();

            // then, wire this up so when we drop it it knows what sound to play. Yay for closures.
            controller.onHoverExit.AddListener((XRBaseInteractable item) =>
            {
                if (item)
                {
                    Debug.Log("onHoverExit: " + item.gameObject.name);
                    QueSound(hoverSound);
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
                    Debug.Log("onHoverEnter: " + item.gameObject.name);
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
                    Debug.Log("onSelectEnter item " + item.gameObject.name);
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
                    Debug.Log("onSelectExit: " + item.gameObject.name);
                    QueSound(dropSound);
                }
                else
                {
                    Debug.Log("onSelectExit of unknown item ");
                }
            });

            // this stuff doesn't get called until after the event handlers have already been done, so we have to check if it's happening right now
            // but also bind the one for exit, because on exit there is no destination object.
            if (controller.selectTarget.isHovered)
            {
                Debug.Log("controller.selectTarget.isHovered: " + controller.selectTarget.gameObject.name);
                QueSound(hoverSound);
            }

        }
        else
        {
            // they just dropped the item, so we can't pull data from it. It'll already have exit events bound to it though, so we can just assume those will handle it
            // do nothing
            Debug.Log("ResolveInteractionSounds with a null target. that means you dropped the object");
        }

        /*
        Debug.Log("Resolving interaction sounds for target: " + target.name);
        if (controller.isSelectActive)
        {
            Debug.Log("controller.isSelectActive: " + controller.isSelectActive);
            controller.onSelectExit.AddListener((XRBaseInteractable item) =>
            {
                Debug.Log("Dropping item: " + item.gameObject.name);
            });
        }
        /*
        if (controller.isHoverActive)
        {
            Debug.Log("controller.isHoverActive: " + controller.isSelectActive);
            controller.onHoverExit.AddListener(DropItem);
        }
        */
    }
     
    /*
    public void HoverOffItem(XRBaseInteractable item)
    {
        Debug.Log("HoverOffItem: " + gameObject.name);

    }
    */

}
