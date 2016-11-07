using UnityEngine;
using System.Collections;

public class Actor : MonoBehaviour {

    public const float ACCEL_TIME = 0.3f;
    public const float AIR_ACCEL_TIME = 0.6f;
    public const float AIR_DECEL_TIME = 0.6f;
    public const float DECEL_TIME = 0.2f;
    public const float MOVE_SPEED = 5.0f;
    public const float GRAVITY_SCALE = 5.0f;
    public const float AIR_GRAVITY_SCALE = 2.0f;
    public const float JUMPSPEED = 15;
    public const float FALLSPEED = 30;

    public float hor_move_axis = 0.0f;
    public float movespeed = MOVE_SPEED;
    public float accel_time = ACCEL_TIME;
    public float decel_time = DECEL_TIME;
    public float jumpspeed = JUMPSPEED;
    public float fallspeed = FALLSPEED;

    public bool jump_pressed = false;
    public bool up_pressed = false;
    public bool down_pressed = false;
    public bool forward_pressed = false;
    public bool jump_locked = false;

    public int max_health = 0;
    public int cur_health = 0;
    public bool is_dead = false;


    public Rigidbody2D _rigidbody;

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

    public void Horizontal_Movement(float direction)
    {
        if (direction != 0)
        {
            float movement = direction * (movespeed * Time.fixedDeltaTime) / accel_time;
            if (direction > 0)
            {
                _rigidbody.velocity = new Vector2(Mathf.Min(_rigidbody.velocity.x + movement, movespeed), _rigidbody.velocity.y);
                _transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                facing_right = true;
            }
            else
            {
                _rigidbody.velocity = new Vector2(Mathf.Max(_rigidbody.velocity.x + movement, -movespeed), _rigidbody.velocity.y);
                _transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                facing_right = false;
            }

        }
        else
        {
            float movement = (movespeed * Time.fixedDeltaTime) / decel_time;

            if (_rigidbody.velocity.x > 0)
            {
                movement *= -1;
            }

            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x + movement, _rigidbody.velocity.y);

            if (_rigidbody.velocity.x > -3.5 && _rigidbody.velocity.x < 3.5 && isOnGround)
            {
                _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
            }


        }
    }


    public void Vertical_Movement(bool jumping)
    {
        //TODO - Find some way of enforcing explicit pressing of the jump key so holding jump wont make you bounce.
        if (isOnGround && jumping)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, jumpspeed);
        }

        if (_rigidbody.velocity.y > 0 && jumping)
        {
            //lower gravity's effect
            _rigidbody.gravityScale = AIR_GRAVITY_SCALE;
        }

        if (_rigidbody.velocity.y < 0 || !jumping)
        {
            //lower gravity's effect
            _rigidbody.gravityScale = GRAVITY_SCALE;
            //disable the jump key
            //may want an additional condition to allow for multi-jump
            jump_locked = true;

        }

        if (_rigidbody.velocity.y == 0 || isOnGround)
        {
            jump_locked = false;
        }

        //cap fallspeed
        if (_rigidbody.velocity.y < -fallspeed)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, -fallspeed);
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

    public void BounceActor(Vector2 bounce)
    {
        _rigidbody.AddForce(bounce, ForceMode2D.Impulse);
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
