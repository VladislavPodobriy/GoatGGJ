using UnityEngine;

public abstract class InteractiveObject : MonoBehaviour
{
    public string Tip;
        
    public abstract void Interact();

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            var player = col.GetComponent<PlayerController>();
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
}
