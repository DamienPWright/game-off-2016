using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System;

[Serializable]
public class AttackContainer
{

    public string first_normal_attack;
    public string first_air_normal_attack;
    public string first_special_attack;
    public string first_air_special_attack;
    public string first_down_special_attack;
    public string first_up_special_attack;
    public string first_forward_special_attack;

    public List<AttackElementContainer> attacks = new List<AttackElementContainer>();
}

[Serializable]
public class AttackElementContainer
{
    public int _animation;
    public bool _xinput_lock;
    public bool _jumpinput_lock;
    public float _internal_cooldown = 0.0f;
    public string _attackKey = "";
    public string _nextNormalAttackKey = "";
    public string _nextSpecialAttackKey = "";
    public string _nextUpSpecialAttackKey = "";
    public string _nextDownSpecialAttackKey = "";
    public string _nextForwardSpecialAttackKey = "";
    public int _hack_colour = 0;
    public bool _inflicts_knockback = false;
    public Vector2 _knockback_vector;
    public int hitstop_frames = 0;
    public int power = 0;
}