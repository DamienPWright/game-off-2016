using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public Player player;

    FiniteStateMachine game_state;
    public GameState_Gameplay state_gameplay;
    public GameState_GameOver state_gameover;
    public GameState_LevelClear state_levelclear;
    public LevelGoal level_goal;

    public Slider _health_slider;

    public MonoBehaviour GameOverText;
    public MonoBehaviour LevelClearText;
    public MonoBehaviour GameClearText;
    public Text ScoreText;

    public static int _score;
    public static int _score_at_start;

    public string[] levelnames;
    public static int current_level = 0;

    public AudioSource _audiosource;

    public static void UpdateScore(int score)
    {
        _score += score;
        //Debug.Log("Bits: " + _score + " time: " + Time.time);
        
    }

    public void ResetScore()
    {
        _score = 0;
    }

    // Use this for initialization
    void Start() {
        game_state = new FiniteStateMachine();

        state_gameplay = new GameState_Gameplay(this, game_state);
        state_gameover = new GameState_GameOver(this, game_state);
        state_levelclear = new GameState_LevelClear(this, game_state);

        levelnames = new string[2]
        {
            "TestLevelWithGraphics",
            "Level2"
        };

        game_state.ChangeState(state_gameplay);

        _score = _score_at_start;

        _audiosource = GetComponent<AudioSource>();
        if (!_audiosource.isPlaying)
        {
            _audiosource.Play();
        }
	}
	
	// Update is called once per frame
	void Update () {
        game_state.Update();
        _health_slider.value = (float)player.cur_health / player.max_health;
        ScoreText.text = "Bits: " + _score.ToString();
    }

    public void ReleasePlayerControls()
    {
        player.Jump_release();
        player.Up_released();
        player.Down_released();
        player.Forward_released();
        player.Special_released();
        player.Attack_release();
        player.Move(0.0f);
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
            return;
        }

        if (_gm.level_goal.level_cleared)
        {
            _fsm.ChangeState(_gm.state_levelclear);
            return;
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


public class GameState_LevelClear : FSM_State
{
    GameManager _gm;

    public GameState_LevelClear(GameManager gm, FiniteStateMachine fsm) : base(fsm)
    {
        _gm = gm;
    }

    public override void FixedUpdate()
    {
        //throw new NotImplementedException();
    }

    public override void OnEnter()
    {
        _gm.ReleasePlayerControls();
        _gm.player.fsm.ChangeState(_gm.player.state_clear);
        if (GameManager.current_level >= _gm.levelnames.Length - 1)
        {
            //Game complete!
            Debug.Log("GameClear!");
            _gm.GameClearText.enabled = true;
        }
        else
        {
            Debug.Log("LevelClear!");
            _gm.LevelClearText.enabled = true;
        }

        
    }

    public override void OnExit()
    {
        //throw new NotImplementedException();
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            GameManager.current_level++;
            _gm._audiosource.Stop();
            if(GameManager.current_level >= _gm.levelnames.Length)
            {
                GameManager.current_level = 0;
                GameManager._score_at_start = 0;
                SceneManager.LoadScene(_gm.levelnames[GameManager.current_level]);
            }
            else
            {
                GameManager._score_at_start = GameManager._score;
                SceneManager.LoadScene(_gm.levelnames[GameManager.current_level]);
            }
            Debug.Log(SceneManager.GetActiveScene().name);

        }
    }
}