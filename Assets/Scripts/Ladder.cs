using UnityEngine;

public class Ladder : InteractiveObject
{
    public Transform Start;
    public Transform End;

    public override void Interact()
    {
        var player = FindObjectOfType<PlayerController>();
        player.ClimbLadder(this);
    }
}
