using UnityEngine;
using System.Collections;

public class Actor : MonoBehaviour {

    protected Rigidbody2D _rigidbody;
    protected BoxCollider2D _boxCollider;
    protected Transform _transform;

    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;

    public AnimationMonitor _animation_monitor;
    public AttackManager _attack_manager;

    public HitBoxManager _hitbox_manager;

    protected bool isPlayer = false;
    protected bool isEnemy = false;

    protected bool facing_right = false;
    public bool IsFacingRight
    {
        get { return facing_right; }
    }

    public bool isOnGround = false;

    public bool IsOnGround
    {
        get { return isOnGround; }
    }

    float distToGround;

    public FiniteStateMachine fsm;
    private float stored_animspeed;
    private Vector2 stored_velocity;
    private bool hitstop;
    private int hitstop_frames;

    public bool IsPlayer
    {
        get
        {
            return isPlayer;
        }
    }

    public bool IsEnemy
    {
        get
        {
            return isEnemy;
        }
    }

    

    public Transform actorTransform
    {
        get { return _transform; }
    }


    // Use this for initialization
    protected virtual void Start () {
        _rigidbody = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _animator = GetComponentInChildren<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _transform = GetComponent<Transform>();

        fsm = new FiniteStateMachine();

        distToGround = _boxCollider.bounds.extents.y;
    }

    bool IsGrounded()
    {
        Vector3 extent_A = new Vector3(_boxCollider.bounds.extents.x, 0.0f, 0.0f);
        Vector3 extent_B = new Vector3(-_boxCollider.bounds.extents.x, 0.0f, 0.0f);

        Debug.DrawRay(transform.position + extent_A, -Vector3.up * (distToGround + 0.1f), Color.green);
        Debug.DrawRay(transform.position + extent_B, -Vector3.up * (distToGround + 0.1f), Color.green);
        bool detector_A = Physics2D.Raycast(transform.position + extent_A, -Vector3.up, distToGround + 0.07f, LayerMask.GetMask("env_solid"));
        bool detector_B = Physics2D.Raycast(transform.position + extent_B, -Vector3.up, distToGround + 0.07f, LayerMask.GetMask("env_solid"));
        return (detector_A || detector_B);
    }

    public virtual void applyImpulse(int index)
    {

    }

    public virtual void applyControlledImpulse(int index)
    {

    }

    public void ApplyHitStop(int frames)
    {
        if (hitstop)
        {
            return;
        }

        hitstop = true;
        hitstop_frames = frames;
        if (_rigidbody != null)
        {
            stored_velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y);
            _rigidbody.velocity = new Vector2(0, 0);
            _rigidbody.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
        }

        stored_animspeed = _animator.speed;
        _animator.speed = 0;
    }

    protected void ProcessHitStop()
    {
        if (!hitstop)
        {
            return;
        }

        if (hitstop_frames > 0)
        {
            hitstop_frames--;
        }
        else
        {
            ResetHitStop();
            hitstop = false;
        }
    }

    public void ResetHitStop()
    {
        if (_rigidbody != null)
        {
            _rigidbody.velocity = new Vector2(stored_velocity.x, stored_velocity.y);
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        _animator.speed = stored_animspeed;
    }

    public void setAnimation(int anim)
    {
        //Debug.Log("Setting Anim to : " + anim);
        _animator.SetInteger("PlayAnim", anim);
    }

    public void resetAnimation(string anim)
    {
        _animator.ResetTrigger(anim);
    }


    // Update is called once per frame
    void Update () {
        fsm.Update();
	}

    void FixedUpdate()
    {
        fsm.FixedUpdate();
        isOnGround = IsGrounded();
    }
}
