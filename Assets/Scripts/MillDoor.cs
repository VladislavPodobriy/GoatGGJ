public class MillDoor : InteractiveObject
{
    public Scene_Controller MillScene;
    
    public override void Interact()
    {
        MillScene.TeleportToLevel(MillScene.spawnPointLeft, MillScene);
    }
}
