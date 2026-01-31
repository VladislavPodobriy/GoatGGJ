using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DialogueLine
{
    public bool isNPC;
    public string text;
}

public class DialogSystem : InteractiveObject
{
    [SerializeField] TextMeshProUGUI textUI;
    [SerializeField] TextMeshProUGUI speakerNameUI;
    [SerializeField] GameObject dialogueCanvas;
    [SerializeField] Image npcAvatarUI;
    [SerializeField] Image playerAvatarUI;

    [SerializeField] string playerName;
    [SerializeField] string charName;
    [SerializeField] Sprite charAvatar;
    [SerializeField] Sprite playerAvatar;
    [SerializeField] DialogueLine[] dialogue;

    private int dialogueStep = 0;

    public override void Interact()
    {

        if (dialogueStep >= dialogue.Length)
        {
            dialogueCanvas.SetActive(false);
            return;
        }

        if(!dialogueCanvas.activeSelf) dialogueCanvas.SetActive(true);

        DialogueLine line = dialogue[dialogueStep];

        textUI.SetText(line.text);

        UpdateSpeaker(line.isNPC);

        dialogueStep++;

    }

    private void UpdateSpeaker(bool isNPC)
    {
        if (isNPC)
        {
            npcAvatarUI.sprite = charAvatar;
            speakerNameUI.SetText(charName);
        }
        else
        {
            playerAvatarUI.sprite = playerAvatar;
            speakerNameUI.SetText(playerName);
        }
    }

}
