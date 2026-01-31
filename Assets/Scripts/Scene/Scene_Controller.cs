
using Cinemachine;
using UnityEngine;

public class Scene_Controller : MonoBehaviour
{
    public bool IsInitial;
    
    [Header("Target Left Level Settings")]
    [SerializeField] private Scene_Controller targetSceneLeft;
    
    [Header("Target Right Level Settings")]
    [SerializeField] private Scene_Controller targetSceneRight;
    
    [Header("Scene Camera Settings")] 
    public GameObject cameraBoundsObj;
    public Transform spawnPointLeft;
    public Transform spawnPointRight;
    public int OrthoSize = 8;
    
    [HideInInspector]
    public CinemachineConfiner2D cameraConfiner; 

    [Header("Overlay")]
    [SerializeField] GameObject overlay;

    private void Start()
    {
        cameraConfiner = FindObjectOfType<CinemachineConfiner2D>();

        if (IsInitial)
        {
            SetCameraBounds(cameraBoundsObj, cameraConfiner);
            FindObjectOfType<CinemachineVirtualCamera>().m_Lens.OrthographicSize = OrthoSize;
        }
    }

    #region SCENE_GATE_TRIGGER
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) 
            return;
        //SetOverlayState(true);
        
        if (collision.transform.position.x < 0)
        {
            TeleportToLevel(targetSceneLeft.spawnPointRight, targetSceneLeft);
        }
        else
        {
            TeleportToLevel(targetSceneRight.spawnPointLeft, targetSceneRight);
        }

        //SetOverlayState(false);
    }

    #endregion

    #region SCENE_GATE_CONTROLS

    public void TeleportToLevel(Transform targetPos, Scene_Controller targetScene)
    {
        var player = FindObjectOfType<PlayerController>();
        print("targetPos:" + targetPos.position);
        print("playerPos:" + player.transform.position);
        player.transform.position = targetPos.position;
        SetCameraBounds(targetScene.cameraBoundsObj, targetScene.cameraConfiner);
        FindObjectOfType<CinemachineVirtualCamera>().m_Lens.OrthographicSize = targetScene.OrthoSize;
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
