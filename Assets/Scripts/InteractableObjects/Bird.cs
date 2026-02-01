using UnityEngine;

public class Bird : InteractiveObject
{
    string text;

    readonly string ua = "Йди сюди, маленька пташко";
    readonly string en = "Come here, little bird";

    private void Awake()
    {
        base.Awake();
        text = Language.Instance.language == Language.Instance.ukrainian ? ua : en;
    }

    public override void Interact()
    {
        var player = FindObjectOfType<PlayerController>();
        player.AddBird();
        TalkTextController.SpawnTalkText(player.transform.position + new Vector3(-2, 2, 0), text);
        Destroy(gameObject);
    }
}
