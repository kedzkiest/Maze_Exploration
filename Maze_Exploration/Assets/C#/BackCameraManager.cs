using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BackCameraManager : MonoBehaviour
{
    public Camera backCamera;
    public Camera mainCamera;

    public SkinnedMeshRenderer[] hideOnBackCameraEnabled;
    private bool isBackCameraON = false;

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
            isBackCameraON = !isBackCameraON;

            foreach(SkinnedMeshRenderer s in hideOnBackCameraEnabled)
            {
                s.enabled = !isBackCameraON;
            }
            
            backCamera.enabled = !backCamera.enabled;
        }

        backCamera.transform.position = mainCamera.transform.position;
        backCamera.transform.localRotation = Quaternion.Euler(mainCamera.transform.localRotation.x,mainCamera.transform.localRotation.y + 180, mainCamera.transform.localRotation.z);
    }
}
