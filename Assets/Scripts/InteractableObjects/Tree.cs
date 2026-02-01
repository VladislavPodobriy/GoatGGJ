using UnityEngine;

public class Tree : InteractiveObject
{
    public Rigidbody2D Star;

    string text;

    readonly string ua = "На дереві щось є, але без палиці не дістати";
    readonly string en = "There is something on the tree, but I can't reach it without a stick";

    private void Awake()
    {
        base.Awake();
        text = Language.Instance.language == Language.Instance.ukrainian ? ua : en;
    }

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
            TalkTextController.SpawnTalkText(player.transform.position + new Vector3(-2, 2, 0), text);
        }
    }
}
