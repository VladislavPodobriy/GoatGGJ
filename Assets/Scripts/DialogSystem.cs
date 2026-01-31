using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class DialogueLine
{
    public bool isNPC;
    public string text;
}

public class DialogSystem : InteractiveObject
{
    public UnityEvent OnComplete;
    
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
    private bool isActive;
    private PlayerController _player;
    private bool _canClick = false;
    
    public override void Interact()
    {
        Activate();
    }

    public void Activate()
    {
        _player = FindObjectOfType<PlayerController>();
        isActive = true;
        _player.ToggleControls(false);
        dialogueCanvas.SetActive(true);
        ShowLine();
    }

    private void ShowLine()
    {
        DialogueLine line = dialogue[dialogueStep];
        textUI.SetText(line.text);
        UpdateSpeaker(line.isNPC);
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && _canClick)
        {
            dialogueStep++;
            if (dialogueStep == dialogue.Length)
            {
                dialogueCanvas.SetActive(false);
                dialogueStep = 0;
                _player.ToggleControls(true);
                isActive = false;
                _canClick = false;
                OnComplete?.Invoke();
            }
            else
                ShowLine();
        }
        if (isActive)
            _canClick = true;
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
        playerAvatarUI.gameObject.SetActive(!isNPC);
        npcAvatarUI.gameObject.SetActive(isNPC);
    }

}
