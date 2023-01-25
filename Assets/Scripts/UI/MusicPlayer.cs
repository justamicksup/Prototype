using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{

    public AudioSource audioSource;
    public AudioClip[] audioMusic;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource.clip = audioMusic[Random.Range(0, audioMusic.Length)];
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
