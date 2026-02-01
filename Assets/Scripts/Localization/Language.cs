using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Language : Singleton<Language>
{
    public string language = "Ukrainian";

    public string ukrainian = "Ukrainian";
    public string english = "English";


    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    public void SwitchLanguage()
    {
        language = language == ukrainian ? english : ukrainian;
    }
}
