using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public static string reason;
    
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Text>().text = reason;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
