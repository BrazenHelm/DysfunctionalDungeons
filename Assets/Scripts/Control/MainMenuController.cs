using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainMenuController : MonoBehaviour
{
    [SerializeField] private PlayMenu play = null;
    [SerializeField] private SettingsMenu settings = null;


    private void Start()
    {
        play.Close();
        settings.Close();
    }


    public void Play()
    {
        play.Open();
    }


    public void Quit()
    {
        Application.Quit();
    }


    public void OpenSettings()
    {
        settings.Open();
    }
}
