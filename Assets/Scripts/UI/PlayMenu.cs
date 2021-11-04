using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayMenu : MonoBehaviour
{
    [SerializeField] private GameObject box = null;
    [SerializeField] private GameObject standardLayout = null;
    [SerializeField] private GameObject confirmLayout = null;

    private SaveGameManager save;


    private void Awake()
    {
        save = FindObjectOfType<SaveGameManager>();
    }


    public void Open()
    {
        box.SetActive(true);
        standardLayout.SetActive(true);
        confirmLayout.SetActive(false);
    }


    public void Close()
    {
        box.SetActive(false);
    }


    public void NewGame()
    {
        if (save.GetProgress() > 1)
        {
            GoToConfirmLayout();
        }
        else
        {
            ConfirmNewGame();
        }
    }


    public void ResumeGame()
    {
        SceneManager.LoadScene(save.GetProgress());
    }


    private void GoToConfirmLayout()
    {
        standardLayout.SetActive(false);
        confirmLayout.SetActive(true);
    }


    public void ConfirmNewGame()
    {
        save.SetProgress(1);
        SceneManager.LoadScene(1);
    }


    public void CancelNewGame()
    {
        standardLayout.SetActive(true);
        confirmLayout.SetActive(false);
    }
}
