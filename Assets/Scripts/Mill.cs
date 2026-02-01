using UnityEngine;

public class Mill : MonoBehaviour
{
    [SerializeField] private Root _root;
    [SerializeField] private Rigidbody2D _wings;
    [SerializeField] private Rigidbody2D _wheel;
    public Jorno Jorno;
    
    private bool _blocked = true;
    private float _speed = 1;
    
    void Start()
    {
        _root.OnDead.AddListener(() =>
        {
            var player = FindObjectOfType<PlayerController>();
            _blocked = false;
            Jorno.ToggleInteractable(true);
            TalkTextController.SpawnTalkText(player.transform.position + new Vector3(-2, 2, 0), 
                "Схоже тепер жорна працюватимуть");
        });
    }
    
    private void FixedUpdate()
    {
        if (_blocked)
        {
            if (_wings.transform.eulerAngles.z > 10 && _wings.transform.eulerAngles.z < 180 && _speed > 0)
            {
                _speed = -0.5f;
                _wings.angularVelocity = 0f;
            }
            else if (_wings.transform.eulerAngles.z > 180 && _wings.transform.eulerAngles.z < 360 && _speed < 0)
            {
                _speed = 1;
            }
        }
        _wings.AddTorque(_speed * 0.5f);
        _wheel.transform.eulerAngles = _wings.transform.eulerAngles;
    }
}
