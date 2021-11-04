using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class CreditsUI : MonoBehaviour
{
    [SerializeField] private GameObject box = null;
    [SerializeField] private GameObject panel = null;

    private void Start()
    {
        box.SetActive(false);
        panel.SetActive(false);
    }


    public void Open()
    {
        box.SetActive(true);
        panel.SetActive(true);
    }


    public void ReturnToMenu()
    {
        FindObjectOfType<SaveGameManager>().SetProgress(1);
        FindObjectOfType<MusicPlayer>().PlayTrack(MusicPlayer.Track.MENU);
        SceneManager.LoadScene(0);
    }
}
