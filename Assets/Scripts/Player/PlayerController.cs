using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MainScripts.Spine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private int _health;
    [SerializeField] private int _heal;
    [SerializeField] private int _birds;
    [SerializeField] float speed;
    [SerializeField] float jumpingPower;
    [SerializeField] float dashPower;
    [SerializeField] private GameObject menu;
    [SerializeField] private Transform goatPlace;
    [SerializeField] private Transform didkoPlace;
    
    [Header("Grounding")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheck;
    
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private bool _skipSpawn;

    [SerializeField] private Collider2D _hornAttackArea;
    [SerializeField] private Collider2D _fearArea;
    [SerializeField] private Transform _staffParticlesOrigin;
    [SerializeField] private ParticleSystem _staffParticlesPrefab;
    
    private SpineAnimationController _anim;
    
    private float horizontal;
    private Rigidbody2D _rb;
    private BoxCollider2D _collider;
    private HealthBar _healthBar;
    
    public bool HasStaff;
    public bool HasBell;
    public bool HomeOpened;
    
    private bool _isJump;
    private bool _isGrounded;
    private bool _jumpAllowed = true;
    
    private float _moveSpeed = 0;
    private bool _canMove = true;

    [HideInInspector]
    public int FaceDirection = 1;

    private bool _leftBtnPressed;
    private PlayerInput _playerInput;
    
    private List<InteractiveObject> _interactiveObjectsInRange;
    private InteractionTip _interactionTip;
    private InteractiveObject _nearestInteractive;
    
    [HideInInspector]
    public float SlowFactor;
	
    public bool LongAttackAllowed = true;
    private bool _inv = false;
    private DialogSystem _finalDialog;
    
    private void Awake()
    {
        ComponentSetup();
    }
    
    private void Start()
    {
        AnimationSetup();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            menu.gameObject.SetActive(!menu.gameObject.activeSelf);
            if (menu.gameObject.activeSelf)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
        }
        
        if (_canMove)
        {
            if (horizontal != 0)
            {
                FaceDirection = Mathf.FloorToInt(Mathf.Sign(horizontal));
                _anim.transform.localScale = new Vector3(-FaceDirection * 0.65f, 0.65f, 1);
            }
            _moveSpeed = horizontal * speed * (1 - SlowFactor);
        }

        if (_interactiveObjectsInRange.Count > 0)
        {
            _nearestInteractive = _interactiveObjectsInRange.OrderByDescending(
                x => Vector2.Distance(transform.position, x.transform.position)).First();
            if (_nearestInteractive != null)
            {
                _interactionTip.gameObject.SetActive(true);
                _interactionTip.SetText(_nearestInteractive.Tip);
            }
            else
                _interactionTip.gameObject.SetActive(false);
        }
        else
        {
            _nearestInteractive = null;
            _interactionTip.gameObject.SetActive(false);
        }
    }
    
    private void FixedUpdate()
    {
        var newIsGrounded = Physics2D.OverlapCapsule(groundCheck.position, new Vector2(0.5f, 0.1f), CapsuleDirection2D.Horizontal, 0, groundLayer);
        if (newIsGrounded && !_isGrounded && _isJump)
            _anim.PlayAnimation("Jump_end");
        _isGrounded = newIsGrounded;
        _rb.velocity = new Vector2(_moveSpeed, _rb.velocity.y);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public int GetHealth()
    {
        return _health;
    }

    public void ShowFinalDialog(DialogSystem finalDialog)
    {
        //Final epic scene setup
        var didko = FindObjectOfType<Didko>();
        transform.position = goatPlace.transform.position;
        didko.transform.position = didkoPlace.transform.position;
        FaceDirection = 1;
        _anim.transform.localScale = new Vector3(-FaceDirection * 0.65f, 0.65f, 1);
        didko.SetFaceDirection(-1);
        didko.Die();
        
        _anim.PlayAnimation("StaffAttack");
        _finalDialog = finalDialog;
    }

    private IEnumerator ShowFinalDialogRoutine()
    {
        yield return new WaitForSeconds(0.8f);
        _finalDialog.OnComplete.AddListener(() =>
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(3);
        });
        _finalDialog.Activate();
        Time.timeScale = 0;
    }
    
    #region START_SETUP

    private void ComponentSetup()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
        _anim = GetComponentInChildren<SpineAnimationController>();
        _playerInput = GetComponent<PlayerInput>();
        _interactiveObjectsInRange = new List<InteractiveObject>();
        _interactionTip = GetComponentInChildren<InteractionTip>(true);
        _healthBar = FindObjectOfType<HealthBar>();
    }

    private void AnimationSetup()
    {
        _anim.CreateAnimationState("Idle", true)
            .AddTransition("Run", false, () => _moveSpeed != 0);

        _anim.CreateAnimationState("Run", true)
            .AddTransition("Idle", false, () => _moveSpeed == 0)
            .AddTransition("RunWithStaff", false, () => HasStaff);

        _anim.CreateAnimationState("RunWithStaff", true)
            .AddTransition("Idle", false, () => _moveSpeed == 0);
        
        _anim.CreateAnimationState("Jump_start", false);

        _anim.CreateAnimationState("Jump_end", false)
            .AddTransition("Run", false, () => _moveSpeed != 0)
            .AddTransition("Idle", true, () => _moveSpeed == 0);

        _anim.CreateAnimationState("Flute", false)
            .AddTransition("Run", false, () => _moveSpeed != 0)
            .AddTransitionOnComplete("Idle");

        _anim.CreateAnimationState("Bird", false)
            .AddTransition("Run", false, () => _moveSpeed != 0)
            .AddTransitionOnComplete("Idle");

        _anim.CreateAnimationState("Topot", false)
            .AddTransition("Run", false, () => _moveSpeed != 0)
            .AddTransitionOnComplete("Idle");

        _anim.CreateAnimationState("HornAttack", false)
            .AddTransition("Run", true, () => _moveSpeed != 0)
            .AddTransition("Idle", true, () => _moveSpeed == 0);

        _anim.CreateAnimationState("HornAttack_Long", false)
            .AddTransition("Run", true, () => _moveSpeed != 0)
            .AddTransition("Idle", true, () => _moveSpeed == 0);

        _anim.CreateAnimationState("StaffAttack", false)
            .AddTransition("Run", true, () => _moveSpeed != 0)
            .AddTransitionOnComplete("Idle");

        _anim.CreateAnimationState("FallIntoCart", false)
            .AddTransitionOnComplete("Idle");

        _anim.CreateAnimationState("Climb", true);

        _anim.CreateAnimationState("Inv", false);
        
        _anim.CreateAnimationState("Damage", false)
            .AddTransitionOnComplete("Idle");

        _anim.CreateAnimationState("Die", false);
        
        _anim.OnAnimationComplete.AddListener(x =>
        {
            if (x.StateName == "FallIntoCart")
            {
                _anim.SetSortingLayer("Default", 0);
                ToggleControls(true);
            }
            else if (x.StateName == "Jump_start")
            {
                _rb.velocity = new Vector2(_rb.velocity.x, jumpingPower);
            }
            else if (x.StateName == "HornAttack")
            {
                _jumpAllowed = true;
                _canMove = true;
            }
            else if (x.StateName == "HornAttack_Long")
            {
                _jumpAllowed = true;
                _canMove = true;
            }
            else if (x.StateName == "Damage")
            {
                
            }
            else if (x.StateName == "Inv")
            {
                _inv = false;
            }
            else if (x.StateName == "Flute")
            {
                _health = 5;
                _heal--;
                _healthBar.UpdateHealth(_health);
                _healthBar.UpdateHeal(_heal);
            }
        });

        _anim.OnAnimationEvent.AddListener(x =>
        {
            if (x.EventData.Data.Name == "HornAttack_End")
            {
                if (_leftBtnPressed)
                {
                    _anim.PlayAnimation("HornAttack_Long");
                    _moveSpeed = 12 * FaceDirection;
                }

                List<Collider2D> _hitColliders = new List<Collider2D>();
                var contactFilter = new ContactFilter2D();
                contactFilter.useLayerMask = true;
                contactFilter.layerMask = LayerMask.GetMask("HitBox");
                Physics2D.OverlapCollider(_hornAttackArea, contactFilter, _hitColliders);
                if (_hitColliders.Count > 0)
                {
                    foreach (var hitCollider in _hitColliders)
                    {
                        var hitBox = hitCollider.GetComponent<HitBox>();
                        if (hitBox != null)
                            hitBox.Hit(HitType.Horn);
                    }
                }
            }
            else if (x.EventData.Data.Name == "HornAttackLong_MoveEnd")
            {
                _moveSpeed = 0;
            }
            else if (x.EventData.Data.Name == "staffattack")
            {
                if (_finalDialog != null)
                {
                    StartCoroutine(ShowFinalDialogRoutine());
                }
                
                var instance = Instantiate(_staffParticlesPrefab, _staffParticlesOrigin.position, Quaternion.identity);
                instance.transform.localScale = new Vector3(FaceDirection, 1, 1);
            }
        });

        if (_skipSpawn)
        {
            _anim.PlayAnimation("Idle");
            ToggleControls(true);
            _anim.SetSortingLayer("Default", 0);
        }
        else
        {
            transform.position = _spawnPoint.position;
            _anim.PlayAnimation("FallIntoCart");
            _anim.SetSortingLayer("Back", 9);
        }
    }
    
    #endregion

    public void Die()
    {
        _anim.PlayAnimation("Die");
    }

    public void Live()
    {
        _anim.PlayAnimation("Idle");
    }
    
    public void GetDamage()
    {
        if (_inv)
            return;
        _health--;
        _healthBar.UpdateHealth(_health);
        
        _inv = true;
        _anim.PlayAnimation("Inv", 1);
        _anim.PlayAnimation("Damage");
        StartCoroutine(DamageRoutine());
        if (_health == 0)
        {
            SceneManager.LoadScene(0);
        }
    }

    private IEnumerator DamageRoutine()
    {
        ToggleControls(false);
        yield return new WaitForSeconds(1.1f);
        ToggleControls(true);
    }

    public void AddBird()
    {
        _birds++;
        _healthBar.UpdateBirds(_birds);
    }
    
    #region INTERACTIVE_OBJECT_SETTINGS

    public void AddInteractiveObject(InteractiveObject interactiveObject)
    {
        if (!_interactiveObjectsInRange.Contains(interactiveObject))
            _interactiveObjectsInRange.Add(interactiveObject);
    }
    
    public void RemoveInteractiveObject(InteractiveObject interactiveObject)
    {
        if (_interactiveObjectsInRange.Contains(interactiveObject))
            _interactiveObjectsInRange.Remove(interactiveObject);
    }

    #endregion

    #region PLAYER_CONTROLS

    public void ToggleControls(bool value)
    {
        _playerInput.enabled = value;
    }
    
    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(context.performed && _isGrounded && _jumpAllowed)
        {
            _anim.PlayAnimation("Jump_start");
            _isJump = true;
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            print("Dash");
            _rb.velocity = new Vector2(dashPower + _rb.velocity.x, _rb.velocity.y);
        }
    }

    public void ClimbLadder(Ladder ladder)
    {
        StartCoroutine(ClimbLadderRoutine(ladder));
    }

    private IEnumerator ClimbLadderRoutine(Ladder ladder)
    {
        _collider.enabled = false;
        _rb.simulated = false;
        ToggleControls(false);
        transform.position = ladder.Start.position;
        _anim.PlayAnimation("Climb");
        
        while (Vector3.Distance(ladder.End.position, transform.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, ladder.End.position, 3 * Time.deltaTime);
            yield return null;
        }
        
        _anim.PlayAnimation("Idle");
        _rb.simulated = true;
        _collider.enabled = true;
        ToggleControls(true);
    }
    
    public void Flute(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_isGrounded)
            {
                if (_nearestInteractive != null)
                {
                    _nearestInteractive.Interact();
                }
                else if (_heal > 0)
                {
                    horizontal = 0;
                    _anim.PlayAnimation("Flute");
                }
            }
        }
    }
    
    public void Bird(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_isGrounded)
            {
                horizontal = 0;
                _anim.PlayAnimation("Bird");
            }
        }
    }
    
    public void StaffAttack(InputAction.CallbackContext context)
    {
        if (!HasStaff)
            return;
        
        if (context.performed)
        {
            if (_isGrounded)
            {
                horizontal = 0;
                _anim.PlayAnimation("StaffAttack");
            }
        }
    }
    
    public void Topot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_isGrounded)
            {
                horizontal = 0;
                _anim.PlayAnimation("Topot");
                
                List<Collider2D> _hitColliders = new List<Collider2D>();
                var contactFilter = new ContactFilter2D();
                contactFilter.useLayerMask = true;
                contactFilter.layerMask = LayerMask.GetMask("HitBox");
                Physics2D.OverlapCollider(_fearArea, contactFilter, _hitColliders);
                if (_hitColliders.Count > 0)
                {
                    foreach (var hitCollider in _hitColliders)
                    {
                        var hitBox = hitCollider.GetComponent<HitBox>();
                        if (hitBox != null)
                            hitBox.Hit(HitType.Fear);
                    }
                }
            }
        }
    }

    private float _startAttackTime;
    public void Attack(InputAction.CallbackContext context)
    {
        if (context.started)
            _leftBtnPressed = true;
        else if (context.canceled)
            _leftBtnPressed = false;
        else if (context.performed)
        {
            if (_isGrounded)
            {
                _anim.PlayAnimation("HornAttack");
                _moveSpeed = 0;
                _canMove = false;
                _jumpAllowed = false;
            }
        }
    }
    
    #endregion
}
