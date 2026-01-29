using MainScripts.Spine;
using Spine;
using Spine.Unity;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Unity.VisualScripting.Member;

public static class AnimationKeys
{
    public static string Idle = "idle";
    public static string Attack = "attack";
    public static string Damage = "damage";
    public static string Jump = "jump";
    public static string RunAway = "runAway";
    public static string Walk = "walk";
}

public static class Enemy_State
{
    //Stats
    public static float health = 10f;
    public static float damage = 10f;

    //Movement
    public static int direction;
    public static float attackDistance;

    //Animation States
    public static bool IsMoving;
    public static bool IsIdling;
    public static bool IsGrounded;
    public static bool IsPlayerProximity;
    public static bool IsPlayerClose;
    public static bool ShouldStopMoving;
    public static bool IsRunningFromPlayer;
    public static bool IsDefeated;
}

public class Evil_Controller : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] SpineAnimationController _spine;

    [Header("AI Settings")]
    [SerializeField] float speed;
    [SerializeField] float jumpingPower;
    [SerializeField] float idleChangeTime = 2f;
    [SerializeField] float minXBound = -45f;
    [SerializeField] float maxXBound = -30f;
    [SerializeField] float chanceToStop = 0.4f;
    [SerializeField] float aggroDistance = 10f;
    [SerializeField] float attackDistance = 4f;

    [Header("Grounding")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheck;

    [Header("Player Object")]
    [SerializeField] Transform player;
    
    private Rigidbody2D rb;
    private float distanceToPlayer;
    private float directionToPlayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating(nameof(PickRandomDirection), 0f, idleChangeTime);

        AnimationSetup();
        PlayDefaultAnimation();
    }

    private void FixedUpdate()
    {
        AI_Think();
    }

    #region Animation_Settings

    private void PlayDefaultAnimation()
    {
        _spine.PlayAnimation(AnimationKeys.Idle, 0);
    }

    private void AnimationSetup()
    {
        _spine.CreateAnimationState(AnimationKeys.Idle, true)
            .AddTransition(AnimationKeys.Walk, false, () => Enemy_State.IsMoving)
            .AddTransition(AnimationKeys.Attack, false, () => Enemy_State.IsPlayerClose);

        _spine.CreateAnimationState(AnimationKeys.Walk, true)
           .AddTransition(AnimationKeys.Idle, false, () => !Enemy_State.IsMoving)
           .AddTransition(AnimationKeys.Attack, false, () => Enemy_State.IsPlayerClose);

        _spine.CreateAnimationState(AnimationKeys.Attack, true)
            .AddTransition(AnimationKeys.Walk, false, () => !Enemy_State.IsPlayerClose)
            .AddTransition(AnimationKeys.Idle, false, () => !Enemy_State.IsPlayerProximity);
    }

    #endregion

    #region AI_Control

    private void AI_Think()
    {
        UpdatePlayerProximity();

        print("Enemy_State IsIdling: " + Enemy_State.IsIdling);
        print("Enemy_State IsPlayerProximity: " + Enemy_State.IsPlayerProximity);
        print("Enemy_State IsPlayerClose: " + Enemy_State.IsPlayerClose);
        print("Enemy_State ShouldStop: " + Enemy_State.ShouldStopMoving);
        print("Enemy_State Distance: " + distanceToPlayer);
        print("Enemy_State IsMoving: " + Enemy_State.IsMoving);
        print("Enemy_State XVelocity: " + Mathf.Abs(rb.velocity.x));

        if (Enemy_State.IsIdling)
        {
            Move(Enemy_State.direction);
        }
        else if (Enemy_State.IsPlayerProximity)
        {
            MoveToPlayer();
        }
        else if (Enemy_State.IsPlayerClose)
        {
            AttackPlayer();
        }
    }

    void Move(float direction)
    {
        rb.velocity = new Vector2(direction * speed, rb.velocity.y);

        if (direction != 0)
        {
            transform.localScale = new Vector3(-direction, transform.localScale.y, transform.localScale.z);
        }
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
    }

    void MoveToPlayer()
    {
        directionToPlayer = Mathf.Sign(player.position.x - transform.position.x);

        if (!Enemy_State.ShouldStopMoving)
        {
            Move(directionToPlayer);
        }

        if(!Enemy_State.ShouldStopMoving && !Enemy_State.IsMoving && Enemy_State.IsGrounded)
        {
            print("I should keep moving && but I can't move, && my feet are touching the ground => so I should Jump.");
            Jump();
        }
    }

    void PickRandomDirection()
    {
        // -1 = left, 0 = idle, 1 = right

        if (!Enemy_State.IsIdling) return;

        float x = transform.position.x;

        Enemy_State.direction = x <= minXBound ? 1 :
                     x >= maxXBound ? -1 :
                     Random.Range(-1, 2);

        if (Random.value < chanceToStop) Enemy_State.direction = 0;
    }

    private void AttackPlayer()
    {

    }

    private void UpdatePlayerProximity()
    {
        distanceToPlayer = Vector2.Distance(player.position, transform.position);

        Enemy_State.IsMoving = Mathf.Abs(rb.velocity.x) >= 1;

        Enemy_State.IsGrounded = Physics2D.OverlapCapsule(groundCheck.position, new Vector2(0.5f, 0.1f), CapsuleDirection2D.Horizontal, 0, groundLayer);

        Enemy_State.IsPlayerProximity = distanceToPlayer < aggroDistance;
        Enemy_State.IsPlayerClose = distanceToPlayer < attackDistance;
        Enemy_State.ShouldStopMoving = Enemy_State.IsPlayerClose;

        Enemy_State.IsIdling = !Enemy_State.IsPlayerProximity;
    }

    #endregion
}
