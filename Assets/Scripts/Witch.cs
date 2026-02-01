using System.Collections;
using MainScripts.Spine;
using Pixelplacement;
using UnityEngine;

public class Witch : MonoBehaviour
{
    private SpineAnimationController _anim;
    private PlayerController _player;
    private Rigidbody2D _rb;
    private HitBox _hitBox;
    private DamageArea _damageArea;
    
    [SerializeField] private InteractiveObject _well;
    [SerializeField] private DialogSystem _finalDialog;
    [SerializeField] private DialogSystem _startDialog;
    [SerializeField] private GameObject _roots;
    [SerializeField] private Trigger _initialTrigger;
    
    [SerializeField] private Spline _spline;
    [SerializeField] private Transform _particleSystem;
    
    [SerializeField] private int _health = 10;
    [SerializeField] private float _playerOffsetX;
    [SerializeField] private float _flyY;
    [SerializeField] private float _flyForce;
    [SerializeField] private float _maxFlyVelocity;
    [SerializeField] private float _forceMultiplier;
    [SerializeField] private AnimationCurve _flyForceCurve;
    [SerializeField] private float _maxForceDistance;
    [SerializeField] private float _attackDistance;
    [SerializeField] private float _attackDuration;
    
    [SerializeField] private AnimationCurve _attackYCurve;
    [SerializeField] private Vector2 _attackTimeoutRange;
    [SerializeField] private Vector2 _suckTimeoutRange;
    
    private int _faceDirection = 1;
    private float _startScale;
    private bool _isFlying;
    private float _attackTimer;
    private float _attackTimeout;
    private float _suckTimer;
    private float _suckTimeout;
    
    private bool _isSuccess;
    private bool _active;
    
    private void Start()
    {
        _anim = GetComponentInChildren<SpineAnimationController>();
        _player = FindObjectOfType<PlayerController>();
        _rb = GetComponent<Rigidbody2D>();
        _hitBox = GetComponentInChildren<HitBox>();
        _damageArea = GetComponentInChildren<DamageArea>(true);
        
        _anim.CreateAnimationState("Idle", true);
        _anim.CreateAnimationState("FlyIdle", true);
        _anim.CreateAnimationState("FlyAttack", false)
            .AddTransitionOnComplete("FlyIdle");
        _anim.CreateAnimationState("Fly", true);
        _anim.CreateAnimationState("FlySuck", true);
        _anim.CreateAnimationState("Damage", false);
        
        _hitBox.OnHit.AddListener((x) =>
        {
            if (!_active)
                return;
            if (x == HitType.Fear)
                return;
            _health -= 1;
            if (_health == 0)
            {
                StopAllCoroutines();
                _particleSystem.gameObject.SetActive(false);
                _anim.PlayAnimation("FlyIdle");
                _rb.velocity = Vector2.zero;
                _finalDialog.Activate();
                _finalDialog.OnComplete.AddListener(() =>
                {
                    _well.ToggleInteractable(true);
                    _roots.SetActive(false);
                    Destroy(gameObject);
                });
            }
            else
                _anim.PlayAnimation("Damage", 1);
        });
        
        _initialTrigger.OnTriggerEnter.AddListener(() =>
        {
            _active = true;
            _initialTrigger.gameObject.SetActive(false);
            _startDialog.Activate();
        });
        
        _startScale = _anim.transform.localScale.x;
        _attackTimeout = Random.Range(_attackTimeoutRange.x, _attackTimeoutRange.y);
        _suckTimeout = Random.Range(_suckTimeoutRange.x, _suckTimeoutRange.y);
        SetFaceToPlayer();
    }
    
    private void Update()
    {
        if (_active)
        {
            _attackTimer += Time.deltaTime;
            _suckTimer += Time.deltaTime;
        
            var playerPos = _player.transform.position + new Vector3(0, 0.5f, 0);
            var pos = transform.position + new Vector3(0, 1, 0);
        
            _particleSystem.position = playerPos;
        
            _spline.Anchors[1].Anchor.position = playerPos;
            _spline.Anchors[1].InTangent.position = playerPos + (pos - playerPos).normalized + new Vector3(0, 1f, 0);
            _spline.Anchors[0].Anchor.position = pos;
            _spline.Anchors[0].OutTangent.position = pos + (playerPos - pos).normalized + new Vector3(0, 1f, 0);;
            _spline.Anchors[1].Changed = true;
            _spline.Anchors[0].Changed = true;
        }
    }
    
    public void StartBattle()
    {
        _roots.SetActive(true);
        _startDialog.ToggleInteractable(false);
        _anim.PlayAnimation("FlyIdle");
        Tween.Position(transform, new Vector3(transform.position.x, _flyY, 0), 1, 0, Tween.EaseOut, completeCallback:
            () =>
            {
                StartCoroutine(ChaseRoutine());
            });
    }
    
    private IEnumerator ChaseRoutine()
    {
        _hitBox.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        
        _anim.PlayAnimation("Fly");
        while (true)
        {
            SetFaceToPlayer();
            var playerDirection = _player.transform.position.x < transform.position.x ? -1 : 1;
            var targetX = _player.transform.position.x + (_playerOffsetX * -playerDirection);
            var moveDirection = targetX < transform.position.x ? -1 : 1;
            var distanceToTargetAbs = Mathf.Abs(targetX - transform.position.x);
            
            if (_suckTimer > _suckTimeout && distanceToTargetAbs < _attackDistance)
            {
                yield return SuckRoutine();
                _anim.PlayAnimation("Fly");
            }
            
            var distanceToTargetAbsNorm = distanceToTargetAbs / _maxForceDistance;
            var forceMultiplier = _flyForceCurve.Evaluate(distanceToTargetAbsNorm) * _forceMultiplier;
            _rb.AddForce(new Vector2((_flyForce * forceMultiplier + 2) * moveDirection, 0));
            if (Mathf.Abs(_rb.velocity.x) > _maxFlyVelocity)
            {
                _rb.velocity = new Vector2(_maxFlyVelocity * Mathf.Sign(_rb.velocity.x), 0);
            }

            if (_attackTimer > _attackTimeout)
            {
                yield return AttackRoutine();
                _anim.PlayAnimation("Fly");
            }
            
            yield return null;
        }
    }

    private IEnumerator SuckRoutine()
    {
        _rb.velocity = Vector2.zero;
        _anim.PlayAnimation("FlySuck");
        var success = true;
        _particleSystem.gameObject.SetActive(true);
        var timer = 0f;
        while (timer < _attackDuration)
        {
            SetFaceToPlayer();
            timer += Time.deltaTime;
            if (Mathf.Abs(transform.position.x - _player.transform.position.x) > 6)
            {
                success = false;
                break;
            }
            yield return null;
        }
        if (success)
            _player.GetDamage();
        _particleSystem.gameObject.SetActive(false);
        _suckTimeout = Random.Range(_suckTimeoutRange.x, _suckTimeoutRange.y);
        _suckTimer = 0;
    }
    
    private IEnumerator AttackRoutine()
    {
        _anim.PlayAnimation("FlyAttack");
        _rb.velocity = new Vector2(6, 0);
        _damageArea.gameObject.SetActive(true);
        Tween.Value(transform.position.y, _flyY - 2, value =>
        {
            transform.position = new Vector3(transform.position.x, value);
        }, 3f, 0f, _attackYCurve);
        Tween.Value(0, 6f, value =>
        {
            _rb.velocity = new Vector2(value * -_faceDirection, 0);
        }, 3f, 0f, _attackYCurve);
        yield return new WaitForSeconds(4f);
        _damageArea.gameObject.SetActive(false);
        _attackTimeout = Random.Range(_attackTimeoutRange.x, _attackTimeoutRange.y);
        _attackTimer = 0;
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
