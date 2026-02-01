using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Clicker : MonoBehaviour
{
    [SerializeField] private List<Sprite> _sprites;
    [SerializeField] private float _clicksPerSprite;
    [SerializeField] private Button _button;
    [SerializeField] private Image _image;
    [SerializeField] private DialogSystem _finalDialog;
    
    private int _clicks;
    private int _currentIndex;

    public void Activate()
    {
        var player = FindObjectOfType<PlayerController>();
        player.ToggleControls(false);
        gameObject.SetActive(true);
        _button.onClick.AddListener(() =>
        {
            _clicks++;
            if (_clicks >= _clicksPerSprite)
            {
                _currentIndex++;
                _clicks = 0;
                if (_currentIndex == _sprites.Count)
                {
                    player.ShowFinalDialog(_finalDialog);
                    _button.enabled = false;
                    gameObject.SetActive(false);
                    return;
                }
                _image.sprite = _sprites[_currentIndex];
            }
        });
    }
}
