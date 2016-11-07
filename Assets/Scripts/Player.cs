﻿using UnityEngine;
using System.Collections;
using System;

public class Player : Actor, IControllableActor, IAttackableActor {

    

    public PlayerIdle state_idle;
    public PlayerAirborn state_airborn;
    public PlayerAttack state_attack;
    public bool attack_pressed;
    public bool special_pressed;
    bool hit_invuln = false;
    float hit_invuln_counter = 0;
    float hit_invuln_time = 1.0f;
    float HitInvulnTime
    {
        get { return hit_invuln_time; }
    }
    bool hit_flicker = false;

    public enum AnimId
    {
        idle=0,
        attack1
    }

    void Update()
    {
        fsm.Update();
        if (hit_invuln)
        {
           ProcessHitFlicker();
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

        fsm.ChangeState(state_idle);
	}

    public void takeDamage(int damage)
    {
        if (!hit_invuln)
        {
            Debug.Log("take damage!");
            hit_invuln = true;
        }
    }

    public void knockBack(Vector2 knockback)
    {
        if (facing_right)
        {
            _rigidbody.velocity = new Vector2(-knockback.x, knockback.y);
        }
        else
        {
            _rigidbody.velocity = new Vector2(knockback.x, knockback.y);
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

public class PlayerHurt : FSM_State
{
    Player _player;
    float hurt_time;

    public PlayerHurt(FiniteStateMachine fsm, Player player) : base(fsm)
    {
        _player = player;
        hurt_time = _player.
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