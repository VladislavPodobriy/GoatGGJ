using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        if (Input.GetKeyDown(KeyCode.I))
            InventoryCanvas.gameObject.SetActive(!InventoryCanvas.gameObject.activeSelf);
    }

    private void SetupInventorySlots()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Inventory_Slot_Icon"))
        {
            slots.Add(new InventorySlot
            {
                image = obj.GetComponent<Image>(),
                title = "",
            });
        }

        InventoryCanvas.gameObject.SetActive(false);
    }

    public void Add(Item item)
    {
        if (items.Exists(e => e.item == item)) return;

        items.Add(new ItemEntry
        {
            item = item,
            data = item.data,
        });

        int slotIndex = items.Count - 1;
        if (slotIndex < slots.Count)
        {
            slots[slotIndex].image.sprite = item.data.icon;
            slots[slotIndex].title = item.data.title;
        }

        item.gameObject.SetActive(false);
    }

    public void Remove(Item item)
    {
        items.RemoveAll(e => e.item == item);

        InventorySlot slot = slots.Where(e => e.title == item.data.title).First();
        slot.image.sprite = null;
        slot.title = null;
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
