using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class EndLevelUI : MonoBehaviour
{
    [SerializeField] GameObject endLevelScreen = null;


    private void Start()
    {
        endLevelScreen.SetActive(false);    
    }


    public void EndLevel(int result)
    {
        endLevelScreen.SetActive(true);
        endLevelScreen.transform.GetChild(1).GetComponent<Text>().text = "you " + ((result == 0) ? "loses..." : "wins!");
    }


    public void ReturnToMenu()
    {
        FindObjectOfType<MusicPlayer>().PlayTrack(MusicPlayer.Track.MENU);
        SceneManager.LoadScene("MainMenu");
    }
}
