using Pixelplacement;
using UnityEngine;

public class Hide : MonoBehaviour
{
    private bool _revealed;
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private GameObject _cage;
    
    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && !_revealed)
        {
            Reveal();
        }
    }

    public void Reveal()
    {
        _revealed = true;
        _cage.gameObject.SetActive(true);
        Tween.Value(1f, 0f, value =>
        {
            _spriteRenderer.color = new Color(0.05f, 0.05f, 0.05f, value);
        }, 1f, 0f, Tween.EaseInOut);
    }
}
