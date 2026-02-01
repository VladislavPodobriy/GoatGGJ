using System.Collections;
using UnityEngine;

public class FateBullet : MonoBehaviour
{
    private Rigidbody2D _rb;
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        if (_rb != null && _rb.bodyType == RigidbodyType2D.Dynamic)
        {
            Vector2 velocity = _rb.velocity;
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 90);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        _rb.bodyType = RigidbodyType2D.Static;
        StartCoroutine(DestroyAfterTime(2f));
    }

    private IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        GetComponentInChildren<DamageArea>().gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
