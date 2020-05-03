using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundConeManager : MonoBehaviour
{

    [SerializeField]
    private GameObject collidingWith;

    public void OnTriggerEnter(Collider collider)
    {
        collidingWith = collider.gameObject;
        Debug.Log("colliding with " + collider.name);
    }
}
