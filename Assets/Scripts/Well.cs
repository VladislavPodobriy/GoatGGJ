using UnityEngine;

public class Well : InteractiveObject
{
    public Item FullBucket;
    
    public override void Interact()
    {
        var player = FindObjectOfType<InventoryManager>();
        if (player.HasItem("Відро"))
        {
            player.Remove("Відро");
            FindObjectOfType<InventoryManager>().Add(FullBucket);
            ToggleInteractable(false);
            TalkTextController.SpawnTalkText(player.transform.position + new Vector3(-2, 2, 0), 
                "Вода, водиця!");
        }
        else
        {
            TalkTextController.SpawnTalkText(player.transform.position + new Vector3(-2, 2, 0), 
                "Тут немає відра...");
        }
    }
}
