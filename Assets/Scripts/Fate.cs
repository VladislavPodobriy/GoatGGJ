using MainScripts.Spine;
using UnityEngine;

public class Fate : InteractiveObject
{
    public DialogSystem Dialog;
    public GameObject Bell;
    public SpineSkinsController SkinsController;
    public ParticleSystem ParticleSystem;
    
    public override void Interact()
    {
        Dialog.OnComplete.AddListener(() =>
        {
            ToggleInteractable(false);
            SkinsController.TryAddSkin("Bell");
            var player = FindObjectOfType<PlayerController>();
            player.HasBell = true;
            ParticleSystem.gameObject.SetActive(true);
            TalkTextController.SpawnTalkText(player.transform.position + new Vector3(-2, 2, 0), 
                "Тепер мені нічого не страшно!");
            Destroy(Bell.gameObject);
        });
        Dialog.Activate();
    }
}
