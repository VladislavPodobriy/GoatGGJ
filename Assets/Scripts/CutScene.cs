using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutScene : MonoBehaviour
{
    public List<Transform> Scenes;
    public Button Skip;
    private int currentSceneIndex = -1;
    
    [SerializeField]
    private int nextSceneIndex;
    
    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ShowNextScene();
        }
    }
    
    public void Start()
    {
        if (Skip != null)
        {
            Skip.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(nextSceneIndex);
            });
        }
        ShowNextScene();
    }
    
    public void ShowNextScene()
    {
        if (currentSceneIndex == Scenes.Count - 1)
        {
            SceneManager.LoadScene(nextSceneIndex);
            return;
        }
        if (currentSceneIndex >= 0)
            Scenes[currentSceneIndex].gameObject.SetActive(false);
        currentSceneIndex++;
        Scenes[currentSceneIndex].gameObject.SetActive(true);
    }
}
