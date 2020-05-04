﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandSummon : MonoBehaviour
{
    // must be set by hand

    // can be set, but populated automatically too.
    [Tooltip("Set automagically. Left hand controller game object. If empty it'll just look for a gameobject called 'LeftHand Controller'")]
    public XRBaseControllerInteractor l_hand;
    [Tooltip("Set automagically. Right hand controller game object. If empty it'll just look for a gameobject called 'RightHand Controller'")]
    public XRBaseControllerInteractor r_hand;
    [Tooltip("Set automagically.")] [SerializeField]
    private XRRig xrRig;

    // used internally
    [SerializeField]
    public Vector3 summonCubeCenter;
    [SerializeField]
    public Vector3 l_handCenter;
    [SerializeField]
    public Vector3 r_handCenter;
    [SerializeField]
    private bool leftHandIn = false;
    [SerializeField]
    private bool rightHandIn = false;
    [SerializeField]
    private float debounceLastTime = 0;

    public void Start()
    {
        if (!xrRig)
        {
            xrRig = GameObject.Find("XR Rig").GetComponent<XRRig>() ?? null;
        }
        if (!xrRig)
        {
            Debug.LogError("Critical Error in HandSummon.cs. Cannot find XR Rig named 'XR Rig', did you rename it? If so, drag it into the XRRig field in HandSummon via the inspector.");
        }
        // set up the hands
        if (!l_hand)
        {
            l_hand = GameObject.Find("LeftHand Controller").GetComponent<XRBaseControllerInteractor>() ?? null;
        }
        if (!r_hand)
        {
            r_hand = GameObject.Find("RightHand Controller").GetComponent<XRBaseControllerInteractor>() ?? null;
        }
        if (!l_hand || !r_hand)
        {
            Debug.LogError("Error in HandSummon.cs: Unable to find hands. Either define them manually by dragging your controllers to the l_hand and r_hand fields in the inspector, or name them LeftHand Controller and RightHand Controller so I can find them automagically.");
        }

        // set up the summoning space
        summonCubeCenter = transform.position;
    }

    public void Update()
    {
        /*
        this.summonCubeCenter = transform.position;
        this.l_handCenter = l_hand.transform.position;
        this.r_handCenter = r_hand.transform.position;

        Vector3 newSummonBoxPosition = new Vector3(xrRig.transform.position.x, xrRig.transform.position.y+0.575f, xrRig.transform.position.z+0.4f);
        transform.position = newSummonBoxPosition;
        */
        if (rightHandIn && leftHandIn)
        {
            if (Debounce(1f))
            {
                gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
                Debug.Log("Woohoo!");
                SoundManager soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
                soundManager.QueSound(SFX.Sounds.Correct);
            }
        } else
        {

        }
    }

    // kind of a utility class. Should probably put this somewhere with utils
    public bool Debounce(float debounceTime)
    {

        // if the current time is greater than the last time we played a sound + the defined minimum sound interval, then...
        if (Time.time > debounceLastTime + debounceTime)
        {
            debounceLastTime = Time.time;
            return true;
        }
        return false;
    }

    public void OnTriggerEnter(Collider other)
    {
        GameObject hitByParent = other.GetComponent<Transform>().parent.gameObject;  // :-O
        Debug.Log("HandSummon::onTriggerEnter:" + hitByParent.name);
        if (hitByParent.name == l_hand.gameObject.name)
        {
            // left hand triggered by entering the summon area
            leftHandIn = true;
        }
        if (hitByParent.name == r_hand.gameObject.name)
        {
            // right hand triggered by entering the summon area
            rightHandIn = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        GameObject hitByParent = other.GetComponent<Transform>().parent.gameObject;  // :-O
        Debug.Log("HandSummon::OnTriggerExit:" + hitByParent.name);
        if (hitByParent.name == l_hand.gameObject.name)
        {
            // left hand triggered by entering the summon area
            leftHandIn = false;
        }
        if (hitByParent.name == r_hand.gameObject.name)
        {
            // right hand triggered by entering the summon area
            rightHandIn = false;
        }
    }

}
