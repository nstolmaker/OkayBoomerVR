using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundConeManager : MonoBehaviour
{

    // fields used internally only
    [SerializeField][Tooltip("Set automagically.")]
    private GameObject collidingWith;
    [SerializeField][Tooltip("Set automagically.")]
    private bool inTheConeZone = false;
    [SerializeField][Tooltip("Set automagically.")]
    private Transform centerOfTheConeZone;
    [SerializeField][Tooltip("Set automagically.")]
    private Transform microphonePickup;
    [SerializeField][Tooltip("Set automagically.")]
    private Ray rayFromMic;
    private SoundManager soundManager;



    // can be set manually, but have automatic values
    [Tooltip("Drag the transform from Talky Talky here. Otherwise it'll be automatically set to the parent gameobject's transform.")]
    public Transform talkyTalky;
    [Tooltip("Perfect Mic Distance. Aka the 'perfect distance'.")]
    public float perfectDistance = 0.2f;
    [Tooltip("Critical hit is perfectDistance +/- perfectDistanceAllowedVariancePercent")]
    public float perfectDistanceAllowedVariancePercent = 0.1f;
    [Tooltip("Tell us where the director is, so that we can make sounds come from him")]
    public GameObject director;


    void Awake()
    {
        if (talkyTalky == null)
        {
            talkyTalky = transform.parent.GetComponent<Transform>();
        }
        if (director == null)
        {
            director = GameObject.Find("Director") ? GameObject.Find("Director") : null;
        }
        Destroy(director.GetComponent<AudioSource>());
        director.AddComponent<AudioSource>();
        if (soundManager == null)
        {
            soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>() ?? null;
        }
    }
    public void OnTriggerEnter(Collider collider)
    {
        collidingWith = collider.gameObject;
        Debug.Log("colliding with " + collider.name);

        // let's determine how well they did at nailing the position.
        // the ideal position should be in the center of the waveform cone (ConeZone), pointing at the sound generator (TalkyTalky)
        // we just need to draw a triangle between these three points, and then look at the angles.
        // Scratch that, that's too much work, let's just compare the transform rotations/positions of the michead and the center of the cone.
        centerOfTheConeZone = transform;
        microphonePickup = collidingWith.transform;
        // NOTE: We could add rotation here as well, but at the moment I think we only care about rotation along two of the three axis
        rayFromMic = new Ray(microphonePickup.position, microphonePickup.forward);
        inTheConeZone = true;

    }

    public void OnTriggerExit(Collider collider)
    {
        inTheConeZone = false;
    }

    void Update()
    {
        if (inTheConeZone)
        {
            CheckIfRayCastHit();
        }
    }

    public void CheckIfRayCastHit()
    {
        //Debug.Log("CheckIfRayCastHit running...");
        RaycastHit hit;
            if (Physics.Raycast(rayFromMic, out hit))
            {
            //Debug.DrawRay(centerOfTheMic.position, centerOfTheMic.forward, Color.blue, 5);
            if (hit.collider.gameObject.name == talkyTalky.gameObject.name)
            {
                //Debug.Log("Hitting: " + hit.collider.gameObject.name + ". Looking for: " + talkyTalky.gameObject.name + "; Distance is: " + hit.distance);
                //Debug.DrawRay(centerOfTheMic.position, centerOfTheMic.forward, Color.red, 5);
                // let's see how far away it is, and compute the floor and ceiling for our sweet-spot. Give them a reward if they hit it.
                var perfectDistanceMin = perfectDistance + (-1 * perfectDistanceAllowedVariancePercent);
                var perfectDistanceMax = perfectDistance + (1 * perfectDistanceAllowedVariancePercent);
                if (hit.distance > perfectDistanceMin && hit.distance < perfectDistanceMax)
                {
                    //Debug.Log("Critical Hit!" + "Distance is: " + hit.distance);
                    PerfectPositionHit(microphonePickup.gameObject, hit.collider.gameObject);
                }
            }
            }


    }


    public void PerfectPositionHit(GameObject whichMic, GameObject whichTalker)
    {
        // if they've got the perfect position, and they're actively holding the mic, let them know.
        if (whichMic.GetComponentInParent<Mic>().isBeingHeld)
        {
            // make sure we don't play it too often.
            if (soundManager.CheckSoundEffectDebounce())
            {
                Debug.Log("Perfect position hit! Mic was: " + whichMic.name + ". Talker was :" + whichTalker.name);
                soundManager.QueSound(whichMic.GetComponentInParent<SoundEffects>().correctSound);
            }
        }
    }


}