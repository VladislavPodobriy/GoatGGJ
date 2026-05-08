using TMPro;
using UnityEngine;

public class LocalizedUIText : MonoBehaviour
{
    private TextMeshProUGUI text;
    [SerializeField] private string ua;
    [SerializeField] private string en;
    
    void Start()
    {
        Env.Instance.OnLanguageChanged.AddListener(UpdateText);
        UpdateText();   
    }

    public void UpdateText()
    {
        text = GetComponent<TextMeshProUGUI>();
        if (Env.Instance.language == Env.Instance.ukrainian)
            text.SetText(ua);
        else
            text.SetText(en);
    }
}
