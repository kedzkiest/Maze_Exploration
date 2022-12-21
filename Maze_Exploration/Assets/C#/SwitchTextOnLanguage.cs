using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchTextOnLanguage : MonoBehaviour
{
    private Text text;
    [SerializeField, Multiline(3)] private string JapaneseText;
    [SerializeField, Multiline(3)] private string EnglishText;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (LanguageSelector.getCurrentLanguage)
        {
            case LanguageSelector.Language.JAPANESE:
                text.text = JapaneseText;
                break;
            case LanguageSelector.Language.ENGLISH:
                text.text = EnglishText;
                break;
        }
    }
}
