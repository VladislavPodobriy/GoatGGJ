using UnityEngine;

public class Bird : InteractiveObject
{
    public override void Interact()
    {
        var player = FindObjectOfType<PlayerController>();
        player.AddBird();
        TalkTextController.SpawnTalkText(player.transform.position + new Vector3(-2, 2, 0), 
            "Йди сюди, маленька пташко");
        Destroy(gameObject);
    }
}
