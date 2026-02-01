using System.Collections;
using MainScripts.Spine;
using UnityEngine;

public class Didko : MonoBehaviour
{
    [SerializeField] private DialogSystem _startDialog;
    [SerializeField] private DialogSystem _finalDialog;
    [SerializeField] private DamageArea _dashDamageArea;
    [SerializeField] private Transform _bombOrigin;
    [SerializeField] private Rigidbody2D _bombPrefab;
    
    [SerializeField] private float _hp;
    [SerializeField] private Vector2 _jumpVelocityRange;
    [SerializeField] private float _clawAttackDistance;
    [SerializeField] private float _dashVelocity;
    [SerializeField] private Vector2 _bombForce;
    
    private Rigidbody2D _rb;
    private SpineAnimationController _anim;
    private PlayerController _player;
    private HitBox _hitBox;

    private float _startScale;
    private int _faceDirection;
    private bool _isBusy = false;
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponentInChildren<SpineAnimationController>();
        _player = FindObjectOfType<PlayerController>();
        _hitBox = GetComponentInChildren<HitBox>();
        
        _anim.CreateAnimationState("Damage", false);
        _anim.CreateAnimationState("Idle", true);
        _anim.CreateAnimationState("DashAttack", false)
            .AddTransitionOnComplete("Idle");
        _anim.CreateAnimationState("ClawAttack", false)
            .AddTransitionOnComplete("Idle");
        _anim.CreateAnimationState("Jump", false)
            .AddTransitionOnComplete("Idle");
        _anim.CreateAnimationState("Throw", false)
            .AddTransitionOnComplete("Idle");
        
        _anim.OnAnimationComplete.AddListener(x =>
        {
            if (x.StateName == "Jump" || x.StateName == "ClawAttack" 
                                      || x.StateName == "DashAttack" 
                                      || x.StateName == "Throw")
                _isBusy = false;
        });
        
        _anim.OnAnimationEvent.AddListener(x =>
        {
            if (x.EventData.Data.Name == "jumpstart")
            {
                var velocity = Random.Range(_jumpVelocityRange.x, _jumpVelocityRange.y);
                _rb.velocity = new Vector2(velocity * _faceDirection, 0);
            }
            else if (x.EventData.Data.Name == "jumpend")
            {
                _rb.velocity = Vector2.zero;
            }
            else if (x.EventData.Data.Name == "dashstart")
            {
                _rb.velocity = new Vector2(_dashVelocity * _faceDirection, 0);
                _dashDamageArea.gameObject.SetActive(true);
            }
            else if (x.EventData.Data.Name == "dashend")
            {
                _rb.velocity = Vector2.zero;
                _dashDamageArea.gameObject.SetActive(false);
            }
            else if (x.EventData.Data.Name == "clawhit")
            {
                if (Mathf.Abs(transform.position.x - _player.transform.position.x) < _clawAttackDistance)
                {
                    _player.GetDamage();
                }
            }
            else if (x.EventData.Data.Name == "throw")
            {
                var rb = Instantiate(_bombPrefab, _bombOrigin.position, Quaternion.identity);
                rb.AddForce(new Vector2(_bombForce.x * _faceDirection, _bombForce.y), ForceMode2D.Impulse);
                rb.AddTorque(1, ForceMode2D.Impulse);
            }
        });
        
        _hitBox.OnHit.AddListener(x =>
        {
            if (x == HitType.Fear)
                return;
            _hp--;
            if (_hp <= 0)
            {
                StopAllCoroutines();
                _rb.velocity = Vector2.zero;
                _anim.PlayAnimation("Idle");
                _finalDialog.Activate();
            }
            else
            {
                _anim.PlayAnimation("Damage", 1);
            }
        });
        
        _startDialog.OnComplete.AddListener(() =>
        {
            StartCoroutine(StateRoutine());
        });

        _finalDialog.OnComplete.AddListener(() =>
        {
            Destroy(gameObject);
        });
        
        _startScale = _anim.transform.localScale.x;
        
        _startDialog.Activate();
    }

    private IEnumerator StateRoutine()
    {
        while (true)
        {
            var jumpsCount = Random.Range(1, 4);
            for (var i = 0; i < jumpsCount; i++)
            {
                int direction;
                if (transform.position.x < -6)
                    direction = 1;
                else if (transform.position.x > 6)
                    direction = -1;
                else 
                    direction = Random.Range(0, 100) > 50 ? -1 : 1; 
                
                SetFaceDirection(direction);
                _isBusy = true;
                _anim.PlayAnimation("Jump");
                yield return new WaitUntil(() => !_isBusy);
                yield return new WaitForSeconds(Random.Range(0f, 1f));

                if (Mathf.Abs(transform.position.x - _player.transform.position.x) < _clawAttackDistance)
                {
                    SetFaceToPlayer();
                    _anim.PlayAnimation("ClawAttack");
                    _isBusy = true;
                    yield return new WaitUntil(() => !_isBusy);
                }
            }

            if (Random.Range(0, 100) < 0)
            {
                SetFaceToPlayer();
                _isBusy = true;
                _anim.PlayAnimation("DashAttack");
                yield return new WaitUntil(() => !_isBusy);
            }
            else
            {
                SetFaceToPlayer();
                _isBusy = true;
                _anim.PlayAnimation("Throw");
                yield return new WaitUntil(() => !_isBusy);
            }
        }
    }
    
    private void SetFaceToPlayer()
    {
        if (_player.transform.position.x < transform.position.x)
            SetFaceDirection(-1);
        else
            SetFaceDirection(1);
    }

    private void SetFaceDirection(int direction)
    {
        _faceDirection = direction;
        var scale = _anim.transform.localScale;
        _anim.transform.localScale = new Vector3(_startScale * -_faceDirection, scale.y, scale.z);
    }
}
