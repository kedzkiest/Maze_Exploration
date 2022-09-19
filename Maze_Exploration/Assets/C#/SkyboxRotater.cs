using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxRotater : MonoBehaviour
{
    public float rotateSpeed;
    public bool isDarken;

    // Start is called before the first frame update
    void Start()
    {
        RenderSettings.skybox.SetFloat("_Rotation", 0);
    }

    // Update is called once per frame
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotateSpeed);

        if (isDarken)
        {
            float t = Mathf.Sin(Time.time / 5) + 1;
            RenderSettings.skybox.SetFloat("_Exposure", t);
        }
        else
        {
            RenderSettings.skybox.SetFloat("_Exposure", 2);
        }
    }
}
