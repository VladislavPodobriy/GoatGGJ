using System.Collections;
using UnityEngine;

public class Pich : InteractiveObject
{
    public Item Kalach;
    public Didko Didko;
    
    public override void Interact()
    {
        var player = FindObjectOfType<InventoryManager>();
        player.Remove("Борошно");
        player.Remove("ПовнеВідро");
        player.Add(Kalach);
        ToggleInteractable(false);
        TalkTextController.SpawnTalkText(player.transform.position + new Vector3(-2, 2, 0), 
            "Ну ось я і спекла Калач");
        StartCoroutine(BossWait());
    }

    private IEnumerator BossWait()
    {
        yield return new WaitForSeconds(7);
        Didko.gameObject.SetActive(true);
    }
}
