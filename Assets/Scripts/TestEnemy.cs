using UnityEngine;
using System.Collections;
using System;

public class TestEnemy : Enemy, IAttackableActor, IHackableActor {

    public TestEnemyIdle state_idle;
    public TestEnemyHurt state_hurt;
    public TestEnemyDie state_die;
    public TestEnemyDead state_dead;

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

    public void onHackBlue()
    {
        throw new NotImplementedException();
    }

    public void onHackCyan()
    {
        throw new NotImplementedException();
    }

    public void onHackPurple()
    {
        throw new NotImplementedException();
    }

    public void onHackRed()
    {
        throw new NotImplementedException();
    }

    public void takeDamage(int damage)
    {
        if (is_dead)
        {
            return;
        }
        cur_health -= damage;
        Debug.Log("TestEnemy took " + damage + " damage!");
        fsm.ChangeState(state_hurt);

    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        max_health = 5;
        cur_health = max_health;

        state_idle = new TestEnemyIdle(fsm, this);
        state_die = new TestEnemyDie(fsm, this);
        state_dead = new TestEnemyDead(fsm, this);
        state_hurt = new TestEnemyHurt(fsm, this);

        fsm.ChangeState(state_idle);
    }

    // Update is called once per frame
    void Update () {
        fsm.Update();
	}
}

public class TestEnemyIdle : FSM_State
{
    TestEnemy _actor;

    public TestEnemyIdle(FiniteStateMachine fsm, TestEnemy actor) : base(fsm)
    {
        _actor = actor;
    }

    public override void FixedUpdate()
    {
        _actor.Horizontal_Movement(_actor.hor_move_axis);
        _actor.Vertical_Movement(_actor.jump_pressed);
    }

    public override void OnEnter()
    {
        Debug.Log("TestEnemy idle entered");
    }

    public override void OnExit()
    {
        //throw new NotImplementedException();
    }

    public override void Update()
    {
        //throw new NotImplementedException();
    }
}

public class TestEnemyHurt : FSM_State
{
    TestEnemy _actor;
    float hurt_timer = 1.0f;
    float counter = 0.0f;

    public TestEnemyHurt(FiniteStateMachine fsm, TestEnemy actor) : base(fsm)
    {
        _actor = actor;
    }

    public override void FixedUpdate()
    {
        _actor.Horizontal_Movement(0.0f);
        _actor.Vertical_Movement(false);
    }

    public override void OnEnter()
    {
        Debug.Log("TestEnemy hurt entered");
        counter = 0.0f;
    }

    public override void OnExit()
    {
        
    }

    public override void Update()
    {
        counter += Time.deltaTime;

        if(_actor.cur_health <= 0)
        {
            _fsm.ChangeState(_actor.state_die);
            return;
        }

        if(counter > hurt_timer)
        {
            _fsm.ChangeState(_actor.state_idle);
        }
    }
}

public class TestEnemyDie : FSM_State
{
    TestEnemy _actor;
    float dying_time = 2.0f;
    float counter = 0.0f;

    public TestEnemyDie(FiniteStateMachine fsm, TestEnemy actor) : base(fsm)
    {
        _actor = actor;
    }

    public override void FixedUpdate()
    {
        _actor.Horizontal_Movement(0.0f);
        _actor.Vertical_Movement(false);
    }

    public override void OnEnter()
    {
        Debug.Log("TestEnemy dying");
        _actor.is_dead = true;
        counter = 0.0f;
    }

    public override void OnExit()
    {
        
    }

    public override void Update()
    {
        counter += Time.deltaTime;

        if(counter > dying_time)
        {
            _fsm.ChangeState(_actor.state_dead);
        }
    }
}

public class TestEnemyDead : FSM_State
{
    TestEnemy _actor;

    public TestEnemyDead(FiniteStateMachine fsm, TestEnemy actor) : base(fsm)
    {
        _actor = actor;
    }

    public override void FixedUpdate()
    {
        
        _actor.Horizontal_Movement(0.0f);
        _actor.Vertical_Movement(false);
    }

    public override void OnEnter()
    {
        Debug.Log("TestEnemy dead");
    }

    public override void OnExit()
    {
        
    }

    public override void Update()
    {
        
    }
}