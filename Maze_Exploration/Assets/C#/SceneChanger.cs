using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip EnterMaze;
    public AudioClip PressHelpButton;
    
    public Image blackPanel;
    public float blackenSpeed;
    private bool blacken;

    private float t;

    public GameObject loadImage;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        audioSource = GetComponent<AudioSource>();
        
        blackPanel.enabled = false;
        blackPanel.color = new Color(0, 0, 0, 0);
        blacken = false;
        t = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (blacken)
        {
            t += Time.deltaTime * blackenSpeed;
            blackPanel.color = new Color(0, 0, 0, t);
        }
    }

    public void ChangeToMain()
    {
        blackPanel.enabled = true;
        blacken = true;
        audioSource.PlayOneShot(EnterMaze);
        //loadImage.SetActive(true);
        StartCoroutine(nameof(Enu_LoadMain));
    }

    public void ChangeToTutorial()
    {
        blackPanel.enabled = true;
        blacken = true;
        audioSource.PlayOneShot(EnterMaze);
        //loadImage.SetActive(true);
        StartCoroutine(nameof(Enu_LoadTutorial));
    }

    IEnumerator Enu_LoadMain()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Main");
    }

    IEnumerator Enu_LoadTutorial()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Tutorial");
    }


    public void LoadTitle()
    {
        SceneManager.LoadScene("Title");
    }

    public void LoadMain()
    {
        SceneManager.LoadScene("Main");
    }

    public void OnCLickHelpButton()
    {
        audioSource.PlayOneShot(PressHelpButton);
    }

    public void Exit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        
        #else
        Application.Quit();

        #endif
    }
}
