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
using System;

public class TutorialUI : MonoBehaviour
{
    private Camera cameraComponent;

    [SerializeField] private Text[] popUpTextOnStart;
    
    [SerializeField] private Slider stamina;
    [SerializeField] private Image staminaBar;
    [SerializeField] private bool canDash;

    [SerializeField] private Slider destroy;
    [SerializeField] private Image destroyBar;
    [SerializeField] private bool canDestroy;

    [SerializeField] private bool canEscape;
    [SerializeField] private Image escapePanel;
    [SerializeField] private Text escapeText;

    private AudioSource audioSource;
    public AudioClip messagePopUp;
    private bool escapeMessagePopUp;
    private bool playMessagePopUpSound;
    
    [CanBeNull] private GameObject  player;
    [CanBeNull] private GameObject  goalLight;

    private float distUpdateFreq = 0.3f;
    private float elapsedTimeForDist;
    private float distBTWPlayerAndGoal = 10000;

    private float time;

    private float escapePanelFlashTime;

    [SerializeField] private float StaminaConsumeSpeed;
    [SerializeField] private float StaminaRecoverSpeed_Rest;
    [SerializeField] private float StaminaRecoverSpeed_Walking;
    [SerializeField] private float DestroyRechargeSpeed;

    [SerializeField] private AudioClip DestroyCharged;
    private bool DestroyRechargeSound_isCalledOnce = true;

    // Start is called before the first frame update
    void Start()
    {
        cameraComponent = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Camera>();
        
        audioSource = GetComponent<AudioSource>();
        
        player = GameObject.FindGameObjectWithTag("Player");
        goalLight = GameObject.FindGameObjectWithTag("Goal");

        canDash = true;
        destroy.value = 0;

        escapePanelFlashTime = 0;
    }
  

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        #region instruction on escape
        elapsedTimeForDist += Time.deltaTime;
        if (elapsedTimeForDist > distUpdateFreq)
        {
            elapsedTimeForDist = 0;
            
            if(goalLight != null)
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
            escapePanelFlashTime += Time.deltaTime * 2;
            escapePanel.color = new Color(255, 255, 255, escapePanelFlashTime);
        }
        #endregion

        #region instruction on game start
        // on game start instruction pops up
        int textIndex = 0;
        popUpTextOnStart[textIndex].enabled = true;
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
                if (tag == "UnbreakableWall") return;

                Destroy(clickedGameObject);
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (GameObject enemy in enemies) {
                    enemy.GetComponent<RunningEnemyController>().VisitDestination(clickedGameObject.transform.position);
                }
                DestroyRechargeSound_isCalledOnce = false;
           
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
    
    [SerializeField] private void LoadTitle()
    {
        SceneManager.LoadScene("Title");
    }
}
