using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private List<AudioClip> clips;
    AudioSource audioSource;

    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
        
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(int num)
    {
        if(num > clips.Count || num < 0 || clips[num] == null){
            Debug.Log("You cant Count");
            return;
        }
        audioSource.PlayOneShot(clips[num]);
    }
}