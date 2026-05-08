using Pixelplacement;
using UnityEngine;

public class MavkaBird : MonoBehaviour
{
    [SerializeField] private float _percent;
    [SerializeField] private Spline _spline;
    [SerializeField] private float _speed;
    [SerializeField] private float _rotationSpeed;
    void Update()
    {
        _percent += Time.deltaTime * _speed;
        if (_percent > 100)
            _percent = 0;
        var position = _spline.GetPosition(_percent, false);
        if (position.x < transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else 
            transform.localScale = new Vector3(-1, 1, 1);
        
        transform.position = position;
        _spline.transform.eulerAngles = new Vector3(0, 0, _spline.transform.eulerAngles.z + _rotationSpeed * Time.deltaTime);
    }
}
