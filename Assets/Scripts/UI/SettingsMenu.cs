using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private GameObject box = null;
    [SerializeField] private Slider sfxSlider = null;
    [SerializeField] private Slider musicSlider = null;

    private MusicPlayer music;


    private void Awake()
    {
        music = FindObjectOfType<MusicPlayer>();
    }


    public void Open()
    {
        box.SetActive(true);
        sfxSlider.value = FindObjectOfType<MusicPlayer>().sfxVolume;
        musicSlider.value = FindObjectOfType<MusicPlayer>().musicVolume;
    }


    public void Close()
    {
        box.SetActive(false);
        PlayerPrefs.SetFloat("musicVolume", music.musicVolume);
        PlayerPrefs.SetFloat("sfxVolume", music.sfxVolume);
    }


    public void ChangeSFXVolume(float volume)
    {
        FindObjectOfType<MusicPlayer>().ChangeSFXVolume(volume);
    }


    public void ChangeMusicVolume(float volume)
    {
        FindObjectOfType<MusicPlayer>().ChangeMusicVolume(volume);
    }
}
