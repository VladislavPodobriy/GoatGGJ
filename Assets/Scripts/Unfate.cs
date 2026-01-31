using System.Collections;
using MainScripts.Spine;
using Pixelplacement;
using UnityEngine;
using UnityEngine.Serialization;

public class Unfate : MonoBehaviour
{
    private SpineAnimationController _anim;
    private bool _isBusy;
    private int _faceDirection = 1;
    private float _startScale;
    private PlayerController _player;
    private HitBox _hitBox;
    [SerializeField] private int _health;
    
    [SerializeField] private Rigidbody2D FateBullet;
    
    [SerializeField] private Transform _ultimatePosition;
    [SerializeField] private Transform _startPosition;
    [FormerlySerializedAs("_shootPosition")] [SerializeField] private Transform _shootOrigin;
    
    [SerializeField] private float _ultimateTimeout;
    [SerializeField] private float _ultimateY;
    [SerializeField] private int _bulletCount;
    [SerializeField] private Vector2 _bulletForce;
    
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    
    private float _ultimateTimer;
    private int _bulletIndex;
    private int _moveDirection = 1;

    [SerializeField] private GameObject _wheat;
    [SerializeField] private GameObject _fate;
    [SerializeField] private GameObject _ropes;
    
    private void Start()
    {
        _player = FindObjectOfType<PlayerController>();
        _anim = GetComponentInChildren<SpineAnimationController>();
        _hitBox = GetComponentInChildren<HitBox>();
        
        _anim.CreateAnimationState("Float", true);

        _anim.CreateAnimationState("RangeAttack", false)
            .AddTransitionOnComplete("Float");
        
        _anim.CreateAnimationState("MeleeAttack", false)
            .AddTransitionOnComplete("Float");

        _anim.CreateAnimationState("Damage", false);
        
        _anim.OnAnimationComplete.AddListener(x =>
        {
            if (x.StateName == "RangeAttack" || x.StateName == "MeleeAttack")
                _isBusy = false;
        });
        
        _anim.OnAnimationEvent.AddListener(x =>
        {
            if (x.EventData.Data.Name == "shoot")
            {
                var bullet = Instantiate(FateBullet, _shootOrigin.position, Quaternion.identity);
                bullet.transform.eulerAngles = new Vector3(0, 0, 145  * _faceDirection);
                bullet.AddForce(-bullet.transform.up * (Random.Range(_bulletForce.x, _bulletForce.y) + (_bulletIndex * 3)), ForceMode2D.Impulse);
                _bulletIndex++;
            }
            else if (x.EventData.Data.Name == "hit")
            {
                
            }
        });
        
        _hitBox.OnHit.AddListener((x) =>
        {
            _anim.PlayAnimation("Damage", 1);
            _health--;
            if (_health == 0)
            {
                _fate.SetActive(true);
                _wheat.SetActive(true);
                _ropes.SetActive(false);
                _player.SlowFactor = 0;
                _player.LongAttackAllowed = true;
                Destroy(gameObject);
            }
        });
        
        _startScale = _anim.transform.localScale.x;
        SetFaceDirection(1);

        _anim.PlayAnimation("Float");
        
        StartCoroutine(MainStateRoutine());
    }

    private void Update()
    {
        _ultimateTimer += Time.deltaTime;
    }
    
    private IEnumerator MainStateRoutine()
    {
        yield return new WaitForSeconds(3f);
        while (true)
        {
            var timer = 0f;
            var timeout = Random.Range(1f,3f);
            var attacked = false;
            while (timer < timeout)
            {
                var distance = Mathf.Abs(_player.transform.position.x - transform.position.x);
                SetFaceToPlayer();
                if (distance > 2 && distance < 4 && !attacked)
                {
                    _anim.PlayAnimation("MeleeAttack");
                    attacked = true;
                    _isBusy = true;
                    yield return new WaitUntil(() => !_isBusy);
                }
                transform.position += new Vector3(2 * Time.deltaTime * _moveDirection, 0, 0);
                if (transform.position.x < minX && _moveDirection == -1)
                    _moveDirection = 1;
                else if (transform.position.x > maxX && _moveDirection == 1)
                    _moveDirection = -1;
                timer += Time.deltaTime;
                yield return null;
            }
            
            SetFaceToPlayer();
            if (_ultimateTimer >= _ultimateTimeout)
            {
                _hitBox.gameObject.SetActive(false);
                bool onPosition = false;
                Tween.Position(_anim.transform, transform.position + new Vector3(0, 4, 0), 2f, 0f, Tween.EaseInOut, completeCallback: () =>
                {
                    onPosition = true;
                });
                yield return new WaitUntil(() => onPosition);
                for (int i = 0; i < _bulletCount; i++)
                {
                    var x = _player.transform.position.x +  Random.Range(-1f, 1f);
                    Instantiate(FateBullet, new Vector3(x, _ultimateY, 0), Quaternion.identity);
                    yield return new WaitForSeconds(Random.Range(0.3f, 0.6f));
                }
                onPosition = false;
                Tween.Position(_anim.transform, transform.position, 2f, 0f, Tween.EaseInOut, completeCallback: () =>
                {
                    onPosition = true;
                });
                yield return new WaitUntil(() => onPosition);
                _ultimateTimer = 0;
                _hitBox.gameObject.SetActive(true);
            }
            else
            {
                _anim.PlayAnimation("RangeAttack");
                _isBusy = true;
                _bulletIndex = 0;
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
        _anim.transform.localScale = new Vector3(_startScale * _faceDirection, scale.y, scale.z);
    }
}
