using UnityEngine;
using System.Collections;

public class Actor : MonoBehaviour {

    public const float ACCEL_TIME = 0.1f;
    public const float AIR_ACCEL_TIME = 0.4f;
    public const float AIR_DECEL_TIME = 0.4f;
    public const float DECEL_TIME = 0.1f;
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

    float x_accel = 0.0f;
    float x_velocity = 0.0f;
    float global_x_velocity = 0.0f;

    public bool jump_pressed = false;
    public bool up_pressed = false;
    public bool down_pressed = false;
    public bool forward_pressed = false;
    public bool jump_locked = false;

    public int max_health = 0;
    public int cur_health = 0;
    public bool is_dying = false;
    public bool is_dead = false;
    public bool instant_decel = false;
    public bool touching_right = false;
    public bool touching_left = false;

    public bool detector_A = false;
    public bool detector_B = false;
    public Rigidbody2D _rigidbody;
    public Rigidbody2D _platform;

    //Hackable Actor stuff.
    public ParticleSystem redhack_particles;
    public ParticleSystem bluehack_particles;
    public ParticleSystem cyanhack_particles;
    public ParticleSystem purplehack_particles;

    protected ParticleSystem.EmissionModule red_emitter;
    protected ParticleSystem.EmissionModule blue_emitter;
    protected ParticleSystem.EmissionModule cyan_emitter;
    protected ParticleSystem.EmissionModule purple_emitter;

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

    public void Uncontrolled_Horizontal_Movement()
    {
        x_velocity = 0.0f;

        x_accel = (movespeed * Time.fixedDeltaTime) / decel_time;

        if (_rigidbody.velocity.x > 0)
        {
            x_accel *= -1;
        }

        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x + x_accel, _rigidbody.velocity.y);

        if (_rigidbody.velocity.x > -0.5 && _rigidbody.velocity.x < 0.5 && isOnGround)
        {
            _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
            x_accel = 0;
        }
    }

    public void Horizontal_Movement(float direction)
    {
        if (direction != 0)
        {
            x_accel = direction * (movespeed * Time.fixedDeltaTime) / accel_time;
            if (direction > 0)
            {
                _transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                facing_right = true;
            }
            else
            {
                _transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                facing_right = false;
            }

            if (x_velocity >= movespeed)
            {
                x_velocity = movespeed;          
            }
            else if (x_velocity <= -movespeed)
            {
                x_velocity = -movespeed;
            }
            //Seems to have a bug where when you're moving the same direction as the platform, your velocity goes the wrong way.
        }
        else
        {
            if (instant_decel)
            {
                x_velocity = 0;
            }
            else
            {
                x_accel = (movespeed * Time.fixedDeltaTime) / decel_time;

                if (x_velocity > 0)
                {
                    x_accel *= -1;
                }
                if (x_velocity > -0.5 && x_velocity < 0.5 && isOnGround)
                {
                    x_velocity = 0;
                    x_accel = 0;
                }
            }
        }

        

        x_velocity = x_velocity + x_accel;

        _rigidbody.velocity = new Vector2(x_velocity, _rigidbody.velocity.y);

       //Debug.Log("local x_accel: " + x_accel + " local xvel: " + x_velocity + "Global velocity: " + _rigidbody.velocity.x);

        if ((_platform != null))
        {
            //Debug.Log("player xvel: " + _rigidbody.velocity.x + " platform xvel: " + _platform.velocity.x + " acceleration: " + x_accel);
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x + _platform.velocity.x,
                _rigidbody.velocity.y);
            //Debug.Log("player total xvel: " + _rigidbody.velocity.x);
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

        if(_boxCollider != null)
        {
            distToGround = _boxCollider.bounds.extents.y;
        }
        

        if(redhack_particles != null)
        {
            red_emitter = redhack_particles.emission;
            red_emitter.enabled = false;
        }
        if(bluehack_particles != null)
        {
            blue_emitter = bluehack_particles.emission;
            blue_emitter.enabled = false;
        }
        if(cyanhack_particles != null)
        {
            cyan_emitter = cyanhack_particles.emission;
            cyan_emitter.enabled = false;
        }
        if (purplehack_particles != null)
        {
            purple_emitter = purplehack_particles.emission;
            purple_emitter.enabled = false;
        }
        
    }

   
    void OnCollisionStay2D(Collision2D collider)
    {
        foreach(ContactPoint2D contact in collider.contacts)
        {
            touching_left = false;
            touching_right = false;
            instant_decel = false;

            if (contact.normal.y == 1.0)
            {
                _platform = collider.gameObject.GetComponent<Rigidbody2D>();
            }

            if(contact.normal.x == 1.0)
            {
                touching_left = true;
            }

            if (contact.normal.x == -1.0)
            {
                touching_right = true;
            }

            if((facing_right && touching_right) || (!facing_right && touching_left))
            {
                //this bit is horrible, there has to be a better way...
                if (contact.collider.gameObject.GetComponent<Rigidbody2D>())
                {
                    if (contact.collider.gameObject.GetComponent<Rigidbody2D>().isKinematic)
                    {
                        x_velocity = 0;
                    }

                }else
                {
                    x_velocity = 0;
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collider)
    {
        _platform = null;
    }

    bool IsGrounded()
    {
        Vector3 extent_A = new Vector3(_boxCollider.bounds.extents.x, 0.0f, 0.0f);
        Vector3 extent_B = new Vector3(-_boxCollider.bounds.extents.x, 0.0f, 0.0f);

        Debug.DrawRay(transform.position + extent_A, -Vector3.up * (distToGround + 0.1f), Color.green);
        Debug.DrawRay(transform.position + extent_B, -Vector3.up * (distToGround + 0.1f), Color.green);

        

        detector_A = Physics2D.Raycast(transform.position + extent_A, -Vector3.up, distToGround + 0.07f, LayerMask.GetMask("env_solid"));
        detector_B = Physics2D.Raycast(transform.position + extent_B, -Vector3.up, distToGround + 0.07f, LayerMask.GetMask("env_solid"));

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
