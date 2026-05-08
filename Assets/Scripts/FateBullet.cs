using System.Collections;
using MainScripts.Audio;
using MainScripts.Controllers;
using UnityEngine;

public class FateBullet : MonoBehaviour
{
    private Rigidbody2D _rb;
    [SerializeField] private AudioLibrary _audioLibrary;
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        AudioController.PlayAtWorldPosition(_audioLibrary.GetRandom("Spawn"), transform.position);
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
        AudioController.PlayAtWorldPosition(_audioLibrary.GetRandom("Hit"), transform.position);
        _rb.bodyType = RigidbodyType2D.Static;
        GetComponentInChildren<DamageArea>().gameObject.SetActive(false);
        StartCoroutine(DestroyAfterTime(2f));
    }

    private IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
