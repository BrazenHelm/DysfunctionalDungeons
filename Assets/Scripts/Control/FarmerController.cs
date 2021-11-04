using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FarmerController : UnitController
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Play()
    {
        SceneManager.LoadScene(1);
        //Application.LoadLevel(1);
    }


    public void Quit()
    {
        Application.Quit();
    }
}
