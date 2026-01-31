using UnityEngine;

public class Root : MonoBehaviour
{
    private HitBox _hitBox;

    public void Start()
    {
        _hitBox = GetComponentInChildren<HitBox>();
        _hitBox.OnHit.AddListener((x) =>
        {
            if (x == HitType.Staff)
            {
                Destroy(gameObject);
            }
        });
    }
}
