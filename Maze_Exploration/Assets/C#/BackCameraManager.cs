using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BackCameraManager : MonoBehaviour
{
    [SerializeField] private Camera backCamera;

    // Start is called before the first frame update
    void Start()
    {
        backCamera.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyUp(KeyCode.Tab))
        {
            backCamera.enabled = !backCamera.enabled;
        }
    }
}
