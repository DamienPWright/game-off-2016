using UnityEngine;
using System.Collections;
using System;

public class EnemyLadybug : Enemy, IAttackableActor, IHackableActor
{

    public LadybugIdle state_idle;
    public LadybugHurt state_hurt;
    public LadybugDie state_die;
    public LadybugDead state_dead;
    public LadybugWander state_wander;

    public Collider2D[] triggers; 

    bool red_hack_active = false;
    bool blue_hack_active = false;
    bool purple_hack_active = false;
    bool cyan_hack_active = false;

    bool hit_invuln = false;
    float hit_invuln_counter = 0;
    float hit_invuln_time = 0.2f;
    public float HitInvulnTime
    {
        get { return hit_invuln_time; }
    }


    new const float MOVE_SPEED = 2.0f;

    public void enableTriggers()
    {
        foreach(Collider2D trig in triggers)
        {
            trig.enabled = true;
        }
    }

    public void disableTriggers()
    {
        foreach (Collider2D trig in triggers)
        {
            trig.enabled = false;
        }
    }

    public void knockBack(Vector2 knockback)
    {
        if (hit_invuln || is_dead)
        {
            return;
        }

        _rigidbody.velocity = new Vector2(knockback.x, knockback.y);

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
        if (purple_hack_active)
        {
            purple_hack_active = false;
            movespeed = MOVE_SPEED;
        }
        else
        {
            purple_hack_active = true;
            movespeed = MOVE_SPEED * 4;
        }
        purple_emitter.enabled = purple_hack_active;
    }

    public void onHackRed()
    {
        if (red_hack_active)
        {
            red_hack_active = false;
            //_spriteRenderer.color = Color.white;
        }
        else
        {
            red_hack_active = true;
            //_spriteRenderer.color = Color.red;
        }
        red_emitter.enabled = red_hack_active;
    }

    public void takeDamage(int damage)
    {
        if (is_dead || hit_invuln)
        {
            return;
        }
        
        cur_health -= damage;
        Debug.Log("TestEnemy took " + damage + " damage!");
        fsm.ChangeState(state_hurt);

    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Attackable"))
        {
            MonoBehaviour script = other.gameObject.GetComponentInParent<MonoBehaviour>();

            if(script is IAttackableActor)
            {
                dealContactDamage(script as IAttackableActor);
            }
        }
    }

    void dealContactDamage(IAttackableActor ia)
    {
        if (ia.GetIsPlayer())
        {
            ia.knockBack(new Vector2(4, 4));
            ia.takeDamage(1);
        }
        if (red_hack_active && ia.GetIsEnemy())
        {
            ia.knockBack(new Vector2(8, 8));
            ia.takeDamage(1); 
        }
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        max_health = 5;
        cur_health = max_health;

        movespeed = MOVE_SPEED;

        state_idle = new LadybugIdle(fsm, this);
        state_die = new LadybugDie(fsm, this);
        state_dead = new LadybugDead(fsm, this);
        state_hurt = new LadybugHurt(fsm, this);
        state_wander = new LadybugWander(fsm, this);

        fsm.ChangeState(state_idle);
    }

    public bool GetIsPlayer()
    {
        return isPlayer;
    }

    public bool GetIsEnemy()
    {
        return isEnemy;
    }

    // Update is called once per frame
    void Update()
    {
        fsm.Update();

        if (hit_invuln)
        {
            if(hit_invuln_counter > hit_invuln_time)
            {
                hit_invuln = false;
                hit_invuln_counter = 0.0f;
            }
        }
    }
}

public class LadybugIdle : FSM_State
{
    EnemyLadybug _actor;

    float idle_time = 1.0f;
    float idle_timer = 0.0f;

    public LadybugIdle(FiniteStateMachine fsm, EnemyLadybug actor) : base(fsm)
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
        //Debug.Log("TestEnemy idle entered");
        idle_timer = 0.0f;
    }

    public override void OnExit()
    {
        //throw new NotImplementedException();
    }

    public override void Update()
    {
        idle_timer += Time.deltaTime;

        if(idle_timer > idle_time)
        {
            _fsm.ChangeState(_actor.state_wander);
            return;
        }
    }
}

public class LadybugHurt : FSM_State
{
    EnemyLadybug _actor;
    float hurt_timer = 0.0f;
    float counter = 0.0f;

    public LadybugHurt(FiniteStateMachine fsm, EnemyLadybug actor) : base(fsm)
    {
        _actor = actor;
        hurt_timer = _actor.HitInvulnTime;
    }

    public override void FixedUpdate()
    {
        _actor.Uncontrolled_Horizontal_Movement();
        _actor.Vertical_Movement(false);
    }

    public override void OnEnter()
    {
        Debug.Log("TestEnemy hurt entered");
        _actor.disableTriggers();
        counter = 0.0f;
    }

    public override void OnExit()
    {
         _actor.enableTriggers();
    }

    public override void Update()
    {
        counter += Time.deltaTime;

        if (_actor.cur_health <= 0)
        {
            _fsm.ChangeState(_actor.state_die);
            return;
        }

        if (counter > hurt_timer)
        {
            _fsm.ChangeState(_actor.state_idle);
        }
    }
}

public class LadybugDie : FSM_State
{
    EnemyLadybug _actor;
    float dying_time = 2.0f;
    float counter = 0.0f;

    public LadybugDie(FiniteStateMachine fsm, EnemyLadybug actor) : base(fsm)
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
        _actor.disableTriggers();
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        counter += Time.deltaTime;

        if (counter > dying_time)
        {
            _fsm.ChangeState(_actor.state_dead);
        }
    }
}

public class LadybugDead : FSM_State
{
    EnemyLadybug _actor;

    public LadybugDead(FiniteStateMachine fsm, EnemyLadybug actor) : base(fsm)
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
        _actor.gameObject.SetActive(false); 
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {

    }
}

public class LadybugWander : FSM_State
{
    EnemyLadybug _actor;
    bool moving_right = false;
    float move_time = 3.0f;
    float move_timer = 0.0f;

    public LadybugWander(FiniteStateMachine fsm, EnemyLadybug actor) : base(fsm)
    {
        _actor = actor;
    }

    public override void FixedUpdate()
    {

        _actor.Horizontal_Movement(_actor.hor_move_axis);
        _actor.Vertical_Movement(false);
    }

    public override void OnEnter()
    {
        if (moving_right)
        {
            moving_right = false;
        }
        else
        {
            moving_right = true;
        }
        move_timer = 0.0f;
    }

    public override void OnExit()
    {
        _actor.hor_move_axis = 0.0f;
    }

    public override void Update()
    {
        if (moving_right)
        {
            _actor.hor_move_axis = 1.0f;
            if (!_actor.detector_A)
            {
                _fsm.ChangeState(_actor.state_idle);
                return;
            }
        }
        else
        {
            _actor.hor_move_axis = -1.0f;
            if (!_actor.detector_B)
            {
                _fsm.ChangeState(_actor.state_idle);
                return;
            }
        }

        move_timer += Time.deltaTime;
        if(move_timer >= move_time)
        {
            _fsm.ChangeState(_actor.state_idle);
            return;
        }
        //to stop it wandering off cliffs
        //if moving_right and detector_B is false
        //if moving_left and detector_A is false
    }
}