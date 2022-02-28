using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightScript : MonoBehaviour
{
    public GameObject flashLight;

    [SerializeField] private AudioClip[] audioClips;

    private AudioSource audioSource;
    private Light lightComponent;
    private bool isLightOn = false;
    public float lightIntensity;
    public float lightTremor;
    private float t;

    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        lightComponent = flashLight.GetComponent<Light>();
        lightComponent.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        t += 0.1f;
        if (t > 1000)
        {
            t = 0;
            lightComponent.intensity = 0;
        }
        
        if(Input.GetKeyDown(KeyCode.T))
        {
            lightComponent.enabled = isLightOn ? false : true;
            isLightOn = !isLightOn;
            
            audioSource.PlayOneShot(audioClips[flashLight.activeSelf ? 0 : 1]);
        }

        lightComponent.intensity = lightIntensity * (1 + Mathf.Abs(Mathf.Sin(lightTremor * t)));
    }
}
