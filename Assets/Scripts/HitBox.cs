using UnityEngine;
using UnityEngine.Events;

public class HitBox : MonoBehaviour
{
    public UnityEvent OnHit;
    
    public void Hit()
    {
        OnHit?.Invoke();
    }
}
