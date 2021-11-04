using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject box = null;
    [SerializeField] private GameObject standardLayout = null;
    [SerializeField] private GameObject menuLayout = null;
    [SerializeField] private SettingsMenu settingsMenu = null;


    void Start()
    {
        box.SetActive(false);
    }


    public void Open()
    {
        box.SetActive(true);
        standardLayout.SetActive(true);
        menuLayout.SetActive(false);
        //settingsMenu.Close();
    }


    public void Resume()
    {
        box.SetActive(false);
        FindObjectOfType<GameController>().PauseGame();
    }


    public void OpenSettings()
    {
        settingsMenu.Open();
    }


    public void ReturnToMenu()
    {
        standardLayout.SetActive(false);
        menuLayout.SetActive(true);
    }


    public void CancelReturnToMenu()
    {
        standardLayout.SetActive(true);
        menuLayout.SetActive(false);
    }


    public void ConfirmReturnToMenu()
    {
        FindObjectOfType<MusicPlayer>().PlayTrack(MusicPlayer.Track.MENU);
        SceneManager.LoadScene("MainMenu");
    }
}
