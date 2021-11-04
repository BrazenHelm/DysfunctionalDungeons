using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGameManager : MonoBehaviour
{
    public static SaveGameManager instance;
    private int levelProgress = 1;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }


    private void Start()
    {
        if (PlayerPrefs.HasKey("levelProgress"))
        {
            instance.levelProgress = PlayerPrefs.GetInt("levelProgress");
            Debug.Log("save game progress = " + instance.levelProgress);
        }
    }


    public void SetProgress(int value)
    {
        instance.levelProgress = value;
        Debug.Log("save game progress = " + instance.levelProgress);
        PlayerPrefs.SetInt("levelProgress", value);
    }


    public int GetProgress()
    {
        Debug.Log("save game progress = " + instance.levelProgress);
        return instance.levelProgress;
    }


    public void IncrementProgress()
    {
        instance.levelProgress++;
        Debug.Log("save game progress = " + instance.levelProgress);
        PlayerPrefs.SetInt("levelProgress", instance.levelProgress);
    }
}
