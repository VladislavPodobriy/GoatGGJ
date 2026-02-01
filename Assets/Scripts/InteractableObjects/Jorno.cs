using UnityEngine;

public class Jorno : InteractiveObject
{
    public Item Boroshno;


    string text_success;
    string text_fail;

    readonly string ua_success = "Чудове борошно вийшло!";
    readonly string en_success = "Flour came out wonderfully";

    readonly string ua_fail = "Без пшениці борошна не буде";
    readonly string en_fail = "Without wheat, there will be no flour!";

    private void Awake()
    {
        base.Awake();
        text_success = Language.Instance.language == Language.Instance.ukrainian ? ua_success : en_success;
        text_fail = Language.Instance.language == Language.Instance.ukrainian ? ua_fail : en_fail;
    }

    public override void Interact()
    {
        var player = FindObjectOfType<InventoryManager>();
        if (player.HasItem("Пшениця"))
        {
            player.Remove("Пшениця");
            FindObjectOfType<InventoryManager>().Add(Boroshno);
            ToggleInteractable(false);
            TalkTextController.SpawnTalkText(player.transform.position + new Vector3(-2, 2, 0), text_success);
        }
        else
        {
            TalkTextController.SpawnTalkText(player.transform.position + new Vector3(-2, 2, 0), text_fail);
        }
    }
}
