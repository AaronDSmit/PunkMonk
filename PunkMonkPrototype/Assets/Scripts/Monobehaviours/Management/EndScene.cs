using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class EndScene : MonoBehaviour
{

    private void Awake()
    {
        if(GameObject.FindGameObjectWithTag("Manager") != null)
        {
            Destroy(GameObject.FindGameObjectWithTag("Manager"));
        }
    }
    public void Quit()
    {
        Application.Quit();
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(0);
    }
}