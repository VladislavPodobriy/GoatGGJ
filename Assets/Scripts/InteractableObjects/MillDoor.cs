

using UnityEngine;

public class MillDoor : InteractiveObject
{
    public Scene_Controller MillScene;

    string text;

    string ua = "Там щось страшне...";
    string en = "There is something scary inside...";

    private void Awake()
    {
        base.Awake();
        text = Language.Instance.language == Language.Instance.ukrainian ? ua : en;
    }

    public override void Interact()
    {
        var player = FindObjectOfType<PlayerController>();
        if (player.HasBell)
        {
            MillScene.TeleportToLevel(MillScene.spawnPointLeft, MillScene);
        }
        else
        {
            TalkTextController.SpawnTalkText(player.transform.position + new Vector3(-2, 2, 0), text);
        }
    }
}
