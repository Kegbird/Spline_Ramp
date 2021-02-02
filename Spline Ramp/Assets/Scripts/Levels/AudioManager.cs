using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource audio_source;
    [SerializeField]
    private AudioClip button_pressed_fx;
    [SerializeField]
    private AudioClip win_fx;
    [SerializeField]
    private AudioClip confirm_fx;
    [SerializeField]
    private AudioClip tap_fx;
    [SerializeField]
    private AudioClip coin_pick_fx;
    [SerializeField]
    private AudioClip bounce_fx;

    private void Awake()
    {
        audio_source = GetComponent<AudioSource>();
    }

    public void PlayButtonPressedSound()
    {
        audio_source.clip = button_pressed_fx;
        audio_source.Play();
    }

    public void PlayWinSound()
    {
        audio_source.clip = win_fx;
        audio_source.Play();
    }

    public void PlayConfirmSound()
    {
        audio_source.clip = confirm_fx;
        audio_source.Play();
    }

    public void TapSound()
    {
        audio_source.clip = tap_fx;
        audio_source.Play();
    }

    public void PlayCoinPickedSound()
    {
        audio_source.clip = coin_pick_fx;
        audio_source.Play();
    }
}
