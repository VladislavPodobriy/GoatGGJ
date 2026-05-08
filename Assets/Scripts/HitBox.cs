using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum HitType
{
    Horn = 1,
    Staff = 2,
    Fear = 3 
}

public class HitBox : MonoBehaviour
{
    public UnityEvent<HitType> OnHit;
    public List<HitType> IgnoredHitTypes = new();
    
    private void Start()
    {
        gameObject.tag = "HitBox";
    }
    
    public bool Hit(HitType hitType)
    {
        if (IgnoredHitTypes.Contains(hitType))
            return false;
        OnHit?.Invoke(hitType);
        return true;
    }
}
