using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Receiver : InteractiveObject
{

    public override void Interact()
    {
        FindObjectOfType<InventoryManager>().DropFirst();
    }
}
