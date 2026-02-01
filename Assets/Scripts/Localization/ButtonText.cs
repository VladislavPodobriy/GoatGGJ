using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.TextCore.Text;
using UnityEngine;

public class ButtonText : MonoBehaviour
{
    private TextMeshProUGUI textUI;
    private Language language;

    private void Start()
    {
        textUI = GetComponentInChildren<TextMeshProUGUI>();
        language = FindObjectOfType<Language>();
    }

    private void Update()
    {
        if(textUI.text != Language.Instance.language)
            textUI.SetText(Language.Instance.language);
    }
}
