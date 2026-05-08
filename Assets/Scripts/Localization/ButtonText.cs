using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _langText;
    [SerializeField]
    private Button _langBtn;
    [SerializeField]
    private Toggle _lowHpToggle;
    [SerializeField]
    private Toggle _endlessHealToggle;
    
    private void Start()
    {
        _langBtn.onClick.AddListener(() =>
        {
            Env.Instance.SwitchLanguage();
        });
        Env.Instance.OnLanguageChanged.AddListener(UpdateLangText);
        UpdateLangText();
        
        _lowHpToggle.onValueChanged.AddListener((x) =>
        {
            Env.Instance.ToggleLowHp(x);
        });
        Env.Instance.OnLowHpChanged.AddListener(() =>
        {
            _lowHpToggle.isOn = Env.Instance.lowHp;
        });
        _lowHpToggle.isOn = Env.Instance.lowHp;
        
        _endlessHealToggle.onValueChanged.AddListener((x) =>
        {
            Env.Instance.ToggleEndlessHeal(x);
        });
        Env.Instance.OnEndlessHealChanged.AddListener(() =>
        {
            _endlessHealToggle.isOn = Env.Instance.endlessHeal;
        });
        _endlessHealToggle.isOn = Env.Instance.endlessHeal;
    }

    private void UpdateLangText()
    {
        if (Env.Instance.language == Env.Instance.ukrainian)
            _langText.text = "UA";
        else
            _langText.text = "EN";
    }
}
