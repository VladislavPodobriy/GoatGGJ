using UnityEngine;

public class Tree : InteractiveObject
{
    public Rigidbody2D Star;

    public override void Interact()
    {
        var player = FindObjectOfType<InventoryManager>();
        if (player.HasItem("Палиця"))
        {
            Star.simulated = true;
            ToggleInteractable(false);
        }
        else
        {
            TalkTextController.SpawnTalkText(player.transform.position + new Vector3(-2, 2, 0), 
                "На дереві щось є, але без палиці не дістати");
        }
    }
}
