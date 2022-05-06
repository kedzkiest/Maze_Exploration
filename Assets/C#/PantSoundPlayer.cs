using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PantSoundPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip pant;

    private bool isCalledOnce;

    public UIController UIController;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (UIController.canDash)
        {
            isCalledOnce = false;
            audioSource.volume -= 0.01f;
            if (audioSource.volume <= 0)
            {
                audioSource.Stop();
            }
        }
        else
        {
            if (!isCalledOnce)
            {
                isCalledOnce = true;
                audioSource.volume = 1;
                audioSource.Play();
            }
        }
    }
}
