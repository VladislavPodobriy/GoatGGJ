using System.Collections.Generic;
using UnityEngine;

public class Hiter : MonoBehaviour
{
    public HitType HitType;
    private List<HitBox> _hitBoxes;

    private void Start()
    {
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
                hitBox.Hit(HitType);
            }
        }
    }
}
