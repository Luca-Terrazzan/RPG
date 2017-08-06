using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSounds : MonoBehaviour
{
    private AudioSource backgroundSound;
    [HideInInspector] public AudioClip[] audioList;
    public enum backgorundAudioList {FreeRoaming, Fattoria, Becchino, Prigione, Saloon, Bordello, Boss }
    public backgorundAudioList audio;
    
	// Use this for initialization
	void Start ()
    {
        backgroundSound = GetComponent<AudioSource>();
        backgroundSound.clip = audioList[audio.GetHashCode()];
        backgroundSound.Play();
        
	}
	

}
