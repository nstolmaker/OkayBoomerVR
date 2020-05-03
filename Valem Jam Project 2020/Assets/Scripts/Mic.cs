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
        interactable.onSelectEnter.RemoveAllListeners();
        interactable.onSelectExit.RemoveAllListeners();
        interactable.onSelectEnter.AddListener(DidGetSelected);
        interactable.onSelectExit.AddListener(DidLoseSelected);

    }

    public void DidGetSelected(XRBaseInteractor interactor)
    {
        Debug.Log("Mic selected?");
        var controller = interactor.GetComponent<XRController>();
        XRBaseInteractable remote = interactor.selectTarget;


        if (controller.gameObject.name == "RightHand Controller")
        {
            Debug.Log("Yep, with the RightHand Controller!");
            isBeingHeld = true;
        }
    }

    public void DidLoseSelected(XRBaseInteractor interactor)
    {
        isBeingHeld = false;
    }
}
