using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class EndLevelMenu : MonoBehaviour
{
    [SerializeField] private GameObject box = null;
    [SerializeField] private GameObject victoryLayout = null;
    [SerializeField] private GameObject defeatLayout = null;



    private void Start()
    {
        box.SetActive(false);
    }


    public void OnVictory()
    {
        FindObjectOfType<MusicPlayer>().PlayTrack(MusicPlayer.Track.GAME_OVER);
        box.SetActive(true);
        victoryLayout.SetActive(true);
        defeatLayout.SetActive(false);
        FindObjectOfType<SaveGameManager>().IncrementProgress();
    }


    public void OnDefeat()
    {
        FindObjectOfType<MusicPlayer>().PlayTrack(MusicPlayer.Track.GAME_OVER);
        box.SetActive(true);
        victoryLayout.SetActive(false);
        defeatLayout.SetActive(true);
    }


    public void DoNextLevel()
    {
        SceneManager.LoadScene(FindObjectOfType<SaveGameManager>().GetProgress());
    }


    public void ReturnToMenu()
    {
        FindObjectOfType<MusicPlayer>().PlayTrack(MusicPlayer.Track.MENU);
        SceneManager.LoadScene("MainMenu");
    }
}
