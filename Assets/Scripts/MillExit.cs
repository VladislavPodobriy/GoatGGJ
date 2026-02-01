using UnityEngine;

public class MillExit : MonoBehaviour
{
    public Scene_Controller targetScene;
    public Transform targetPoint;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        { 
            targetScene.TeleportToLevel(targetPoint, targetScene);
        }
    }
}
