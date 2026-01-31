using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private List<Image> _images;
    
    [SerializeField]
    private List<Image> _heal;
    
    public void UpdateHealth(int count)
    {
        for (int i = 0; i < _images.Count; i++)
        {
            _images[i].gameObject.SetActive(i < count);
        }
    }

    public void UpdateHeal(int count)
    {
        for (int i = 0; i < _heal.Count; i++)
        {
            _heal[i].gameObject.SetActive(i < count);
        }
    }
}
