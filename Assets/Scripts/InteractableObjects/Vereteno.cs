using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Vereteno : InteractiveObject
{
    public List<LineRenderer> _lines;
    
    [SerializeField]
    private float _range;

    [SerializeField]
    private float _step;

    [SerializeField] 
    private Trigger _trigger;
    
    private PlayerController _player;
    private bool _active;
    
    private void Start()
    {
        _player = FindObjectOfType<PlayerController>();
        _trigger.OnTriggerEnter.AddListener(() =>
        {
            if (!_active)
            {
                var text = Env.Instance.language == Env.Instance.ukrainian ? 
                    "Це що, величезне веретено? Дивина..." :
                    "Is that a giant spindle? How strange…";
                TalkTextController.SpawnTalkText(_player.transform.position + new Vector3(-2, 2, 0), text);
                _active = true;
                ToggleInteractable(true);
            }
        });
        ToggleInteractable(false);
    }

    private void Update()
    {
        if (_active)
        {
            var distance = _player.transform.position.x - transform.position.x;
            var linesCount = 0f;
            if (Mathf.Abs(distance) > _range)
                linesCount = Mathf.Min((Mathf.Abs(distance) - _range) / _step, _lines.Count);
            for (int i = 0; i < _lines.Count; i++)
            {
                if (i < linesCount)
                {
                    _lines[i].gameObject.SetActive(true);
                    _lines[i].SetPosition(0, _lines[i].transform.position);
                    _lines[i].SetPosition(1, _player.transform.position + new Vector3(0, 1.5f - (i * 0.1f)));
                }
                else
                {
                    _lines[i].gameObject.SetActive(false);
                }
            }
            _player.SlowFactor = Mathf.RoundToInt(Mathf.Sign(_player.FaceDirection)) == Mathf.RoundToInt(Mathf.Sign(distance)) ? 
                linesCount / _lines.Count : 0f;
        }
        
    }
    
    public override void Interact()
    {
        var unfate = FindObjectOfType<Unfate>(true);
        unfate.gameObject.SetActive(true);
        ToggleInteractable(false);
        _player.LongAttackAllowed = false;
    }
}
