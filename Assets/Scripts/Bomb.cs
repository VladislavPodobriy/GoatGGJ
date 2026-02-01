using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject ExplosionPrefab;

    private void Start()
    {
        StartCoroutine(Explode());
    }
    
    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(2f);
        Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
