using System.Collections;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float Time;
    
    private void Start()
    {
        StartCoroutine(DestroyAfterTimeCoroutine());
    }

    private IEnumerator DestroyAfterTimeCoroutine()
    {
        yield return new WaitForSeconds(Time);
        Destroy(gameObject);
    }
}
