using Cinemachine;
using UnityEngine;

public class Paralax : MonoBehaviour
{
    [Range(-1, 5f), SerializeField]
    private float _factor = 0;

    private Vector3 _startPos;
    private CinemachineVirtualCamera _virtualCam;

    private void Start()
    {
        _startPos = transform.position;
        _virtualCam = FindObjectOfType<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        var distance = _virtualCam.State.CorrectedPosition.x - _startPos.x;
        transform.position = new Vector3(_startPos.x + (distance * _factor), _startPos.y, 0);
    }

    private bool IsCamMoving()
    {
        //CinemachineConfiner confiner = _virtualCamTransform.GetComponent<CinemachineConfiner>();
        //CinemachineVirtualCamera cam = _virtualCamTransform.GetComponent<CinemachineVirtualCamera>();

        //return confiner.GetCameraDisplacementDistance(cam) <= 0.01f;
        return true;
    }
}