﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]

public class TRexWalkScript : MonoBehaviour
{

    void OnAnimatorMove()
    {
        Animator animator = GetComponent<Animator>();

        if (animator)
        {
            Vector3 newPosition = transform.position;
            newPosition.z += animator.GetFloat("WalkSpeed") * Time.deltaTime;
            transform.position = newPosition;
        }
    }
}