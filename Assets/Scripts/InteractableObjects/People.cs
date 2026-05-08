using MainScripts.Spine;
using System.Collections;
using System.Collections.Generic;
using MainScripts.Audio;
using MainScripts.Controllers;
using UnityEngine;
using Random = UnityEngine.Random;

public class People : InteractiveObject
{
    private List<string> lines;

    public readonly List<string> ua = new List<string>
    {
        "Коляд-коляд колядниця, добра з медом паляниця!",
        "Коляда іде, всім дари везе!",
        "Коляд-коляд-коляда, хай обходить вас біда!",
        "Святий вечір, а я йду. Сидить заєць на льоду...",
        "Коляд-коляд-колядин, я у батька один..."
    };

    public readonly List<string> en = new List<string>
    {
        "Kolyad-kolyad, little carol, a good flatbread with honey sweet!",
        "Kolyada is coming, bringing gifts for everyone!",
        "Kolyad-kolyad-kolyada, may misfortune pass you by!",
        "Holy Evening, off I go. There sits a hare upon the ice…",
        "Kolyad-kolyad-kolyadyn, I’m my father’s only son…"
    };

    public AudioLibrary _audioLibrary;
    public DialogSystem _noHat;
    public DialogSystem _hat;
    public Item _bucket;
    public SpineSkinsController SkinsController;
    public SpineAnimationController AnimationController;
    
    private void Awake()
    {
        base.Awake();
        lines = Env.Instance.language == Env.Instance.ukrainian ? ua : en;
    }
    private void Start()
    {
        AnimationController.CreateAnimationState("idle", true);
        AnimationController.OnAnimationEvent.AddListener(x =>
        {
            if (x.EventData.Data.Name == "tambu")
            {
                AudioController.PlayAtWorldPosition(_audioLibrary.GetRandom("Tambu"), transform.position);
            }
        });
        StartCoroutine(TalkRoutine());
    }

    private IEnumerator TalkRoutine()
    {
        AnimationController.PlayAnimation("idle");
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
