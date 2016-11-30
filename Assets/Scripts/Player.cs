using UnityEngine;
using System.Collections;
using System;

public class Player : Actor, IControllableActor, IAttackableActor {



    public PlayerIdle state_idle;
    public PlayerAirborn state_airborn;
    public PlayerAttack state_attack;
    public PlayerHurt state_hurt;
    public PlayerDying state_dying;
    public PlayerDead state_dead;
    public PlayerLevelClearing state_clear;
    public bool attack_pressed;
    public bool special_pressed;


    public AudioClip sound_hurt;
    public AudioClip sound_jump;
    public AudioClip sound_attack;
    public AudioClip sound_special;
    public AudioClip sound_upspecial;
    public AudioClip sound_forwardspecial;
    public AudioClip sound_downspecial;
    public AudioClip sound_die;
    public AudioClip sound_clear;
    public AudioClip sound_teleport;

    bool hit_invuln = false;
    public bool invuln = false;
    float hit_invuln_counter = 0;
    float hit_invuln_time = 1.0f;
    public float HitInvulnTime
    {
        get { return hit_invuln_time; }
    }
    bool hit_flicker = false;

    public enum AnimId
    {
        idle=0,
        attack1,
        special,
        upSpecial,
        downSpecial,
        forwardSpecial,
        hurt,
        jump,
        run,
        die,
        clear
    }

    void Update()
    {
        fsm.Update();
        if (hit_invuln)
        {
           ProcessHitFlicker();
        }

        _animator.SetFloat("YVel", _rigidbody.velocity.y);
        _animator.SetBool("OnGround", IsOnGround);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("the player walked into something!");
        if (collider.gameObject.CompareTag("Pickup"))
        {
            MonoBehaviour script = collider.gameObject.GetComponent<MonoBehaviour>();

            if(script is HeartPickup)
            {
                (script as HeartPickup).OnPickUp(this);
            }

            if (script is CollectableBit)
            {
                (script as CollectableBit).Collect();
                Debug.Log("collected time: " + Time.time);
            }
        }
    }


    void ProcessHitFlicker()
    {
        if (hit_invuln_counter < hit_invuln_time)
        {
            hit_invuln_counter += Time.deltaTime;
            if (hit_flicker)
            {
                hit_flicker = false;
            }
            else
            {
                hit_flicker = true;
            }
            _spriteRenderer.enabled = hit_flicker;
        }
        else
        {
            hit_invuln_counter = 0.0f;
            hit_invuln = false;
            hit_flicker = false;
            _spriteRenderer.enabled = true;
        }
    }


    //IControllableActor
    public void Attack()
    {
        attack_pressed = true;
    }

    public void Attack_release()
    {
        attack_pressed = false;
    }

    public void Special_pressed()
    {
        special_pressed = true;
    }

    public void Special_released()
    {
        special_pressed = false;
    }

    public void Up_pressed()
    {
        up_pressed = true;
    }

    public void Up_released()
    {
        up_pressed = false;
    }

    public void Down_pressed()
    {
        down_pressed = true;
    }

    public void Down_released()
    {
        down_pressed = false;
    }

    public void Forward_pressed()
    {
        forward_pressed = true;
    }

    public void Forward_released()
    {
        forward_pressed = false;
    }



    public void Jump()
    {
        if (!jump_locked)
        {
            jump_pressed = true;
           
        }

        
    }

    public void Jump_release()
    {
        jump_pressed = false;
    }

    public void Move(float axis)
    {
        hor_move_axis = axis;
    }
   

    public void setToGroundAccel()
    {
        accel_time = ACCEL_TIME;
        decel_time = DECEL_TIME;
    }

    public void setToAirAccel()
    {
        accel_time = AIR_ACCEL_TIME;
        decel_time = AIR_DECEL_TIME;
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();

        isPlayer = true;

        _attack_manager = new AttackManager();
        _attack_manager.Initialise("PlayerAttacks.json");

        state_idle = new PlayerIdle(fsm, this);
        state_airborn = new PlayerAirborn(fsm, this);
        state_attack = new PlayerAttack(fsm, this);
        state_hurt = new PlayerHurt(fsm, this);
        state_dying = new PlayerDying(fsm, this);
        state_dead = new PlayerDead(fsm, this);
        state_clear = new PlayerLevelClearing(fsm, this);

        fsm.ChangeState(state_idle);

        max_health = 10;
        cur_health = max_health;
	}

    public void takeDamage(int damage)
    {
        if(hit_invuln || is_dead || is_dying || invuln)
        {
            return;
        }

        Debug.Log("Player took " + damage + " damage!");
       cur_health -= damage;
       if(cur_health <= 0)
       {
           fsm.ChangeState(state_dying);
           return;
       }
       hit_invuln = true;
       fsm.ChangeState(state_hurt);

    }

    public void restoreHealth(int health)
    {
        cur_health += health;
        if(cur_health > max_health)
        {
            cur_health = max_health;
        }
    }

    public void knockBack(Vector2 knockback)
    {
        if (hit_invuln || is_dying || invuln)
        {
            return;
        }

        if (facing_right)
        {
            _rigidbody.velocity = new Vector2(-knockback.x, knockback.y);
        }
        else
        {
            _rigidbody.velocity = new Vector2(knockback.x, knockback.y);
        }
        
    }

    public bool GetIsPlayer()
    {
        return isPlayer;
    }

    public bool GetIsEnemy()
    {
        return isEnemy;
    }
}

public class PlayerIdle : FSM_State
{
    Player _player;

    public PlayerIdle(FiniteStateMachine fsm, Player player) : base(fsm)
    {
        _player = player;
    }

    public override void FixedUpdate()
    {
        _player.Horizontal_Movement(_player.hor_move_axis);
        _player.Vertical_Movement(_player.jump_pressed);
    }

    public override void OnEnter()
    {;
        _player.setToGroundAccel();
        _player._animation_monitor.reset();
        _player.setAnimation((int)Player.AnimId.idle);
    }

    public override void OnExit()
    {
        //throw new NotImplementedException();
    }

    public override void Update()
    {
        if (_player.isOnGround && _player.jump_pressed)
        {
            if (!_player._audiosource.isPlaying)
            {
                _player._audiosource.clip = _player.sound_jump;
                _player._audiosource.Play();
            }
            
        }

        if (!_player.IsOnGround)
        {
            _fsm.ChangeState(_player.state_airborn);
            return;
        }

        if (_player.attack_pressed || _player.special_pressed)
        {
            _fsm.ChangeState(_player.state_attack);
            return;
        }

        //animation
        if(_player.hor_move_axis == 0)
        {
            _player.setAnimation((int)Player.AnimId.idle);
        }
        else
        {
            _player.setAnimation((int)Player.AnimId.run);
        }
    }
}

public class PlayerAirborn : FSM_State
{
    Player _player;

    public PlayerAirborn(FiniteStateMachine fsm, Player player) : base(fsm)
    {
        _player = player;
    }

    public override void FixedUpdate()
    {
        _player.Horizontal_Movement(_player.hor_move_axis);
        _player.Vertical_Movement(_player.jump_pressed);
    }

    public override void OnEnter()
    {
        Debug.Log("Airborn state entered");
        _player.setToAirAccel();
        _player.setAnimation((int)Player.AnimId.jump);
    }

    public override void OnExit()
    {
        //throw new NotImplementedException();
    }

    public override void Update()
    {
        if (_player.IsOnGround)
        {
            _fsm.ChangeState(_player.state_idle);
        }

        if (_player.attack_pressed || _player.special_pressed)
        {
            _fsm.ChangeState(_player.state_attack);
            return;
        }
    }
}

public class PlayerAttack : FSM_State
{
    Player _player;
    bool isInAir = false;

    public PlayerAttack(FiniteStateMachine fsm, Player player):base(fsm)
    {
        _player = player;
    }

    public override void FixedUpdate()
    {
        if (_player._attack_manager.isXinputLocked())
        {
            _player.Horizontal_Movement(0.0f);
        }
        else
        {
            _player.Horizontal_Movement(_player.hor_move_axis);
        }

        if (_player._attack_manager.isJumpInputLocked())
        {
            _player.Vertical_Movement(false);
        }
        else
        {
            _player.Vertical_Movement(_player.jump_pressed);
        }
    }

    public override void OnEnter()
    {
        _player._animation_monitor.reset();
        if (_player.attack_pressed)
        {
            _player._attack_manager.doNormalAttack();
            _player._audiosource.clip = _player.sound_attack;
            _player._audiosource.Play();
        }
        else if (_player.special_pressed)
        {
            if (_player.up_pressed)
            {
                _player._attack_manager.doUpSpecialAttack();
                _player._audiosource.clip = _player.sound_upspecial;
                _player._audiosource.Play();
            }
            else if (_player.down_pressed)
            {
                _player._attack_manager.doDownSpecialAttack();
                _player._audiosource.clip = _player.sound_downspecial;
                _player._audiosource.Play();
            }
            else if (_player.forward_pressed)
            {
                _player._attack_manager.doForwardSpecialAttack();
                _player._audiosource.clip = _player.sound_forwardspecial;
                _player._audiosource.Play();
            }
            else
            {
                _player._attack_manager.doSpecialAttack();
                _player._audiosource.clip = _player.sound_special;
                _player._audiosource.Play();
            }
        }
        Debug.Log("Hack colour: " + _player._attack_manager.getHackColour());

        _player.setAnimation(_player._attack_manager.getAttackAnim());

        isInAir = !_player.IsOnGround;
    }

    public override void OnExit()
    {
        
    }

    public override void Update()
    {
        if (_player._animation_monitor.isAnimationComplete())
        {
            _fsm.ChangeState(_player.state_idle);
            _player._attack_manager.resetAttackCombo();
            return;
        }

        if(isInAir && _player.IsOnGround)
        {
            _fsm.ChangeState(_player.state_idle);
            _player._attack_manager.resetAttackCombo();
            return;
        }

        if (!isInAir && !_player.IsOnGround)
        {
            _fsm.ChangeState(_player.state_idle);
            _player._attack_manager.resetAttackCombo();
            return;
        }
    }
}

public class PlayerHurt : FSM_State
{
    Player _player;
    float hurt_time;
    float hurt_timer = 0.0f;

    public PlayerHurt(FiniteStateMachine fsm, Player player) : base(fsm)
    {
        _player = player;
        hurt_time = _player.HitInvulnTime / 4;
    }

    public override void FixedUpdate()
    {
        _player.Uncontrolled_Horizontal_Movement();
        _player.Vertical_Movement(false);
    }

    public override void OnEnter()
    {
        hurt_timer = 0.0f;
        _player.setAnimation((int)Player.AnimId.hurt);
        _player._audiosource.clip = _player.sound_hurt;
        _player._audiosource.Play();
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        hurt_timer += Time.deltaTime;

        if(hurt_timer > hurt_time)
        {
            _fsm.ChangeState(_player.state_idle);
        }
    }
}

public class PlayerDying : FSM_State
{
    Player _player;
    float dying_time = 2.0f;
    float dying_timer = 0.0f;

    public PlayerDying(FiniteStateMachine fsm, Player player) : base(fsm)
    {
        _player = player;
    }

    public override void FixedUpdate()
    {
        _player.Horizontal_Movement(0.0f);
        _player.Vertical_Movement(false);
    }

    public override void OnEnter()
    {
        Debug.Log("Player is dying!");
        _player.is_dying = true;
        dying_timer = 0.0f;
        _player._audiosource.clip = _player.sound_die;
        _player._audiosource.Play();
        _player.setAnimation((int)Player.AnimId.die);
        _player._rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        _player._boxCollider.enabled = false;
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        dying_timer += Time.deltaTime;

        if (dying_timer > dying_time)
        {
            _fsm.ChangeState(_player.state_dead);
        }
    }
}

public class PlayerDead : FSM_State
{
    Player _player;

    public PlayerDead(FiniteStateMachine fsm, Player player) : base(fsm)
    {
        _player = player;
    }

    public override void FixedUpdate()
    {
        _player.Horizontal_Movement(0.0f);
        _player.Vertical_Movement(false);
    }

    public override void OnEnter()
    {
        Debug.Log("Player has died!");
        _player.is_dead = true;
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {

    }
}


public class PlayerLevelClearing : FSM_State
{
    Player _player;
    float clear_time = 2.0f;
    float clear_timer = 0.0f;
    bool clear_sound_playing = false;

    public PlayerLevelClearing(FiniteStateMachine fsm, Player player) : base(fsm)
    {
        _player = player;
    }

    public override void FixedUpdate()
    {
        _player.Horizontal_Movement(0.0f);
        _player.Vertical_Movement(false);
    }

    public override void OnEnter()
    {
        _player.invuln = true;
        _player.setAnimation((int)Player.AnimId.clear);
        _player._audiosource.clip = _player.sound_clear;
        _player._audiosource.Play();
       
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        clear_timer += Time.deltaTime;

        if (clear_timer > clear_time && !clear_sound_playing)
        {
            _player._audiosource.clip = _player.sound_teleport;
            _player._audiosource.Play();
            clear_sound_playing = true;
        }
    }
}