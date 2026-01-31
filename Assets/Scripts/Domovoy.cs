using System.Collections;
using MainScripts.Spine;
using UnityEngine;

public class Domovoy : MonoBehaviour
{
    [SerializeField]
    private float _rollSpeed;
    
    [SerializeField]
    private float _jumpSpeed;
    
    private SpineAnimationController _anim;
    private SpineSkinsController _skins;
    private bool _isRolling;
    private bool _isJumping;
    private int _faceDirection = 1;
    private Rigidbody2D _rb;
    private float _startScale;
    private PlayerController _player;
    private Coroutine _stateRoutine;
    
    [SerializeField] 
    private HitBox _standHitBox;
    
    [SerializeField] 
    private int _hp = 10;
    
    void Start()
    {
        _anim = GetComponentInChildren<SpineAnimationController>();
        _skins = GetComponentInChildren<SpineSkinsController>();
        _rb = GetComponent<Rigidbody2D>();
        _player = FindObjectOfType<PlayerController>();
        
        _anim.CreateAnimationState("RollIdle", true);
        
        _anim.CreateAnimationState("Roll", false)
            .AddTransitionOnComplete("RollIdle");
        
        _anim.CreateAnimationState("Idle", true);
        
        _anim.CreateAnimationState("Jump", false)
            .AddTransitionOnComplete("Idle");
        
        _anim.CreateAnimationState("Kick", false)
            .AddTransitionOnComplete("Idle");

        _anim.CreateAnimationState("Damage", false);
        
        _anim.OnAnimationEvent.AddListener(x =>
        {
            if (x.EventData.Data.Name == "rollstart")
                _rb.velocity = new Vector2(-_faceDirection * _rollSpeed, 0);
            else if (x.EventData.Data.Name == "rollend")
                _rb.velocity = Vector2.zero;
            else if (x.EventData.Data.Name == "jumpstart")
                _rb.velocity = new Vector2(-_faceDirection * _jumpSpeed, 0);
            else if (x.EventData.Data.Name == "jumpend")
                _rb.velocity = Vector2.zero;
        });
        
        _anim.OnAnimationComplete.AddListener(x =>
        {
            if (x.StateName == "Roll")
                _isRolling = false;
            else if (x.StateName == "Jump" || x.StateName == "Kick")
                _isJumping = false;
        });
        
        _standHitBox.OnHit.AddListener((x) =>
        {
            _anim.PlayAnimation("Damage", 1);
            _hp -= 1;
            if (_hp == 7)
            {
                StopCoroutine(_stateRoutine);
                _skins.TryAddSkin("Legs");
                _isRolling = false;
                _rb.velocity = Vector2.zero;
                _stateRoutine = StartCoroutine(StandStateRoutine());
            }
        });
        
        _startScale = _anim.transform.localScale.x;
        SetFaceDirection(1);
        
        _stateRoutine = StartCoroutine(RollStateRoutine());
    }

    private IEnumerator StandStateRoutine()
    {
        while (true)
        {
            _isJumping = true;
            if (Vector2.Distance(transform.position, _player.transform.position) < 3 && Random.Range(0, 100) < 50)
                _anim.PlayAnimation("Kick");
            else
                _anim.PlayAnimation("Jump");
            
            yield return new WaitUntil(() => !_isJumping);
            
            var timer = 0f;
            var timeout = Random.Range(0f, 1f);
            while (timer < timeout)
            {
                SetFaceToPlayer();
                timer += Time.deltaTime;
                yield return null;
            }
        }
    }
    
    private IEnumerator RollStateRoutine()
    {
        while (true)
        {
            _isRolling = true;
            _anim.PlayAnimation("Roll");
            
            while (_isRolling)
            {
                var hit = Physics2D.Raycast(transform.position, new Vector2(-_faceDirection, 0), 1, LayerMask.GetMask("Wall"));
                
                if (hit.collider !=null)
                {
                    if (hit.point.x < transform.position.x)
                        SetFaceDirection(-1);
                    else 
                        SetFaceDirection(1);
                    _rb.velocity = new Vector2(-_faceDirection * _rollSpeed, 0);
                    
                }
                yield return null;
            }

            var timer = 0f;
            var timeout = Random.Range(1f, 3f);
            while (timer < timeout)
            {
                SetFaceToPlayer();
                timer += Time.deltaTime;
                yield return null;
            }
        }
    }

    private void SetFaceToPlayer()
    {
        if (_player.transform.position.x < transform.position.x)
            SetFaceDirection(1);
        else
            SetFaceDirection(-1);
    }

    private void SetFaceDirection(int direction)
    {
        _faceDirection = direction;
        var scale = _anim.transform.localScale;
        _anim.transform.localScale = new Vector3(_startScale * _faceDirection, scale.y, scale.z);
    }
}
