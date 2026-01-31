using UnityEngine;

public abstract class InteractiveObject : MonoBehaviour
{
    public string Tip;
    public Collider2D Collider;
    public bool IsInteractable = true;
    
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
