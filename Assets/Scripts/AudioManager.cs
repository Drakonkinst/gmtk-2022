using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{   
    private AudioSource audioSource;
    
    void Awake() {
        audioSource = GetComponent<AudioSource>();
    }
    
    public void PlaySound(SoundEffect effect) {
        if(effect.sounds == null || effect.sounds.Length <= 0) {
            Debug.LogWarning("No sound found");
            return;
        }
        audioSource.PlayOneShot(effect.sounds[UnityEngine.Random.Range(0, effect.sounds.Length)], effect.volume);
    }
}
