using UnityEngine;
using UnityEngine.Events;

public class Root : MonoBehaviour
{
    private HitBox _hitBox;
    public UnityEvent OnDead;
    public GameObject ConnectedRoot;
    public Bird Bird;
    
    public void Start()
    {
        _hitBox = GetComponentInChildren<HitBox>();
        _hitBox.OnHit.AddListener((x) =>
        {
            if (x == HitType.Staff)
            {
                OnDead.Invoke();
                if (ConnectedRoot != null)
                    Destroy(ConnectedRoot.gameObject);
                if (Bird != null)
                    Bird.ToggleInteractable(true);
                Destroy(gameObject);
            }
        });
    }
}
