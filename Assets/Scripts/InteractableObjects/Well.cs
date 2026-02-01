using UnityEngine;

public class Well : InteractiveObject
{
    public Item FullBucket;

    string text_success;
    string text_fail;

    readonly string ua_success = "Вода, водиця!";
    readonly string en_success = "Water, dear sweet water!";

    readonly string ua_fail = "Тут немає відра...";
    readonly string en_fail = "There is no bucket, here";

    private void Awake()
    {
        base.Awake();
        text_success = Language.Instance.language == Language.Instance.ukrainian ? ua_success : en_success;
        text_fail = Language.Instance.language == Language.Instance.ukrainian ? ua_fail : en_fail;
    }

    public override void Interact()
    {
        var player = FindObjectOfType<InventoryManager>();
        if (player.HasItem("Відро"))
        {
            player.Remove("Відро");
            FindObjectOfType<InventoryManager>().Add(FullBucket);
            ToggleInteractable(false);
            TalkTextController.SpawnTalkText(player.transform.position + new Vector3(-2, 2, 0), text_success);
        }
        else
        {
            TalkTextController.SpawnTalkText(player.transform.position + new Vector3(-2, 2, 0), text_fail);
        }
    }
}
