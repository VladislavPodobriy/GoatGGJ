using Cinemachine;
using UnityEngine;

public class Paralax : MonoBehaviour
{
    [Range(-1, 5f), SerializeField]
    private float _factor = 0;

    private Vector3 _startPos;
    private Transform _virtualCamTransform;

    private void Start()
    {
        _startPos = transform.position;
        _virtualCamTransform = FindObjectOfType<CinemachineVirtualCamera>().transform;
    }

    private void Update()
    {
        if (IsCamMoving())
        {
            var distance = _virtualCamTransform.position.x - _startPos.x;
            transform.position = new Vector3(_startPos.x + (distance * _factor), _startPos.y, 0);
        }

    }

    private bool IsCamMoving()
    {
        CinemachineConfiner confiner = _virtualCamTransform.GetComponent<CinemachineConfiner>();
        CinemachineVirtualCamera cam = _virtualCamTransform.GetComponent<CinemachineVirtualCamera>();

        return confiner.GetCameraDisplacementDistance(cam) <= 0.01f;
    }
}