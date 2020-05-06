﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Mic : XRBaseInteractable
{
    [Tooltip("Used internally to keep track of if this object is being held right now.")]
    public bool isBeingHeld = false;
    [SerializeField]
    [Tooltip("Used internally to keep track of the interacatable component")]
    public XRGrabInteractable interactable;
    [SerializeField][Tooltip("Set automagically. Holds the soundManager reference.")]
    private SoundManager soundManager;

    void Start()
    {
        if (!soundManager)
        {
            soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        }
        if (!soundManager)
        {
            Debug.LogError("Error in Mic Prefab/Mic.cs | Unable to find SoundManager game object. This should get set automatically if there's a gameobject called SoundManager in the scene, with a SoundManager component on it.");
        }
        if (!interactable)
        {
            interactable = GetComponent<XRGrabInteractable>();
        }
        interactable.onSelectEnter.RemoveListener(DidGetSelected);
        interactable.onSelectExit.RemoveListener(DidLoseSelected);
        interactable.onSelectEnter.AddListener(DidGetSelected);
        interactable.onSelectExit.AddListener(DidLoseSelected);

        // we can't set references to an external component's function on a prefab, so we have to do this here.
        interactable.onHoverEnter.RemoveListener(soundManager.ResolveInteractionSounds);
        interactable.onHoverExit.RemoveListener(soundManager.ResolveInteractionSounds);
        interactable.onHoverEnter.AddListener(soundManager.ResolveInteractionSounds);
        interactable.onHoverExit.AddListener(soundManager.ResolveInteractionSounds);

    }

    public void DidGetSelected(XRBaseInteractor interactor)
    {
        var controller = interactor.GetComponent<XRController>();
        XRBaseInteractable remote = interactor.selectTarget;
        isBeingHeld = true;
    }

    public void DidLoseSelected(XRBaseInteractor interactor)
    {
        isBeingHeld = false;
    }
}
