

using UnityEngine;

public class HouseDoor : InteractiveObject
{
    public Scene_Controller HouseScene;

    string text;

    readonly string ua = "Я ще не все знайшла";
    readonly string en = "I still haven’t found everything";

    private void Awake()
    {
        base.Awake();
        text = Env.Instance.language == Env.Instance.ukrainian ? ua : en;
    }

    public override void Interact()
    {
        var player = FindObjectOfType<InventoryManager>();
        if (player.HasItem("ПовнеВідро") && player.HasItem("Борошно"))
        {
            HouseScene.TeleportToLevel(HouseScene.spawnPointLeft, HouseScene);
        }
        else
        {
            TalkTextController.SpawnTalkText(player.transform.position + new Vector3(-2, 2, 0), text);
        }
    }
}
