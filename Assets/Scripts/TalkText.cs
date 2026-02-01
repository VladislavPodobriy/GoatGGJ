using TMPro;
using UnityEngine;

public class TalkText : MonoBehaviour
{
    public TextMeshProUGUI Text;

    public void SetText(string text)
    {
        Text.SetText(text);
    }
}
