using UnityEngine;
using System.Collections;
using System;

public class Player : Actor, IControllableActor {

    const float ACCEL_TIME = 0.3f;
    const float AIR_ACCEL_TIME = 0.6f;
    const float AIR_DECEL_TIME = 0.6f;
    const float DECEL_TIME = 0.2f;
    const float MOVE_SPEED = 5.0f;
    const float GRAVITY_SCALE = 5.0f;
    const float AIR_GRAVITY_SCALE = 2.0f;
    const float JUMPSPEED = 15;
    const float FALLSPEED = 30;

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

    public PlayerIdle state_idle;
    public PlayerAirborn state_airborn;
    public PlayerAttack state_attack;
    public bool attack_pressed;
    public bool special_pressed;

    public enum AnimId
    {
        idle=0,
        attack1
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

        fsm.ChangeState(state_idle);
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

            if (_rigidbody.velocity.x > -movement * 2 && _rigidbody.velocity.x < movement * 2)
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
        if(!_player.IsOnGround)
        {
            _fsm.ChangeState(_player.state_airborn);
            return;
        }

        if (_player.attack_pressed || _player.special_pressed)
        {
            _fsm.ChangeState(_player.state_attack);
            return;
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
    }
}

public class PlayerAttack : FSM_State
{
    Player _player;

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
        }
        else if (_player.special_pressed)
        {
            if (_player.up_pressed)
            {
                _player._attack_manager.doUpSpecialAttack();
            }
            else if (_player.down_pressed)
            {
                _player._attack_manager.doDownSpecialAttack();
            }
            else if (_player.forward_pressed)
            {
                _player._attack_manager.doForwardSpecialAttack();
            }
            else
            {
                _player._attack_manager.doSpecialAttack();
            }
        }
        Debug.Log("Hack colour: " + _player._attack_manager.getHackColour());

        _player.setAnimation(_player._attack_manager.getAttackAnim());
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
        }
    }
}