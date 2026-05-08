using MainScripts.Spine;
using Pixelplacement;
using UnityEngine;

public class Bird : MonoBehaviour
{
    string text;
    private SpineAnimationController _anim;
    readonly string ua = "Лети, маленька пташко";
    readonly string en = "Fly, little bird";
    public int dir = 1;
    public AudioSource Sound;
    
    private void Awake()
    {
        text = Env.Instance.language == Env.Instance.ukrainian ? ua : en;
        _anim = GetComponentInChildren<SpineAnimationController>();
    }

    private void Start()
    {
        _anim.CreateAnimationState("Fly", true);
    }

    public void Fly()
    {
        var player = FindObjectOfType<PlayerController>();
        TalkTextController.SpawnTalkText(player.transform.position + new Vector3(-2, 2, 0), text);
        _anim.PlayAnimation("Fly");
        Tween.Position(transform, 
            transform.position + new Vector3(-30, 30, 0), 
            3, 0, Tween.EaseInOut, completeCallback: () =>
            {
                FindObjectOfType<Mavka>().AddBird();
                Destroy(gameObject);
            });
        transform.localScale = new Vector3(dir, 1, 1);
        if (Sound != null)
            Destroy(Sound.gameObject);
    }
}
