using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioClip[] attack = null;
    [SerializeField] private AudioClip[] hit = null;
    [SerializeField] private AudioClip death = null;
    [SerializeField] private AudioClip ability = null;

    private AudioSource audioSource;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }


    private void Start()
    {
        audioSource.volume = FindObjectOfType<MusicPlayer>().sfxVolume;
    }


    public void PlayAttackClip()
    {
        audioSource.clip = attack[Random.Range(0, attack.Length)];
        audioSource.Play();
    }

    
    public void PlayHitClip()
    {
        audioSource.clip = hit[Random.Range(0, hit.Length)];
        audioSource.Play();
    }


    public void PlayDeathClip()
    {
        audioSource.clip = death;
        audioSource.Play();
    }


    public void PlayAbilityClip()
    {
        audioSource.clip = ability;
        audioSource.Play();
    }
}
