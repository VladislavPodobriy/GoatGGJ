using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MainScripts.Controllers;
using MainScripts.Spine;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class InventorySlot
{
    public Image image;
    public string title;
}

public class InventoryManager : MonoBehaviour
{
    public List<ItemEntry> items = new List<ItemEntry>();
    public List<InventorySlot> slots = new List<InventorySlot>();
    public SpineSkinsController SkinsController;
    public GameObject Windows;
    public AudioClip TakeItem;
    private Item _stick;
    private Item _star;
    private Item _boroshno;
    private Item _fullBucket;
    
    public Canvas InventoryCanvas;

    private void Start()
    {
        SetupInventorySlots();
    }

    private void Update()
    {
        CheckShouldDrop();
        SwitchInventoryWindow();
    }

    #region INVENTORY_ACTIONS

    private void SwitchInventoryWindow()
    {
        //if (Input.GetKeyDown(KeyCode.I))
            //InventoryCanvas.gameObject.SetActive(!InventoryCanvas.gameObject.activeSelf);
    }

    private void SetupInventorySlots()
    {
        /*foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Inventory_Slot_Icon"))
        {
            slots.Add(new InventorySlot
            {
                image = obj.GetComponent<Image>(),
                title = "",
            });
        }*/

        //InventoryCanvas.gameObject.SetActive(false);
    }

    public bool HasItem(string title)
    {
        return (items.Exists(e => e.item.data.title == title)) ;
    }
    
    public void Add(Item item)
    {
        if (items.Exists(e => e.item == item)) return;

        AudioController.PlayAtWorldPosition(TakeItem, transform.position);
        
        if (item.data.title == "Палиця")
            _stick = item;
        else if (item.data.title == "Зірка")
            _star = item;
        else if (item.data.title == "Борошно")
            _boroshno = item;
        else if (item.data.title == "ПовнеВідро")
            _fullBucket = item;
        
        items.Add(new ItemEntry
        {
            item = item,
            data = item.data,
        });
        
        foreach (var slot in slots)
        {
            if (!slot.image.enabled)
            {
                slot.image.enabled = true;
                slot.image.sprite = item.data.icon;
                slot.title = item.data.title;
                break;
            }
        }

        item.gameObject.SetActive(false);
        
        var player = FindObjectOfType<PlayerController>();
        if (_stick != null && _star != null && !player.HasStaff)
        {
            SkinsController.TryAddSkin("Staff");
            player.HasStaff = true;
            Remove(_stick);
            Remove(_star);
            var text = Env.Instance.language == Env.Instance.ukrainian ? 
                "Посох вже працює, але треба знайти останню частину" : 
                "The staff is already working, but I need to find the last part";
            TalkTextController.SpawnTalkText(player.transform.position + new Vector3(-2, 2, 0), 
                text);
            TutorialController.Instance.ShowThird();
        }
        if (_fullBucket != null && _boroshno != null && !player.HomeOpened)
        {
            Windows.gameObject.SetActive(true);
            player.HomeOpened = true;
            var text = Env.Instance.language == Env.Instance.ukrainian ? 
                "Я зібрала все, що потрібно" : 
                "I’ve gathered everything I need";
            TalkTextController.SpawnTalkText(player.transform.position + new Vector3(-2, 4, 0), 
                text);
        }
    }

    public void Remove(string title)
    {
        var item = items.FirstOrDefault(i => i.item.data.title == title)?.item;
        if (item != null)
            Remove(item);
    }
    
    public void Remove(Item item)
    {
        items.RemoveAll(e => e.item == item);

        InventorySlot slot = slots.FirstOrDefault(e => e.title == item.data.title);
        if (slot != null)
        {
            slot.image.sprite = null;
            slot.title = null;
            slot.image.enabled = false;
        }
    }

    void CheckShouldDrop()
    {
        var selectedEntry = items.FirstOrDefault(e => e.data.isSelected);
        if (selectedEntry != null) Drop(selectedEntry.item);
    }

    public void DropFirst()
    {
        Item item = items.First().item;

        item.gameObject.SetActive(true);
        item.data.isSelected = false;
        item.transform.position = transform.position;

        Remove(item);
    }

    public void Drop(Item item)
    {
        item.gameObject.SetActive(true);
        item.data.isSelected = false;
        item.transform.position = transform.position;

        Remove(item);
    }

    #endregion
}
