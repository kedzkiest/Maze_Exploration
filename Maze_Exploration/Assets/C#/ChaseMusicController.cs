using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChaseMusicController : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip chaseMusic;

    public AudioClip foundSE;

    private List<GameObject> enemies;
    private List<GameObject> chasingEnemies = new List<GameObject>();
    private bool isChasing;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList<GameObject>();
    }

    private bool isCalledOnce;
    // Update is called once per frame
    void Update()
    {
        
        foreach (GameObject obj in enemies)
        {
            if (obj == null)
            {
                enemies.Remove(obj);
                break;
            }

            if (obj.GetComponent<RunningEnemyController>().isChasing)
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
            foreach(GameObject obj in chasingEnemies)
            {
                if(obj == null)
                {
                    chasingEnemies.Remove(obj);
                    break;
                }
                
                if (!obj.GetComponent<RunningEnemyController>().isChasing)
                {
                    chasingEnemies.Remove(obj);
                    break;
                }
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
