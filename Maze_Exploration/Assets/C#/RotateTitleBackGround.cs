using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTitleBackGround : MonoBehaviour
{
    public float rotateSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float t = Mathf.Sin(Time.time / 5) + 1;
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotateSpeed);
        RenderSettings.skybox.SetFloat("_Exposure", t);
    }
}
