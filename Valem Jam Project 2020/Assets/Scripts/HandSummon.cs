using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class HandSummon : MonoBehaviour
{
    // must be set by hand
    [Tooltip("The material to use to indicate the user screwed up")]
    public Material errorMaterial;
    [Tooltip("The normal material, so we can switch back to it")]
    public Material okMaterial;


    // can be set, but populated automatically too.
    [Tooltip("Set automagically. Left hand controller game object. If empty it'll just look for a gameobject called 'LeftHand Controller'")]
    public XRBaseControllerInteractor l_hand;
    [Tooltip("Set automagically. Right hand controller game object. If empty it'll just look for a gameobject called 'RightHand Controller'")]
    public XRBaseControllerInteractor r_hand;
    [Tooltip("Set automagically.")] [SerializeField]
    private XRRig xrRig;
    [Tooltip("The colliders that you must slide yours hands to, in order to summon")][SerializeField]
    private List<Collider> endCapColliders = null;
    [Tooltip("The intensity of the vibrations triggered with hand summoning")][SerializeField]
    private float hapticIntensity = 0.2f;
    [Tooltip("Duration for fading when stuff fades")][SerializeField]
    float lerpDuration = 3f;

    // used internally
    [SerializeField][Tooltip("Used internally. dont touch")]
    private List<XRController> xrControllers = null;    // basically the same as the other list, but these ones get set internally to the XRController component, which we often need for haptic pulses and stuff like that. Feels redundant.
    [SerializeField]
    private bool leftHandBegin = false;
    [SerializeField]
    private bool rightHandBegin = false;
    [SerializeField]
    private bool leftHandDone = false;
    [SerializeField]
    private bool rightHandDone = false;
    [SerializeField]
    private float debounceLastTime = 0;
    [SerializeField]
    private bool step1 = false;
    [SerializeField]
    private bool step2 = false;
    [SerializeField]
    private float lerpStart = 0;
    [SerializeField]
    private bool errorIndicationHappening = false;
    [SerializeField]
    private bool instantiateIndicationHappening = false; 



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
            xrControllers.Add(l_hand.GetComponent<XRController>());
        }
        if (!r_hand)
        {
            r_hand = GameObject.Find("RightHand Controller").GetComponent<XRBaseControllerInteractor>() ?? null;
            xrControllers.Add(r_hand.GetComponent<XRController>());
        }
        if (!l_hand || !r_hand)
        {
            Debug.LogError("Error in HandSummon.cs: Unable to find hands. Either define them manually by dragging your controllers to the l_hand and r_hand fields in the inspector, or name them LeftHand Controller and RightHand Controller so I can find them automagically.");
        }

        // ensure materials are defined
        if (!errorMaterial)
        {
            Debug.LogError("HandSummon.cs | Missing material for errorMaterial! This is bad. You should set this in the SummonManager to a material");
        }
        if (!okMaterial)
        {
            Debug.LogError("HandSummon.cs | Missing material for okMaterial! This is bad. You should set this in the SummonManager to a material");
        }

        // make sure there are endCapColliders
        if (endCapColliders.Count == 0)
        {
            Debug.Log("No endcap colliders defined, so looking for and adding the expected ones by name 'EndCapLeft' and 'EndCapRight'");
            endCapColliders.Add(GameObject.Find("EndCapLeft").GetComponent<BoxCollider>());
            endCapColliders.Add(GameObject.Find("EndCapRight").GetComponent<BoxCollider>());
        }
        if (endCapColliders.Count != 2)
        {
            Debug.LogError("Error in HandSummon. No endcaps defined. Summoning won't work. Either you have to specify these manually in the inspector, or name them EndCapLeft and EndCapRight");
        }
        foreach (BoxCollider collider in endCapColliders)
        {
            Debug.Log("Binding colliders for endcaps: " + collider.gameObject.name);
            HandSummonEndCaps endcap = collider.GetComponent<HandSummonEndCaps>();
            // a nice little Lambda so we can bind events in this script instead of having to split this script up even more.
            endcap.EndcapCollisionStart += (GameObject other) => {
                Debug.Log("EndcapCollisionStart triggered by: " + other.name);
                switch (other.transform.parent.gameObject.name)
                {
                    case "LeftHand Controller":
                        this.leftHandDone = true;
                        other.transform.parent.GetComponent<XRBaseControllerInteractor>().SendHapticImpulse(hapticIntensity, 0.05f);
                        break;
                    case "RightHand Controller":
                        this.rightHandDone = true;
                        other.transform.parent.GetComponent<XRBaseControllerInteractor>().SendHapticImpulse(hapticIntensity, 0.05f);
                        break;
                    default:
                        Debug.LogWarning("Warning in HandSummon.cs:EndcapCollisionStart triggered by gameobject trigger thats not LeftHand Controller or RightHand Controller. If summoning doesn't work, this could be a clue. If everything works fine, don't worry about it.");
                        break;
                }
                
            };
            endcap.EndcapCollisionEnd += (GameObject other) => {
                Debug.Log("EndcapCollisionEnd triggered by: " + other.name);
                switch (other.transform.parent.gameObject.name)
                {
                    case "LeftHand Controller":
                        leftHandDone = false;
                        break;
                    case "RightHand Controller":
                        rightHandDone = false;
                        break;
                    default:
                        Debug.LogWarning("Warning in HandSummon.cs:EndcapCollisionEnd triggered by gameobject trigger thats not LeftHand Controller or RightHand Controller. If summoning doesn't work, this could be a clue. If everything works fine, don't worry about it.");
                        break;
                }
            };
        }
    }

    public void VibrateBothControllers(float intensity, float duration)
    {
        foreach (XRController device in xrControllers)
        {
            device.SendHapticImpulse(intensity, duration);
        }
    }

    public void Update()
    {
         

        if (rightHandBegin && leftHandBegin)
        {
            if (!step1)
            {
                Debug.Log("Step1 begun! ...");
                SoundManager soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
                soundManager.QueSound(SFX.Sounds.Correct);
                // show them the thingy so they have some kind of visible cue
                gameObject.GetComponent<MeshRenderer>().enabled = true;
                gameObject.GetComponent<MeshRenderer>().material = okMaterial;
                //gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
                step1 = true;   // first step is to get both hands in
            }
            if (HandsInsideBox()) {

                if (Debounce(0.25f))
                {
                    VibrateBothControllers(0.5f, 0.25f);
                }
                /* if (gameObject.GetComponent<MeshRenderer>().material != okMaterial)
                {
                    gameObject.GetComponent<MeshRenderer>().material = okMaterial;
                }
                */
            } else
            {
                ResetSummon();
                FlashRedThenHide();
            }

            // if their hands make it to the endCaps, then they've done it, and they should get an object.
            if (rightHandDone && leftHandDone)
            {
                // and we haven't celebrated yet
                if (!step2) { 
                    Debug.Log("Summoning Item!");
                    SoundManager soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
                    soundManager.QueSound(SFX.Sounds.Correct);
                    step1 = false;
                    step2 = true;
                    WarpInNewObject("mic1");
                }
            }

        }

        // step2 is when we actually summon the object and maybe let them size it and stuff. 
        // It's basically it's own thing, and we no longer care where their hands are.
        if (step2)
        {

        }

        HandleTweeningIfNeeded();
    }

    public void HandleTweeningIfNeeded()
    {
        // this gets started by the FlashRedThenHide() function. One day there might be more than one tween here, so we'll want to break those out.
        if (errorIndicationHappening)
        {
            var progress = Time.time - lerpStart;
            var renderer = GetComponent<MeshRenderer>();
            float newAlpha = Mathf.Lerp(lerpDuration, 0.0f, progress / lerpDuration);
            errorMaterial.color = new Color(errorMaterial.color.r, errorMaterial.color.g, errorMaterial.color.b, newAlpha);
            renderer.material = errorMaterial; //new Color(0, 0, 0, newAlpha);
            Debug.Log("FlashRedThenHide running... " + newAlpha + ".");

            if (lerpDuration < progress)
            {
                Debug.Log("FlashRedThenHide Done! ");
                errorIndicationHappening = false;
                //gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
        }

        // for warping in new objects
        if (instantiateIndicationHappening)
        {
            // do the stuff like above, if we watn to.
        }
    }

    public void WarpInNewObject(string objectName)
    {
        Debug.Log("Warping in new one!");
        UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Mic1.prefab", typeof(GameObject));
        var rotationForHands = Quaternion.Euler(293.188843f, 90.5345764f, 269.418457f);
        GameObject newMic = Instantiate(prefab, GetComponent<Renderer>().bounds.center, rotationForHands) as GameObject;
        instantiateIndicationHappening = true;
        //newMic.SetActive(true);

    }

    public void ResetSummon()
    {
        // they moved their hands too far outside of the allowed bounds while doing the gesture, so reset everything. TODO: make this a function.
        Debug.Log("ResetSummon");
        SoundManager soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        soundManager.QueSound(SFX.Sounds.DropMic);
        rightHandBegin = false;
        leftHandBegin = false;
        rightHandDone = false;
        leftHandDone = false;
        step1 = false;
        step2 = false;
    }

    public void FlashRedThenHide()
    {
        lerpStart = Time.time;
        errorIndicationHappening = true;
    }

    public void FancyWarpIn()
    {
        lerpStart = Time.time;
        instantiateIndicationHappening = true;
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
            if (HoldingBothGrips()) { 
                leftHandBegin = true;
            }
        }
        if (hitByParent.name == r_hand.gameObject.name)
        {
            // right hand triggered by entering the summon area
            if (HoldingBothGrips())
            {
                rightHandBegin = true;
            }
            // TODO: Check rotation of hands as well. Left hand would be Vector3(339.273499, 277.711548, 98.3878937)
        }
    }

    /* leaving this for reference, but now we're going to trigger the exit based on the vector2 of the hand position versus the summonbox posiion.
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
         * */

    public bool HandsInsideBox()
    {
        var handsInBox = 0;
        Renderer rend = GetComponent<Renderer>();
        if (rend.bounds.Contains(l_hand.transform.position))
        {
            //Debug.Log("HandsInsideBox | Lefthand is in the box");
            handsInBox++;
        }
        if (rend.bounds.Contains(r_hand.transform.position))
        {
            //Debug.Log("HandsInsideBox | Right hand is in the box");
            handsInBox++;
        }

        return handsInBox > 0;
    }

    public bool HoldingBothGrips()
    {
        var gripsBeingHeld = 0;
        foreach (XRController controller in xrControllers)
        {
            if (controller.inputDevice.TryGetFeatureValue(CommonUsages.grip, out float grip))
            {
                if (grip > 0.1)
                {
                    gripsBeingHeld++;
                }
            }
        }
        
        return gripsBeingHeld == 2;
    }

    /* Cool stuff that i'm not using anymore. maybe move the fuzzy thing to a util. maybe not */
    /*
    public bool HandsLeftXAxis()
    {
        // prepare fuzzyNumbers, because exact axis numbers are impossible.
        float allowedVariance = 0.03f;
        Vector3 boxAxis = transform.position;
        (float, float) fuzzyNumbersY = FuzzNumber(boxAxis.y, allowedVariance);
        (float, float) fuzzyNumbersZ = FuzzNumber(boxAxis.z, allowedVariance);
        //Debug.Log("FuzzyNumbers returned: "+fuzzyNumbers.Item1+", "+fuzzyNumbers.Item2);

        // make sure our y position is above the minimum and below the maximum allowed "fuzzyness"
        if (l_hand.transform.position.y > fuzzyNumbersY.Item1 && l_hand.transform.position.y < fuzzyNumbersY.Item2)
        {
            //Debug.Log("l_hand We're in the sweet spot.");
            return false;
        }
        if (r_hand.transform.position.y > fuzzyNumbersY.Item1 && r_hand.transform.position.y < fuzzyNumbersY.Item2)
        {
            //Debug.Log("r_hand We're in the sweet spot.");
            return false;
        }
        // TODO: copy-paste for Z values and then test. Might make it too hard?
        return true;
    }

    // Takes a number and returns an anonymous Tupple which are equal to the floor and the ceiling of that number, after applying the variance.
    public (float, float) FuzzNumber(float input, float variance)
    {
        float min = input - variance;
        float max = input + variance;
        return (min, max);
    }
    */

}
