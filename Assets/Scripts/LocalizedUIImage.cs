using UnityEngine;
using UnityEngine.UI;

public class LocalizedUIImage : MonoBehaviour
{
    private Image image;
    [SerializeField] private Sprite ua;
    [SerializeField] private Sprite en;
    
    void Start()
    {
        Env.Instance.OnLanguageChanged.AddListener(UpdateImage);
        UpdateImage();   
    }

    public void UpdateImage()
    {
        image = GetComponent<Image>();
        if (Env.Instance.language == Env.Instance.ukrainian)
            image.sprite = ua;
        else
            image.sprite = en;
    }
}
