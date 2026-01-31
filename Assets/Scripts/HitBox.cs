using UnityEngine;
using UnityEngine.Events;

public enum HitType
{
    Horn = 1,
    Staff = 2
}

public class HitBox : MonoBehaviour
{
    public UnityEvent<HitType> OnHit;

    private void Start()
    {
        gameObject.tag = "HitBox";
    }
    
    public void Hit(HitType hitType)
    {
        OnHit?.Invoke(hitType);
    }
}
