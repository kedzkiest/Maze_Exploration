using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChaseMusicController : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip chaseMusic;

    public AudioClip foundSE;

    private GameObject[] enemies;
    private List<GameObject> chasingEnemies = new List<GameObject>();
    private bool isChasing;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    private bool isCalledOnce;
    // Update is called once per frame
    void Update()
    {
        
        foreach (GameObject obj in enemies)
        {
            if(obj.GetComponent<RunningEnemyController>().isChasing)
            {
                if (!chasingEnemies.Contains(obj))
                {
                    chasingEnemies.Add(obj);
                }

                isChasing = true;
            }
        }

        if (chasingEnemies.Count == 0) isChasing = false;
        
        if (chasingEnemies.Count > 0)
        {
            if (!chasingEnemies.ElementAt(0).GetComponent<RunningEnemyController>().isChasing)
            {
                chasingEnemies.RemoveAt(0);
            }
        }
        
        
        
        if (isChasing)
        {
            if (!isCalledOnce)
            {
                isCalledOnce = true;
                audioSource.volume = 1;
                audioSource.PlayOneShot(foundSE);
                audioSource.clip = chaseMusic;
                audioSource.Play();
                /*
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
                */
            }
        }
        else
        {
            audioSource.volume -= 0.002f;
            if (audioSource.volume == 0)
            {
                audioSource.Stop();
            }
            isCalledOnce = false;
        }
    }
}
