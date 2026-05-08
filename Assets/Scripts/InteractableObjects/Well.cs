using UnityEngine;

public class Well : InteractiveObject
{
    public Item FullBucket;

    string text_success;
    string text_fail;

    readonly string ua_success = "Вода, водиця!";
    readonly string en_success = "Water, dear sweet water!";

    readonly string ua_fail = "Тут немає відра...";
    readonly string en_fail = "There is no bucket, here";
    
    private int _step = 0;

    [SerializeField] private Sprite _emptyBucket;
    [SerializeField] private Sprite _fullBucket;
    [SerializeField] private GameObject _cage;
    [SerializeField] private Animator _anim;
    [SerializeField] private SpriteRenderer _bucketRenderer;
    
    private void Awake()
    {
        base.Awake();
        text_success = Env.Instance.language == Env.Instance.ukrainian ? ua_success : en_success;
        text_fail = Env.Instance.language == Env.Instance.ukrainian ? ua_fail : en_fail;
        Tip_Ua = "Набрати води";
        Tip_En = "Get water";
    }

    public void OnWellDown()
    {
        if (_step == 0)
        {
            _bucketRenderer.sprite = _fullBucket;
        }
        else if (_step == 2)
        {
            _cage.SetActive(true);
        }
    }

    public void OnWellUp()
    {
        if (_step == 0)
        {
            _step = 1;
            ToggleInteractable(true);
            Tip_Ua = "Взяти повне відро";
            Tip_En = "Take full bucket";
        }
    }
    
    public override void Interact()
    {
        var player = FindObjectOfType<InventoryManager>();
        if (_step == 0)
        {
            if (player.HasItem("Відро"))
            {
                player.Remove("Відро");
                _bucketRenderer.gameObject.SetActive(true);
                _anim.Play("Well");
                ToggleInteractable(false);
            }
            else
            {
                TalkTextController.SpawnTalkText(player.transform.position + new Vector3(-2, 2, 0), text_fail);
            }
        }
        else if (_step == 1)
        {
            TalkTextController.SpawnTalkText(player.transform.position + new Vector3(-2, 2, 0), text_success);
            FindObjectOfType<InventoryManager>().Add(FullBucket);
            _bucketRenderer.gameObject.SetActive(false);
            Tip_Ua = "Дослідити";
            Tip_En = "Explore";
            _step = 2;
        }
        else if (_step == 2)
        {
            _anim.Play("Well");
            ToggleInteractable(false);
        }
    }
}
