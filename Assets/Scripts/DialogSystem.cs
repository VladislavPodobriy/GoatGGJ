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
    public string text_ua;
    public string text_en;
}

public class DialogSystem : InteractiveObject
{
    public UnityEvent OnComplete;
    
    [SerializeField] TextMeshProUGUI textUI;
    [SerializeField] TextMeshProUGUI speakerNameUI;
    [SerializeField] GameObject dialogueCanvas;
    [SerializeField] Image npcAvatarUI;
    [SerializeField] Image playerAvatarUI;

    [SerializeField] string playerNameEn;
    [SerializeField] string playerName;
    [SerializeField] private string charNameEn;
    [SerializeField] string charName;
    [SerializeField] Sprite charAvatar;
    [SerializeField] Sprite playerAvatar;
    [SerializeField] DialogueLine[] dialogue;

    private int dialogueStep = 0;
    private bool isActive;
    private PlayerController _player;
    private bool _canClick = false;
    private bool _mouseDown = false;
    
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
        string text = Env.Instance.language == Env.Instance.ukrainian ? line.text_ua : line.text_en;
        textUI.SetText(text);
        UpdateSpeaker(line.isNPC);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && _canClick)
            _mouseDown = true;
        if (Input.GetMouseButtonUp(0) && _canClick && _mouseDown)
        {
            _mouseDown = false;
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
            var value = Env.Instance.language == Env.Instance.ukrainian ? charName : charNameEn;
            speakerNameUI.SetText(value);
        }
        else
        {
            playerAvatarUI.sprite = playerAvatar;
            var value = Env.Instance.language == Env.Instance.ukrainian ? playerName : playerNameEn;
            speakerNameUI.SetText(value);
        }
        playerAvatarUI.gameObject.SetActive(!isNPC);
        npcAvatarUI.gameObject.SetActive(isNPC);
    }

}
