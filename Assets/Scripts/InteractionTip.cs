using TMPro;
using UnityEngine;

public class InteractionTip : MonoBehaviour
{
   [SerializeField]
   private TextMeshProUGUI _text;
   
   public void SetText(string text)
   {
      _text.SetText(text + " [E]");
   }
}
