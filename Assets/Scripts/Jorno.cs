using UnityEngine;

public class Jorno : InteractiveObject
{
    public Item Boroshno;
    
    public override void Interact()
    {
        var player = FindObjectOfType<InventoryManager>();
        if (player.HasItem("Пшениця"))
        {
            player.Remove("Пшениця");
            FindObjectOfType<InventoryManager>().Add(Boroshno);
            ToggleInteractable(false);
            TalkTextController.SpawnTalkText(player.transform.position + new Vector3(-2, 2, 0), 
                "Чудове борошно вийшло!");
        }
        else
        {
            TalkTextController.SpawnTalkText(player.transform.position + new Vector3(-2, 2, 0), 
                "Без пшениці борошна не буде");
        }
    }
}
