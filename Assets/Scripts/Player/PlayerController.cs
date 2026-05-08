using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MainScripts.Audio;
using MainScripts.Controllers;
using MainScripts.Spine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private int _health;
    [FormerlySerializedAs("_heal")] [SerializeField] private int Heal;
    [SerializeField] private int _birds;
    [SerializeField] float speed;
    [SerializeField] float jumpingPower;
    [SerializeField] float dashPower;
    [SerializeField] private GameObject menu;
    [SerializeField] private Transform goatPlace;
    [SerializeField] private Transform didkoPlace;
    [SerializeField] private Transform introCanvas;
    [SerializeField] private Transform oneHpCanvas;
    [SerializeField] private AudioLibrary _audioLibrary;
    
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
    public bool LastBattle;
    private bool _canOpenMenu;
    public bool PowerFlute;
    
    [SerializeField]
    private Hiter _hornAttackHiter;
    [SerializeField] private GameObject _cheatCanvas;
    
    private void Awake()
    {
        if (Env.Instance.isDebug)
        {
            _skipSpawn = true;
        }
            
        ComponentSetup();
    }
    
    private void Start()
    {
        if (Env.Instance.endlessHeal)
        {
            Heal = 0;
            _healthBar.UpdateHeal(0);
        }

        AnimationSetup();
    }

    public void ShowCheatSheet()
    {
        if (_canOpenMenu)
        {
            _cheatCanvas.gameObject.SetActive(true);
        }
    }
    
    private void Update()
    {
        if (_canOpenMenu && Input.GetKeyUp(KeyCode.Escape))
        {
            if (_cheatCanvas.gameObject.activeSelf)
            {
                _cheatCanvas.gameObject.SetActive(false);
            }
            else
            {
                menu.gameObject.SetActive(!menu.gameObject.activeSelf);
                if (menu.gameObject.activeSelf)
                    Time.timeScale = 0;
                else
                    Time.timeScale = 1;
            }
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
        {
            AudioController.PlayAtWorldPosition(_audioLibrary.GetRandom("Run"), transform.position, 0.7f);
            _anim.PlayAnimation("Jump_end");
            _isJump = false;
        }

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
        _canOpenMenu = false;
        _isJump = false;
        _canMove = false;
        _moveSpeed = 0;
        _rb.velocity = Vector2.zero;
        _jumpAllowed = false;
        _isGrounded = true;
        var didko = FindObjectOfType<Didko>();
        transform.position = goatPlace.transform.position;
        didko.transform.position = didkoPlace.transform.position;
        FaceDirection = 1;
        _anim.transform.localScale = new Vector3(-FaceDirection * 0.65f, 0.65f, 1);
        didko.SetFaceDirection(-1);
        didko.Die();
        _rb.velocity = Vector2.zero;
        
        _anim.PlayAnimation("StaffAttack");
        AudioController.PlayAtWorldPosition(_audioLibrary.GetRandom("Staff"), transform.position);
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
            .AddTransition("Run", false, () => _moveSpeed != 0)
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
                _hornAttackHiter.Toggle(false);
            }
            else if (x.StateName == "HornAttack_Long")
            {
                _jumpAllowed = true;
                _canMove = true;
                _hornAttackHiter.Toggle(false);
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
                if (!Env.Instance.endlessHeal)
                {
                    Heal--;
                    _healthBar.UpdateHeal(Heal);
                }
                
                if (PowerFlute)
                    _health = 6;
                else
                {
                    _health += 3;
                    if (_health > 6)
                        _health = 6;
                }

                _healthBar.UpdateHealth(_health);
                oneHpCanvas.gameObject.SetActive(false);
            }
        });

        _anim.OnAnimationEvent.AddListener(x =>
        {
            if (x.EventData.Data.Name == "HornAttack_End")
            {
                _hornAttackHiter.Toggle(true);
                if (_leftBtnPressed)
                {
                    _anim.PlayAnimation("HornAttack_Long");
                    _moveSpeed = 12 * FaceDirection;
                }
            }
            else if (x.EventData.Data.Name == "HornAttackLong_MoveEnd")
            {
                _moveSpeed = 0;
            }
            else if (x.EventData.Data.Name == "staffattack")
            {
                _canMove = true;
                _jumpAllowed = true;
                
                if (_finalDialog != null)
                {
                    StartCoroutine(ShowFinalDialogRoutine());
                }
                AudioController.PlayAtWorldPosition(_audioLibrary.GetRandom("Staff"), transform.position);
                var instance = Instantiate(_staffParticlesPrefab, _staffParticlesOrigin.position, Quaternion.identity);
                instance.transform.localScale = new Vector3(FaceDirection, 1, 1);
            }
            else if (x.EventData.Data.Name == "step")
            {
                AudioController.PlayAtWorldPosition(_audioLibrary.GetRandom("Run"), transform.position, 0.7f);
            }
            else if (x.EventData.Data.Name == "topot")
            {
                AudioController.PlayAtWorldPosition(_audioLibrary.GetRandom("Run"), transform.position, 0.7f);
            }
            else if (x.EventData.Data.Name == "flute_start")
            {
                AudioController.PlayAtWorldPosition(_audioLibrary.GetRandom("Carol"), transform.position);
            }
            else if (x.EventData.Data.Name == "fallIntoCart_hit")
            {
                AudioController.PlayAtWorldPosition(_audioLibrary.GetRandom("Cart"), transform.position);
            }
        });

        _hornAttackHiter.OnHit.AddListener(() =>
        {
            AudioController.PlayAtWorldPosition(_audioLibrary.GetRandom("Punch"), transform.position, 0.3f);
        });
        
        if (_skipSpawn)
        {
            _anim.PlayAnimation("Idle");
            ToggleControls(true);
            _anim.SetSortingLayer("Default", 0);
            _rb.simulated = true;
            _canOpenMenu = true;
        }
        else
        {
            StartCoroutine(SpawnRoutine());
        }
    }
    
    #endregion

    public IEnumerator SpawnRoutine()
    {
        transform.position = _spawnPoint.position;
        _anim.gameObject.SetActive(false);
        introCanvas.gameObject.SetActive(true);
        yield return new WaitForSeconds(7f);
        introCanvas.gameObject.SetActive(false);
        _anim.gameObject.SetActive(true);
        _anim.PlayAnimation("FallIntoCart");
        _anim.SetSortingLayer("Back", 9);
        _rb.simulated = true;
        _canOpenMenu = true;
    }
    
    public void Die()
    {
        _anim.PlayAnimation("Die");
    }
    
    public void Idle()
    {
        _anim.PlayAnimation("Idle");
        _rb.velocity = Vector2.zero;
        _moveSpeed = 0;
        _canMove = false;
        ToggleControls(false);
        Debug.Log("IDLE");
    }
    
    public void GetDamage()
    {
        if (_inv || _anim.GetActiveStateName() == "HornAttack_Long")
            return;
        _health--;
        _healthBar.UpdateHealth(_health);
        AudioController.PlayAtWorldPosition(_audioLibrary.GetRandom("Damage"), transform.position);
        
        if (_health == 1)
            oneHpCanvas.gameObject.SetActive(true);

        if (_health > 0)
        {
            _inv = true;
            _anim.PlayAnimation("Inv", 1);
            _anim.PlayAnimation("Damage");
            StartCoroutine(DamageRoutine());
        }
        else
        {
            if (!LastBattle)
                SceneManager.LoadScene(0);
            else
            {
                oneHpCanvas.gameObject.SetActive(false);
                FindObjectOfType<Didko>().ShowFinalScene(false);
            }
        }
    }

    private IEnumerator DamageRoutine()
    {
        ToggleControls(false);
        yield return new WaitForSeconds(1.1f);
        ToggleControls(true);
        _canMove = true;
        _jumpAllowed = true;
    }

    public void SetHeal(int value)
    {
        Heal = value;
        _healthBar.UpdateHeal(Heal);
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
        if(context.performed && _isGrounded && _jumpAllowed && !_isJump)
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
                else if (Heal > 0 || Env.Instance.endlessHeal)
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
                if (_anim.GetActiveStateName() == "StaffAttack")
                    return;
                _moveSpeed = 0;
                _canMove = false;
                _jumpAllowed = false;
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
                if (_anim.GetActiveStateName() == "HornAttack" || _anim.GetActiveStateName() == "HornAttack_Long")
                    return;
                _anim.PlayAnimation("HornAttack");
                _moveSpeed = 0;
                _canMove = false;
                _jumpAllowed = false;
            }
        }
    }
    
    #endregion
}
