using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDoor : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint;
    
#if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneAsset;
#endif
    [SerializeField] private string sceneName;
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (sceneAsset != null)
        {
            sceneName = sceneAsset.name;
        }
    }
#endif
    
    public int TargetDoorIndex;
    public int DoorIndex;
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            SceneLoader.LoadScene(sceneName, TargetDoorIndex);
        }
    }

    public Vector2 GetSpawnPoint()
    {
        return _spawnPoint.position;
    }
}
