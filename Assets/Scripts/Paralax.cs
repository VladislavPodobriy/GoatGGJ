using Cinemachine;
using UnityEngine;

public class Paralax : MonoBehaviour
{
    [Range(-1, 5f), SerializeField]
    private float _factor = 0;

    private Vector3 _startPos;
    private Transform _camTransform;
    
    private void Start()
    {
        _startPos = transform.position;
        _camTransform = FindObjectOfType<CinemachineVirtualCamera>().transform;
    }

    private void Update()
    {
        var distance = _camTransform.position.x - _startPos.x;
        transform.position = new Vector3(_startPos.x + (distance * _factor), _startPos.y, 0);
    }
}
