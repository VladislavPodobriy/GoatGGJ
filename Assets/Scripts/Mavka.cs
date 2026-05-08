using System.Collections.Generic;
using MainScripts.Spine;
using UnityEngine;
using UnityEngine.UI;

public class Mavka : InteractiveObject
{
    private SpineAnimationController _anim;
    [SerializeField] private DialogSystem _sadDialog;
    [SerializeField] private DialogSystem _midDialog;
    [SerializeField] private DialogSystem _happyDialog;
    [SerializeField] private List<MavkaBird> _birds;
    private int _birdIndex = -1;
    [SerializeField] private List<Image> _images;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private GameObject _tutorial;
    [SerializeField] private AudioSource _birdsSound;
    [SerializeField] private AudioSource _crySound;
    
    private void Start()
    {
        _anim = GetComponentInChildren<SpineAnimationController>();
        _anim.CreateAnimationState("Idle", true);
    }
    
    public override void Interact()
    {
        if (_birdIndex == -1)
            _sadDialog.Activate();
        else if (_birdIndex < 2)
            _midDialog.Activate();
        else
        {
            _happyDialog.OnComplete.AddListener(() =>
            {
                FindObjectOfType<PlayerController>().PowerFlute = true;
                if(!Env.Instance.endlessHeal)
                    FindObjectOfType<PlayerController>().SetHeal(3);
                _tutorial.SetActive(true);
                foreach (var image in _images)
                {
                    image.sprite = _sprite;
                }
                ToggleInteractable(false);
            });
            _happyDialog.Activate();
        }
    }

    public void AddBird()
    {
        _birdIndex++;
        _birds[_birdIndex].gameObject.SetActive(true);
        if (_birdIndex == 2)
        {
            _anim.PlayAnimation("Idle");
            _birdsSound.gameObject.SetActive(true);
            _crySound.gameObject.SetActive(false);
        }
    }
}
