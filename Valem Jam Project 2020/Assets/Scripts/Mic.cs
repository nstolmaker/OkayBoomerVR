using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Mic : XRBaseInteractable
{
    [SerializeField][Tooltip("Used internally to keep track of if this object is being held right now.")]
    public bool isBeingHeld = false;
    [SerializeField]
    [Tooltip("Used internally to keep track of the interacatable component")]
    public XRGrabInteractable interactable;

    void Start()
    {
        if (!interactable)
        {
            interactable = GetComponent<XRGrabInteractable>();
        }
        interactable.onSelectEnter.RemoveListener(DidGetSelected);
        interactable.onSelectExit.RemoveListener(DidLoseSelected);
        interactable.onSelectEnter.AddListener(DidGetSelected);
        interactable.onSelectExit.AddListener(DidLoseSelected);

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
