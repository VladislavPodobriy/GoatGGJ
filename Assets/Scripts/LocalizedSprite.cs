using UnityEngine;

public class LocalizedSprite : MonoBehaviour
{
    private SpriteRenderer image;
    [SerializeField] private Sprite ua;
    [SerializeField] private Sprite en;
    
    void Start()
    {
        Env.Instance.OnLanguageChanged.AddListener(UpdateImage);
        UpdateImage();   
    }

    public void UpdateImage()
    {
        image = GetComponent<SpriteRenderer>();
        if (Env.Instance.language == Env.Instance.ukrainian)
            image.sprite = ua;
        else
            image.sprite = en;
    }
}
