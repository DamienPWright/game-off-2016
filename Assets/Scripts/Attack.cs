using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using System;

[Serializable]
public class Attack
{
    [NonSerialized]
    AnimationMonitor _animation_monitor;

    int _animation;
    bool _xinput_lock;
    bool _jumpinput_lock;
    float _internal_cooldown = 0.0f;

    [NonSerialized]
    float _internal_cooldown_timer = 0.0f;
    [NonSerialized]
    bool _on_cooldown;

    string _attackKey = "";
    string _nextNormalAttackKey = "";
    string _nextSpecialAttackKey = "";
    string _nextUpSpecialAttackKey = "";
    string _nextDownSpecialAttackKey = "";
    string _nextForwardSpecialAttackKey = "";
    int _hackColour = 0;
    bool _inflicts_knockback = false;
    Vector2 _knockback_vector;
    int hitstop_frames = 0;
    int power = 0;

    public int animation
    {
        set { _animation = value; }
        get { return _animation; }
    }


    public bool isXinputLocked
    {
        set { _xinput_lock = value; }
        get { return _xinput_lock; }
    }


    public bool isJumpInputLocked
    {
        set { _jumpinput_lock = value; }
        get { return _jumpinput_lock; }
    }



    public bool isOnCooldown
    {
        set { _on_cooldown = value; }
        get { return _on_cooldown; }
    }



    public string AttackKey
    {
        set { _attackKey = value; }
        get
        {
            return _attackKey;
        }
    }

    public string NextAttackKey
    {
        set { _nextNormalAttackKey = value; }
        get
        {
            return _nextNormalAttackKey;
        }
    }

    public int Hitstop_frames
    {
        set { hitstop_frames = value; }
        get
        {
            return hitstop_frames;
        }
    }

    public int Power
    {
        set { power = value; }
        get
        {
            return power;
        }
    }

    public bool Inflicts_knockback
    {
        set { _inflicts_knockback = value; }
        get
        {
            return _inflicts_knockback;
        }
    }

    public Vector2 Knockback_vector
    {
        set { _knockback_vector = value; }
        get
        {
            return _knockback_vector;
        }
    }

    public string NextSpecialAttackKey
    {
        get
        {
            return _nextSpecialAttackKey;
        }

        set
        {
            _nextSpecialAttackKey = value;
        }
    }

    public string NextUpSpecialAttackKey
    {
        get
        {
            return _nextUpSpecialAttackKey;
        }

        set
        {
            _nextUpSpecialAttackKey = value;
        }
    }

    public string NextDownSpecialAttackKey
    {
        get
        {
            return _nextDownSpecialAttackKey;
        }

        set
        {
            _nextDownSpecialAttackKey = value;
        }
    }

    public string NextForwardSpecialAttackKey
    {
        get
        {
            return _nextForwardSpecialAttackKey;
        }

        set
        {
            _nextForwardSpecialAttackKey = value;
        }
    }

    public int HackColour
    {
        get
        {
            return _hackColour;
        }

        set
        {
            _hackColour = value;
        }
    }

    public Attack()
    {

    }

    // Update is called once per frame
    public void Update()
    {
        if (_on_cooldown)
        {
            _internal_cooldown += Time.deltaTime;
            if (_internal_cooldown >= _internal_cooldown_timer)
            {
                _internal_cooldown_timer = 0.0f;
                _on_cooldown = false;
            }
        }
    }

    public void EndAttack()
    {
        //set cooldowns etc
    }
}