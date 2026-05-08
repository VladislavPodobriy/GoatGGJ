using System.Collections;
using MainScripts.Controllers;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject ExplosionPrefab;
    public AudioClip ExplosionClip;
    
    private void Start()
    {
        StartCoroutine(Explode());
    }
    
    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(2f);
        AudioController.PlayAtWorldPosition(ExplosionClip, transform.position);
        Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
