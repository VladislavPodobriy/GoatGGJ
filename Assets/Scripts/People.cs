using System.Collections;
using System.Collections.Generic;
using MainScripts.Spine;
using UnityEngine;
using Random = UnityEngine.Random;

public class People : InteractiveObject
{
    private List<string> lines = new List<string>
    {
        "Коляд-коляд колядниця, добра з медом паляниця!",
        "Коляда іде, всім дари везе!",
        "Коляд-коляд-коляда, хай обходить вас біда!",
        "Святий вечір, а я йду. Сидить заєць на льоду...",
        "Коляд-коляд-колядин, я у батька один..."
    };

    public DialogSystem _noHat;
    public DialogSystem _hat;
    public Item _bucket;
    public SpineSkinsController SkinsController;
    private void Start()
    {
        StartCoroutine(TalkRoutine());
    }

    private IEnumerator TalkRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5, 8f));
            var index = Random.Range(0, lines.Count);
            TalkTextController.SpawnTalkText(transform.position + new Vector3(0, 6, 0), lines[index]);
        }
    }

    public override void Interact()
    {
        var player = FindObjectOfType<InventoryManager>();
        if (player.HasItem("Шапка"))
        {
            _hat.Activate();
            _hat.OnComplete.AddListener(() =>
            {
                ToggleInteractable(false);
                _bucket.ToggleInteractable(true);
                player.Remove("Шапка");
                SkinsController.TryAddSkin("hat");
            });
        }
        else
        {
            _noHat.Activate();
        }
    }
}
