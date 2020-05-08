using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActuallyStartFirstScene : MonoBehaviour
{

    // Start is called before the first frame update
    void Awake()
    {
        SceneManager.LoadScene(1);
    }


    void Start()
    {
        SceneManager.LoadScene(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
