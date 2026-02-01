

using UnityEngine;

public class HouseDoor : InteractiveObject
{
    public Scene_Controller HouseScene;
    
    public override void Interact()
    {
        var player = FindObjectOfType<InventoryManager>();
        if (player.HasItem("ПовнеВідро") && player.HasItem("Борошно"))
        {
            HouseScene.TeleportToLevel(HouseScene.spawnPointLeft, HouseScene);
        }
        else
        {
            TalkTextController.SpawnTalkText(player.transform.position + new Vector3(-2, 2, 0), 
                "Я ще не все знайшла");
        }
    }
}
