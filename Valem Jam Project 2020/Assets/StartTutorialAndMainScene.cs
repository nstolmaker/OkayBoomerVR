using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTutorialAndMainScene : MonoBehaviour
{
    public GameObject buttonForStarting;
    public GameObject tutorialTimeline;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ControllerGeometry"))
        {
            tutorialTimeline.SetActive(true);
        }
    }
}
