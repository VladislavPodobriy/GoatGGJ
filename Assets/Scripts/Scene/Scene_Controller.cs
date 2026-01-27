using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Controller : MonoBehaviour
{
    [SerializeField] string nextScene;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("To: " + nextScene);
        ChangeScene(nextScene);
    }

    #region SCENE_GATE_CONTROLS

    private void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }


    #endregion
}
