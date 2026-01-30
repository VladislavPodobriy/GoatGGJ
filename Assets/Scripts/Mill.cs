using UnityEngine;

public class Mill : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _wings;
    private bool _broken = true;
    private float _speed = 1;
    
    void Start()
    {
        
    }
    
    private void FixedUpdate()
    {
        if (_broken)
        {
            if (_wings.transform.eulerAngles.z > 10 && _wings.transform.eulerAngles.z < 180 && _speed > 0)
            {
                _speed = -0.5f;
                _wings.angularVelocity = 0f;
            }
            else if (_wings.transform.eulerAngles.z > 180 && _wings.transform.eulerAngles.z < 360 && _speed < 0)
            {
                _speed = 1;
            }
        }
        _wings.AddTorque(_speed * 0.5f);
    }
}
