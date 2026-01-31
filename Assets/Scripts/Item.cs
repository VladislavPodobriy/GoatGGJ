using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ItemData
{
    public string name;
    public string description;
    public int id;
    public Sprite icon;
    public bool isSelected;
}
public class Item : InteractiveObject
{
    public ItemData data;
    public override void Interact()
    {
        FindObjectOfType<InventoryManager>().Add(this);
    }
}

[System.Serializable]
public class ItemEntry
{
    public Item item;
    public ItemData data;
}


