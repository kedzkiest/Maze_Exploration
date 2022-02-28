using System.Collections;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    private Camera cameraComponent;

    public Text timeText;
    public Text escapeText;
    public Text lightText;
    public Text backCameraText;
    
    private AudioSource audioSource;
    public AudioClip messagePopUp;
    private bool escapeMessagePopUp;
    private bool lightMessagePopUp;
    private bool backCameraMessagePopUp;
    
    [CanBeNull] private GameObject  player;
    [CanBeNull] private GameObject  goalLight;

    private float time;
    // Start is called before the first frame update
    void Start()
    {
        cameraComponent = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Camera>();
        
        audioSource = GetComponent<AudioSource>();
        
        player = GameObject.FindGameObjectWithTag("Player");
        goalLight = GameObject.FindGameObjectWithTag("Goal");
    }
  

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        timeText.text = "Time: " + time.ToString();

        if(Vector3.Distance(player.transform.position, goalLight.transform.position) < 5.0f)
        {
            if (!escapeMessagePopUp)
            {
                escapeMessagePopUp = true;
                audioSource.PlayOneShot(messagePopUp);
            }
            escapeText.enabled = true;
        }
        else
        {
            escapeText.enabled = false;
        }
        
        if (escapeText.enabled == true && Input.GetKeyDown(KeyCode.Q))
        {
            SceneManager.LoadScene("Title");
        }
        

        if (time > 5.0f && time < 10.0f)
        {
            if (!lightMessagePopUp)
            {
                lightMessagePopUp = true;
                audioSource.PlayOneShot(messagePopUp);
            }
            lightText.enabled = true;
        }
        else
        {
            lightText.enabled = false;
        }

        
        if (time > 12.0f && time < 17.0f)
        {
            if (!backCameraMessagePopUp)
            {
                backCameraMessagePopUp = true;
                audioSource.PlayOneShot(messagePopUp);
            }

            backCameraText.enabled = true;
        }
        else
        {
            backCameraText.enabled = false;
        }
    }
}
