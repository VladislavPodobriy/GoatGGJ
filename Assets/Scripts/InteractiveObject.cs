using UnityEngine;

public abstract class InteractiveObject : MonoBehaviour
{
    public string Tip;
    public string Tip_Ua;
    public string Tip_En;
    string aaa = "this is tip!";
    public Collider2D Collider;
    public bool IsInteractable = true;
    protected void Awake()
    {
        Tip = Language.Instance.language == Language.Instance.ukrainian ? Tip_Ua : Tip_En;
        print("Set Language At Awake To: " + Language.Instance.language + " FOR " + aaa);
    }

    public abstract void Interact();
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            var player = col.GetComponent<PlayerController>();
            if (IsInteractable)
                player.AddInteractiveObject(this);
        }
    }
    
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            var player = col.GetComponent<PlayerController>();
            player.RemoveInteractiveObject(this);
        }
    }
    
    public void ToggleInteractable(bool value)
    {
        Collider.enabled = value;
        IsInteractable = value;
    }
}
