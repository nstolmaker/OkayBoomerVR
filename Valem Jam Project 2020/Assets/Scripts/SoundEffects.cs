using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    // %%%
    [Tooltip("Object Sound Profile. Used mostly as a reference on the object itself, so that when it gets interacted with, we can resolve it's sound effects.")]
    public SFX.Sounds pickupSound;
    public SFX.Sounds dropSound;
    public SFX.Sounds activateSound;
    public SFX.Sounds hoverSound;
    public SFX.Sounds correctSound;


}
