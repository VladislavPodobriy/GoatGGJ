using UnityEngine;

public class Water : MonoBehaviour
{
    public Transform spawnPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.GetComponent<PlayerController>().GetDamage();
            collision.transform.position = spawnPoint.position;
        }
    }
}
