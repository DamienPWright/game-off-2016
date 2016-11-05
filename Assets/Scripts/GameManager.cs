using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour {

    public Player player;

    FiniteStateMachine game_state;
    State_Gameplay state_gameplay;
    
    // Use this for initialization
	void Start () {
        game_state = new FiniteStateMachine();

        state_gameplay = new State_Gameplay(this, game_state);

        game_state.ChangeState(state_gameplay);
	}
	
	// Update is called once per frame
	void Update () {
        game_state.Update();
	}

    public void ProcessPlayerControls()
    {
        if (Input.GetKeyDown("space"))
        {
            player.Jump();
        }

        if (Input.GetKeyUp("space"))
        {
            player.Jump_release();
        }

        if(Input.GetAxisRaw("Vertical") == 1)
        {
            player.Up_pressed();
        }
        else
        {
            player.Up_released();
        }

        if(Input.GetAxisRaw("Vertical") == -1)
        {
            player.Down_pressed();
        }
        else
        {
            player.Down_released();
        }

        if(Input.GetAxisRaw("Horizontal") == 1 && player.IsFacingRight)
        {
            player.Forward_pressed();
        }
        else if (Input.GetAxisRaw("Horizontal") == -1 && !player.IsFacingRight)
        {
            player.Forward_pressed();
        }
        else
        {
            player.Forward_released();
        }

        if (Input.GetButtonDown("Fire1"))
        {
            player.Attack();
        }

        if (Input.GetButtonUp("Fire1"))
        {
            player.Attack_release();
        }

        if (Input.GetButtonDown("Fire2"))
        {
            player.Special_pressed();
        }

        if (Input.GetButtonUp("Fire2"))
        {
            player.Special_released();
        }

        player.Move(Input.GetAxisRaw("Horizontal"));
    }
}

public class State_Gameplay : FSM_State {

    GameManager _gm;

    public State_Gameplay(GameManager gm, FiniteStateMachine fsm) : base(fsm)
    {
        _gm = gm;
    }

    public override void FixedUpdate()
    {
        throw new NotImplementedException();
    }

    public override void OnEnter()
    {
        Debug.Log("Gameplay state entered");
    }

    public override void OnExit()
    {
        throw new NotImplementedException();
    }

    public override void Update()
    {
        _gm.ProcessPlayerControls();
    }
}
