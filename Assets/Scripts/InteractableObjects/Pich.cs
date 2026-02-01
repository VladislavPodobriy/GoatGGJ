using System.Collections;
using UnityEngine;

public class Pich : InteractiveObject
{
    public Item Kalach;
    public Didko Didko;

    string text;

    readonly string ua = "Ну ось я і спекла Калач";
    readonly string en = "There it is, I've baked Kalach bread";

    private void Awake()
    {
        base.Awake();
        text = Language.Instance.language == Language.Instance.ukrainian ? ua : en;
    }

    public override void Interact()
    {
        var player = FindObjectOfType<InventoryManager>();
        player.Remove("Борошно");
        player.Remove("ПовнеВідро");
        player.Add(Kalach);
        ToggleInteractable(false);
        TalkTextController.SpawnTalkText(player.transform.position + new Vector3(-2, 2, 0), text);
        StartCoroutine(BossWait());
    }

    private IEnumerator BossWait()
    {
        yield return new WaitForSeconds(7);
        Didko.gameObject.SetActive(true);
    }
}
