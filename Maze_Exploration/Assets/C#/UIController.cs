using System.Collections;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using System.Runtime.CompilerServices;

public class UIController : MonoBehaviour
{
    private Camera cameraComponent;

    public Text timeText;
    public Text escapeText;
    public Text lightText;
    public Text backCameraText;
    
    public Slider stamina;
    public Image staminaBar;
    public bool canDash;

    public Slider destroy;
    public Image destroyBar;
    public bool canDestroy;

    public bool canEscape;
    public Image escapePanel;
    
    private AudioSource audioSource;
    public AudioClip messagePopUp;
    private bool escapeMessagePopUp;
    private bool lightMessagePopUp;
    private bool backCameraMessagePopUp;
    
    [CanBeNull] private GameObject  player;
    [CanBeNull] private GameObject  goalLight;

    private float distUpdateFreq = 0.3f;
    private float elapsedTimeForDist;
    private float distBTWPlayerAndGoal = 10000;

    private float time;

    private float t;

    public Maze maze;

    public float StaminaConsumeSpeed;
    public float StaminaRecoverSpeed_Rest;
    public float StaminaRecoverSpeed_Walking;
    public float DestroyRechargeSpeed;

    public AudioClip DestroyCharged;
    private bool DestroyRechargeSound_isCalledOnce = true;

    // Start is called before the first frame update
    void Start()
    {
        cameraComponent = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Camera>();
        
        audioSource = GetComponent<AudioSource>();
        
        player = GameObject.FindGameObjectWithTag("Player");
        goalLight = GameObject.FindGameObjectWithTag("Goal");

        canDash = true;

        t = 0;
    }
  

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        timeText.text = "Time: " + time.ToString();

        #region instruction on escape
        elapsedTimeForDist += Time.deltaTime;
        if (elapsedTimeForDist > distUpdateFreq)
        {
            elapsedTimeForDist = 0;
            distBTWPlayerAndGoal = Vector3.Distance(player.transform.position, goalLight.transform.position);
        }

        // when goal is near the player, an instruction pops up
        if (distBTWPlayerAndGoal < 5.0f)
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
            canEscape = true;
            Invoke(nameof(LoadTitle), 3.0f);
        }

        if (canEscape)
        {
            t += Time.deltaTime * 2;
            escapePanel.color = new Color(255, 255, 255, t);
        }
        #endregion

        #region instruction on game start
        // on game start instruction pops up
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
        #endregion

        #region stamina
        // Stamina UI
        if (stamina.value >= 1)
        {
            stamina.gameObject.SetActive(false);
        }
        else
        {
            stamina.gameObject.SetActive(true);
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
                stamina.value -= StaminaConsumeSpeed * Time.deltaTime;
            }
            else if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                stamina.value += StaminaRecoverSpeed_Walking * Time.deltaTime;
            }
            else
            {
                stamina.value += StaminaRecoverSpeed_Rest * Time.deltaTime;
            }
        }
        else
        {
            staminaBar.color = new Color(0.552f, 0.584f, 0.016f, 1);

            if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                stamina.value += StaminaRecoverSpeed_Walking * Time.deltaTime;
            }
            else
            {
                stamina.value += StaminaRecoverSpeed_Rest * Time.deltaTime;
            }
        }
        #endregion

        #region destroy
        if (destroy.value <= 0)
        {
            canDestroy = false;
        }

        if (destroy.value >= 1.0f)
        {
            canDestroy = true;

            if (!DestroyRechargeSound_isCalledOnce)
            {
                DestroyRechargeSound_isCalledOnce = true;
                audioSource.PlayOneShot(DestroyCharged);
            }
        }

        if (canDestroy)
        {
            destroyBar.color = Color.red;

            if (Input.GetMouseButtonDown(0))
            {
                GameObject clickedGameObject = null;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit = new RaycastHit();

                if (Physics.Raycast(ray, out hit))
                {
                    clickedGameObject = hit.collider.gameObject;
                }
                else
                {
                    return;
                }

                string tag = hit.collider.tag;
                if (tag == "Ground" || tag == "MainCamera" || tag == "Enemy") return;
                Destroy(clickedGameObject);
                DestroyRechargeSound_isCalledOnce = false;
                Invoke(nameof(RebakeNavMesh), 0.1f);
           
                destroy.value = 0;
            }
        }
        else
        {
            destroyBar.color = new Color(0.45f, 0, 0, 1);

            destroy.value += DestroyRechargeSpeed * Time.deltaTime;
        }
        #endregion
    }

    void RebakeNavMesh()
    {
        maze.GetComponent<NavMeshSurface>().useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        maze.GetComponent<NavMeshSurface>().BuildNavMesh();
    }
    
    public void LoadTitle()
    {
        SceneManager.LoadScene("Title");
    }
}
