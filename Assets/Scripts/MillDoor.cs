

using UnityEngine;

public class MillDoor : InteractiveObject
{
    public Scene_Controller MillScene;
    
    public override void Interact()
    {
        var player = FindObjectOfType<PlayerController>();
        if (player.HasBell)
        {
            MillScene.TeleportToLevel(MillScene.spawnPointLeft, MillScene);
        }
        else
        {
            TalkTextController.SpawnTalkText(player.transform.position + new Vector3(-2, 2, 0), 
                "Там щось страшне...");
        }
    }
}
