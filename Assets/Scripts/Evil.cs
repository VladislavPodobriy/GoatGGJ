using System.Collections;
using System.Collections.Generic;
using MainScripts.Spine;
using UnityEngine;
using UnityEngine.Serialization;

public class Evil : MonoBehaviour
{
    private SpineAnimationController _anim;
    private Rigidbody2D _rb;
    private PlayerController _player;
    private Vector3 _startPos;
    private HitBox _hitBox;
    
    private int _faceDirection;
    
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runAwaySpeed;
    [SerializeField] private float agroSpeed;
    [FormerlySerializedAs("jumpSpeed")] [SerializeField] private float _jumpSpeed;
    [SerializeField] private Vector2 _jumpTimoutRange;
    [SerializeField] private float _jumpDistance;
    [SerializeField] private float _minJumpDistance;
    [SerializeField] private float _attackDistance;
    [SerializeField] private float _agroDistance;
    
    private float _jumpTimeout;
    private float _jumpTimer;
    private bool _isJumping;
    private float _startScale;
    private bool _isIdling;
    private bool _isAttacking;
    private bool _damaged;
    
    private Coroutine _stateRoutine;
    
    private void Start()
    {
        _anim = GetComponentInChildren<SpineAnimationController>();
        _rb = GetComponent<Rigidbody2D>();
        _player = FindObjectOfType<PlayerController>();
        _hitBox = GetComponentInChildren<HitBox>();
        
        _anim.CreateAnimationState("idle", true);
        _anim.CreateAnimationState("attack", false)
            .AddTransitionOnComplete("idle");
        _anim.CreateAnimationState("damage", false)
            .AddTransitionOnComplete("idle");
        _anim.CreateAnimationState("jump", false)
            .AddTransitionOnComplete("idle");
        _anim.CreateAnimationState("walk", true);
        _anim.CreateAnimationState("runAway", true);
        
        _anim.OnAnimationComplete.AddListener(x =>
        {
            if (x.StateName == "jump")
                _isJumping = false;
            if (x.StateName == "attack")
                _isAttacking = false;
        });
        
        _anim.OnAnimationEvent.AddListener(x =>
        {
            if (x.EventData.Data.Name == "jumpstart")
                _rb.velocity = new Vector2(_jumpSpeed * _faceDirection, 0);
            else if (x.EventData.Data.Name == "jumpend")
            {
                _rb.velocity = Vector2.zero;
                if (Vector2.Distance(transform.position, _player.transform.position) < _attackDistance)
                    _player.GetDamage();
            }
            else if (x.EventData.Data.Name == "hit")
            {
                if (Vector2.Distance(transform.position, _player.transform.position) < _attackDistance)
                    _player.GetDamage();
            }
        });
        
        _hitBox.OnHit.AddListener((x) =>
        {
            if (_damaged)
                return;
            _damaged = true;
            StopCoroutine(_stateRoutine);
            _stateRoutine = StartCoroutine(RunAwayStateRoutine());
        });

        minX = transform.position.x - 4;
        maxX = transform.position.x + 4;
        
        _startScale = _anim.transform.localScale.x;
        _startPos = transform.position;
        _jumpTimeout = Random.Range(_jumpTimoutRange.x, _jumpTimoutRange.y);

        _isIdling = true;
        _stateRoutine = StartCoroutine(IdlingStateRoutine());
    }

    private void Update()
    {
        if (_damaged)
            return;
        
        _jumpTimer += Time.deltaTime;
        var hit = Physics2D.Raycast(transform.position, new Vector2(_faceDirection, 0), 
            _agroDistance, LayerMask.GetMask("Player"));
        
        if (hit.collider != null && _isIdling)
        {
            _isIdling = false;
            StopCoroutine(_stateRoutine);
            _stateRoutine = StartCoroutine(ChaseStateRoutine());
        }
        else if (hit.collider == null && !_isIdling)
        {
            _isIdling = true;
            StopCoroutine(_stateRoutine);
            _stateRoutine = StartCoroutine(IdlingStateRoutine());
        }
    }

    private IEnumerator RunAwayStateRoutine()
    {
        _anim.PlayAnimation("runAway");
        while (transform.position.x > -34 && transform.position.x < 34)
        {
            _rb.velocity = new Vector2(runAwaySpeed * _faceDirection, 0);
            yield return null;
        }
        _rb.velocity = Vector2.zero;
        yield return new WaitUntil(() => Mathf.Abs(_player.transform.position.y - transform.position.y) > 15);
        transform.position = _startPos;
        _jumpTimeout = Random.Range(_jumpTimoutRange.x, _jumpTimoutRange.y);
        _damaged = false;
        _isIdling = true;
        _stateRoutine = StartCoroutine(IdlingStateRoutine());
    }
    
    private IEnumerator IdlingStateRoutine()
    {
        _anim.PlayAnimation("walk");
        while (true)
        {
            var targetX = Random.Range(minX, maxX);
            var direction = targetX < transform.position.x ? -1 : 1; 
            SetFaceDirection(direction);
            while (Mathf.Abs(transform.position.x - targetX) > 0.5)
            {
                _rb.velocity = new Vector2(walkSpeed * _faceDirection, 0);
                yield return null;
            }
        }
    }
    
    private IEnumerator ChaseStateRoutine()
    {
        var player = FindObjectOfType<PlayerController>();
        while (true)
        {
            _anim.PlayAnimation("walk");
            var targetX = player.transform.position.x;
            var distance = Mathf.Abs(transform.position.x - targetX);
            while (distance > _attackDistance)
            {
                targetX = player.transform.position.x;
                distance = Mathf.Abs(transform.position.x - targetX);
                var direction = targetX < transform.position.x ? -1 : 1; 
                SetFaceDirection(direction);
                
                if (distance < _jumpDistance && distance > _minJumpDistance && _jumpTimer > _jumpTimeout)
                {
                    _isJumping = true;
                    _anim.PlayAnimation("jump");
                    _rb.velocity = Vector2.zero;
                    
                    yield return new WaitUntil(() => !_isJumping);
                    yield return new WaitForSeconds(1f);
                    _anim.PlayAnimation("walk");
                    
                    _jumpTimeout = Random.Range(_jumpTimoutRange.x, _jumpTimoutRange.y);
                    _jumpTimer = 0;
                    continue;
                }
                
                _rb.velocity = new Vector2(agroSpeed * _faceDirection, 0);
                yield return null;
            }

            _isAttacking = true;
            _anim.PlayAnimation("attack");
            _rb.velocity = Vector2.zero;
            yield return new WaitUntil(() => !_isAttacking);
            yield return new WaitForSeconds(1f);
        }
    }

    private void SetFaceDirection(int direction)
    {
        _faceDirection = direction;
        var scale = _anim.transform.localScale;
        _anim.transform.localScale = new Vector3(_startScale * -_faceDirection, scale.y, scale.z);
    }
}
