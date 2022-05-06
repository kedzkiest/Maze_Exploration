using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningSoundAudible : MonoBehaviour
{
    private AudioSource audioSource;

    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(gameObject.transform.position, player.transform.position) < audioSource.maxDistance)
        {
            audioSource.enabled = true;
        }
        else
        {
            audioSource.enabled = false;
        }
    }
}
