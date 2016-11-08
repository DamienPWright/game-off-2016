﻿using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public Player player;

    FiniteStateMachine game_state;
    public GameState_Gameplay state_gameplay;
    public GameState_GameOver state_gameover;

    public MonoBehaviour GameOverText;
    
    // Use this for initialization
	void Start () {
        game_state = new FiniteStateMachine();

        state_gameplay = new GameState_Gameplay(this, game_state);
        state_gameover = new GameState_GameOver(this, game_state);

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

public class GameState_Gameplay : FSM_State {

    GameManager _gm;

    public GameState_Gameplay(GameManager gm, FiniteStateMachine fsm) : base(fsm)
    {
        _gm = gm;
    }

    public override void FixedUpdate()
    {
        //throw new NotImplementedException();
    }

    public override void OnEnter()
    {
        Debug.Log("Gameplay state entered");
    }

    public override void OnExit()
    {
        //throw new NotImplementedException();
    }

    public override void Update()
    {
        _gm.ProcessPlayerControls();
        if (_gm.player.is_dead)
        {
            _fsm.ChangeState(_gm.state_gameover);
        }
    }
}



public class GameState_GameOver : FSM_State
{
    GameManager _gm;

    public GameState_GameOver(GameManager gm, FiniteStateMachine fsm) : base(fsm)
    {
        _gm = gm;
    }

    public override void FixedUpdate()
    {
        //throw new NotImplementedException();
    }

    public override void OnEnter()
    {
        Debug.Log("Gameover!");
        _gm.GameOverText.enabled = true;
    }

    public override void OnExit()
    {
        //throw new NotImplementedException();
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}