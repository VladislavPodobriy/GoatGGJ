using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hiter : MonoBehaviour
{
    public HitType HitType;
    private List<HitBox> _hitBoxes;
    public UnityEvent OnHit;
    
    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        throw new NotImplementedException();
    }

    private void Start()
    {
        _hitBoxes = new List<HitBox>();
    }

    public void Toggle(bool value)
    {
        gameObject.SetActive(value);
        if (!value)
            _hitBoxes = new List<HitBox>();
    }
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "HitBox")
        {
            var hitBox = other.GetComponent<HitBox>();
            if (!_hitBoxes.Contains(hitBox))
            {
                _hitBoxes.Add(hitBox);
                var hit = hitBox.Hit(HitType);
                if (hit)
                    OnHit.Invoke();
            }
        }
    }
}
