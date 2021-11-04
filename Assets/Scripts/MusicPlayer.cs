using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public enum Track
    {
        MENU, TUTORIAL, BOSS, GAME_OVER
    }

    [SerializeField] AudioClip menuMusic = null;
    [SerializeField] AudioClip tutorialMusic = null;
    [SerializeField] AudioClip bossMusic = null;
    [SerializeField] AudioClip gameOverMusic = null;
    private AudioSource speaker;

    public static MusicPlayer instance = null;


    public float musicVolume = 1.0f;
    public float sfxVolume = 1.0f;


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
            speaker = GetComponent<AudioSource>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeMusicVolume(PlayerPrefs.GetFloat("musicVolume"));
        ChangeSFXVolume(PlayerPrefs.GetFloat("sfxVolume"));
        PlayTrack(Track.MENU);
    }

    
    public void PlayTrack(Track track)
    {
        switch (track)
        {
            case Track.MENU:
                instance.speaker.clip = menuMusic;
                instance.speaker.loop = true;
                break;
            case Track.TUTORIAL:
                instance.speaker.clip = tutorialMusic;
                instance.speaker.loop = true;
                break;
            case Track.BOSS:
                instance.speaker.clip = bossMusic;
                instance.speaker.loop = true;
                break;
            case Track.GAME_OVER:
                instance.speaker.clip = gameOverMusic;
                instance.speaker.loop = false;
                break;
        }

        instance.speaker.Play();
    }


    public void ChangeMusicVolume(float volume)
    {
        instance.musicVolume = Mathf.Clamp(volume, 0.0f, 1.0f);
        instance.speaker.volume = musicVolume;
    }


    public void ChangeSFXVolume(float volume)
    {
        instance.sfxVolume = Mathf.Clamp(volume, 0.0f, 1.0f);
    }
}
