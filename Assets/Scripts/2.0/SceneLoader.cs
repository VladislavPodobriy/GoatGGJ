using System.Linq;
using Pixelplacement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    public static void LoadScene(string sceneName, int targetDoorIndex)
    {
        var op = SceneManager.LoadSceneAsync(sceneName);
        op.completed += (a) =>
        {
            var door = FindObjectsOfType<SceneDoor>()
                .FirstOrDefault(x => x.DoorIndex == targetDoorIndex);
            if (door == null)
            {
                Debug.LogError($"No door with index {targetDoorIndex} found");
                return;
            }
            
            var goat = FindObjectOfType<PlayerController>();
            if (goat == null)
            {
                Debug.LogError($"No player controller found");
                return;
            }
            
            goat.transform.position = door.GetSpawnPoint();
        };
    }
}
