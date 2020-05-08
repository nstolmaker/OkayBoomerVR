using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using UnityEngine;
using UnityEngine.SceneManagement;

#if LIH_PRESENT
using UnityEngine.Experimental.XR.Interaction;
#endif

public class MenuInteraction : MonoBehaviour
{
    private void Start()
    {
        //interactor = controller.GetComponent<XRRayInteractor>();
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(1);
    }

    public void ExitLevel()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitToDesktop()
    {
        Application.Quit();
    }
}
