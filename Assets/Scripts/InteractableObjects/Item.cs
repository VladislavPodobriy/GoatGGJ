using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ItemData
{
    public string title;
    public string description;
    public int id;
    public Sprite icon;
    public bool isSelected;
    public string talkText;
    public string talkTextEn;
}

[System.Serializable]
public class ItemEntry
{
    public Item item;
    public ItemData data;
}
public class Item : InteractiveObject
{
    public ItemData data;
    public override void Interact()
    {
        var player = FindObjectOfType<InventoryManager>();
        player.Add(this);
        var text = Env.Instance.language == Env.Instance.ukrainian ? data.talkText : data.talkTextEn;
        if (!string.IsNullOrWhiteSpace(text))
        {
            TalkTextController.SpawnTalkText(player.transform.position + new Vector3(-2, 2, 0), text);
        }
    }
}


