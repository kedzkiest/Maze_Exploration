using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMusicController : MonoBehaviour
{
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MusicFadeOut()
    {
        audioSource.volume -= 0.01f;
    }
}
