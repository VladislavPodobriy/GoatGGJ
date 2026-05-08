using UnityEngine;

public class LocalizedGameObject : MonoBehaviour
{
    [SerializeField] private GameObject ua;
    [SerializeField] private GameObject en;
    
    void Start()
    {
        Env.Instance.OnLanguageChanged.AddListener(UpdateObject);
        UpdateObject();   
    }

    public void UpdateObject()
    {
        ua.SetActive(Env.Instance.language == Env.Instance.ukrainian);
        en.SetActive(Env.Instance.language == Env.Instance.english);
    }
}
