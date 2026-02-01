using System.Collections;
using MainScripts.Spine;
using UnityEngine;

public class Domovoy : MonoBehaviour
{
    [SerializeField]
    private float _rollSpeed;
    
    [SerializeField]
    private float _jumpSpeed;

    public DialogSystem Dialog;
    public DialogSystem FinalDialog;
    public InteractiveObject Ladder;
    public BoxCollider2D Exit;
    public Trigger InitialTrigger;

    public DamageArea RollDamageArea;
    public DamageArea JumpDamageArea;
    public DamageArea KickDamageArea;
    public Transform SpawnPoint;
    
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
    
    [SerializeField] 
    private int _standHp = 5;
    
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
            {
                JumpDamageArea.gameObject.SetActive(true);
                _rb.velocity = Vector2.zero;
            }
            else if (x.EventData.Data.Name == "kick")
            {
                KickDamageArea.gameObject.SetActive(true);
            }
        });
        
        _anim.OnAnimationComplete.AddListener(x =>
        {
            if (x.StateName == "Roll")
                _isRolling = false;
            else if (x.StateName == "Jump" || x.StateName == "Kick")
            {
                _isJumping = false;
                KickDamageArea.gameObject.SetActive(false);
                JumpDamageArea.gameObject.SetActive(false);
            }
        });
        
        _standHitBox.OnHit.AddListener((x) =>
        {
            if (x == HitType.Fear)
                return;
            if (_hp > 0)
            {
                _anim.PlayAnimation("Damage", 1);
                _hp -= 1;
                if (_hp == _standHp)
                {
                    StopCoroutine(_stateRoutine);
                    _skins.TryAddSkin("Legs");
                    _isRolling = false;
                    _rb.velocity = Vector2.zero;
                    _stateRoutine = StartCoroutine(StandStateRoutine());
                }
            }
            else
            {
                StopAllCoroutines();
                _rb.velocity = Vector2.zero;
                _anim.PlayAnimation("RollIdle");
                FinalDialog.Activate();
            }
        });
        
        _startScale = _anim.transform.localScale.x;
        SetFaceDirection(1);
        
        InitialTrigger.OnTriggerEnter.AddListener(() =>
        {
            InitialTrigger.gameObject.SetActive(false);
            Exit.gameObject.SetActive(false);
            Ladder.ToggleInteractable(false);
            Dialog.Activate();
        });
        
        Dialog.OnComplete.AddListener(() =>
        {
            _stateRoutine = StartCoroutine(RollStateRoutine());
        });
        
        FinalDialog.OnComplete.AddListener(() =>
        {
            Exit.gameObject.SetActive(true);
            Ladder.ToggleInteractable(true);
            transform.position = SpawnPoint.position;
            _rb.simulated = false;
            JumpDamageArea.gameObject.SetActive(false);
            RollDamageArea.gameObject.SetActive(false);
            KickDamageArea.gameObject.SetActive(false);
            StartCoroutine(Talk());
        });
        
        _anim.PlayAnimation("RollIdle");
    }

    private IEnumerator Talk()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(6, 10f));
            var chance = Random.Range(0, 100);
            if (chance < 15)
            {
                TalkTextController.SpawnTalkText(transform.position + new Vector3(0, 3, 0), 
                    "Хррр...");
            }
            else if (chance < 40)
            {
                TalkTextController.SpawnTalkText(transform.position + new Vector3(0, 3, 0), 
                    "Не набридай...");
            }
            else if (chance < 65)
            {
                TalkTextController.SpawnTalkText(transform.position + new Vector3(0, 3, 0), 
                    "Коли вже підеш?");
            }
            else if (chance < 85)
            {
                TalkTextController.SpawnTalkText(transform.position + new Vector3(0, 3, 0), 
                    "Не шуми...");
            }
            else
            {
                TalkTextController.SpawnTalkText(transform.position + new Vector3(0, 3, 0), 
                    "Залиш мене в спокої");
            }
        }
    }
    
    private IEnumerator StandStateRoutine()
    {
        RollDamageArea.gameObject.SetActive(false);
        while (true)
        {
            _isJumping = true;
            if (Vector2.Distance(transform.position, _player.transform.position) < 3 && Random.Range(0, 100) < 50)
                _anim.PlayAnimation("Kick");
            else
                _anim.PlayAnimation("Jump");
            
            yield return new WaitUntil(() => !_isJumping);
            
            var chance = Random.Range(0, 100);
            if (chance < 15)
            {
                TalkTextController.SpawnTalkText(transform.position + new Vector3(0, 3, 0), 
                    "ДОСИТЬ ШУМІТИ!!!");
            }
            else if (chance < 30)
            {
                TalkTextController.SpawnTalkText(transform.position + new Vector3(0, 3, 0), 
                    "Начувайся!");
            }
            
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
            RollDamageArea.gameObject.SetActive(true);
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
            RollDamageArea.gameObject.SetActive(false);

            var chance = Random.Range(0, 100);
            if (chance < 15)
            {
                TalkTextController.SpawnTalkText(transform.position + new Vector3(0, 3, 0), 
                    "Як же ти мене дратуєш!");
            }
            else if (chance < 30)
            {
                TalkTextController.SpawnTalkText(transform.position + new Vector3(0, 3, 0), 
                    "Хррр...");
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
