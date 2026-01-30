
using Cinemachine;
using UnityEngine;

public class Scene_Controller : MonoBehaviour
{
    [Header("Target Level Settings")]
    [SerializeField] Transform spawnPoint;
    
    [Header("Target Level Camera Settings")]
    [SerializeField] CinemachineConfiner2D cameraConfiner;
    [SerializeField] GameObject cameraBoundsObj;

    [Header("Overlay")]
    [SerializeField] GameObject overlay;


    #region SCENE_GATE_TRIGGER
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        SetOverlayState(true);
        TeleportToLevel(spawnPoint, collision.gameObject);
        SetCameraBounds(cameraBoundsObj, cameraConfiner);
        SetOverlayState(false);
    }

    #endregion

    #region SCENE_GATE_CONTROLS

    private void TeleportToLevel(Transform targetPos, GameObject player)
    {
        print("targetPos:" + targetPos.position);
         print("playerPos:" + player.transform.position);
        player.transform.position = targetPos.position;
    }

    #endregion

    #region SCENE_CAMERA_CONTROLS

    private void SetCameraBounds(GameObject boundsObj, CinemachineConfiner2D confiner)
    {
        confiner.m_BoundingShape2D = boundsObj.GetComponent<PolygonCollider2D>();
    }

    #endregion

    #region OVERLAY_CONTROLS

    private void SetOverlayState(bool state)
    {
        overlay.SetActive(state);
    }

    #endregion
}
