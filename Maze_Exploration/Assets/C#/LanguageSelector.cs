using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageSelector : MonoBehaviour
{
    public enum Language
    {
        JAPANESE,
        ENGLISH
    };

    public static Language getCurrentLanguage
    {
        get
        {
            return lang;
        }
    }
    private static Language lang;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetJapanese()
    {
        lang = Language.JAPANESE;
    }

    public void SetEnglish()
    {
        lang = Language.ENGLISH;
    }
}