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
    
    public GameObject sliderGameObject;
    private Slider stamina;
    public Image staminaBar;
    public bool canDash;

    public bool canEscape;
    public Image escapePanel;
    
    private AudioSource messagePopUpAudioSource;
    public AudioClip messagePopUp;
    private bool escapeMessagePopUp;
    private bool lightMessagePopUp;
    private bool backCameraMessagePopUp;
    
    [CanBeNull] private GameObject  player;
    [CanBeNull] private GameObject  goalLight;

    private float time;

    private float t;
    // Start is called before the first frame update
    void Start()
    {
        cameraComponent = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Camera>();
        
        messagePopUpAudioSource = GetComponent<AudioSource>();
        
        player = GameObject.FindGameObjectWithTag("Player");
        goalLight = GameObject.FindGameObjectWithTag("Goal");

        stamina = sliderGameObject.GetComponent<Slider>();

        canDash = true;

        t = 0;
    }
  

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        timeText.text = "Time: " + time.ToString();

        // when goal is near the player, an instruction pops up
        if(Vector3.Distance(player.transform.position, goalLight.transform.position) < 5.0f)
        {
            if (!escapeMessagePopUp)
            {
                escapeMessagePopUp = true;
                messagePopUpAudioSource.PlayOneShot(messagePopUp);
            }
            escapeText.enabled = true;
        }
        else
        {
            escapeText.enabled = false;
        }

        if (escapeText.enabled == true && Input.GetKeyDown(KeyCode.Q))
        {
            canEscape = true;
            Invoke(nameof(LoadTitle), 3.0f);
        }

        if (canEscape)
        {
            t += Time.deltaTime * 2;
            escapePanel.color = new Color(255, 255, 255, t);
        }

        // on game start instruction pops up
        if (time > 5.0f && time < 10.0f)
        {
            if (!lightMessagePopUp)
            {
                lightMessagePopUp = true;
                messagePopUpAudioSource.PlayOneShot(messagePopUp);
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
                messagePopUpAudioSource.PlayOneShot(messagePopUp);
            }

            backCameraText.enabled = true;
        }
        else
        {
            backCameraText.enabled = false;
        }

        // Stamina UI
        if (stamina.GetComponent<Slider>().value >= 1)
        {
            sliderGameObject.SetActive(false);
        }
        else
        {
            sliderGameObject.SetActive(true);
        }

        if (stamina.value <= 0)
        {
            canDash = false;
        }

        if (stamina.value >= 0.5f)
        {
            canDash = true;
        }

        if (canDash)
        {
            staminaBar.color = Color.yellow;
            
            if(Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
            {
                stamina.value -= 0.003f;
            }
            else if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                stamina.value += 0.001f;
            }
            else
            {
                stamina.value += 0.003f;
            }
        }
        else
        {
            staminaBar.color = Color.red;

            if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                stamina.value += 0.001f;
            }
            else
            {
                stamina.value += 0.003f;
            }
        }
    }
    
    public void LoadTitle()
    {
        SceneManager.LoadScene("Title");
    }
}
