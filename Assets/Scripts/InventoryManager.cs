using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public List<ItemEntry> items = new List<ItemEntry>();

    private void Update()
    {
        CheckShouldDrop();
    }

    #region INVENTORY_ACTIONS

    public void Add(Item item)
    {
        if (items.Exists(e => e.item == item)) return;

        items.Add(new ItemEntry
        {
            item = item,
            data = item.data,
        });

        item.gameObject.SetActive(false);
    }

    public void Remove(Item item)
    {
        print("Remove from Items"+ item.data.name);

        items.RemoveAll(e => e.item == item);
    }

    void CheckShouldDrop()
    {
        if (items.Exists(e => e.item.data.isSelected))
        {
            Drop(items.Select(e => e.item).First());
        }
    }

    void Drop(Item item)
    {
        item.gameObject.SetActive(true);
        item.transform.position = transform.position;

        Remove(item);
    }

    #endregion
}
