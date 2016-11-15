using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class AttackManager
{

    int combo_counter = 0;
    Dictionary<string, Attack> attacks;
    string first_normal_attack;
    string first_air_normal_attack;
    string first_special_attack;
    string first_air_special_attack;
    string first_up_special_attack;
    string first_air_up_special_attack;
    string first_down_special_attack;
    string first_air_down_special_attack;
    string first_forward_special_attack;
    string first_air_forward_special_attack;
    Attack current_attack;

    public void Initialise(string filename)
    {
        attacks = new Dictionary<string, Attack>();

        //test for attack assembly from json
        AttackAssembler as_test = new AttackAssembler();
        as_test.getFileData(filename);

        first_normal_attack = as_test.getFirstNormalAttack();
        first_air_normal_attack = as_test.getFirstAirNormalAttack();
        first_special_attack = as_test.getFirstSpecialAttack();
        first_air_special_attack = as_test.getFirstAirSpecialAttack();

        first_up_special_attack = as_test.getFirstUpSpecialAttack();
        first_down_special_attack = as_test.getFirstDownSpecialAttack();
        first_forward_special_attack = as_test.getFirstForwardSpecialAttack();
        attacks = as_test.getAttacks();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (KeyValuePair<string, Attack> entry in attacks)
        {
            entry.Value.Update();
        }
    }

    void ComboHandler()
    {
        combo_counter++;
    }

    public void doNormalAttack()
    {
        if (combo_counter == 0)
        {
            current_attack = attacks[first_normal_attack];
        }
        else
        {
            current_attack = getAttackByKey(current_attack.NextAttackKey);
        }
        //Debug.Log("current attack: " + current_attack.AttackKey + " combo count: " + combo_counter);
        ComboHandler();
    }

    public void doAirNormalAttack()
    {
        if (combo_counter == 0)
        {

            current_attack = attacks[first_air_normal_attack];
        }
        else
        {
            current_attack = getAttackByKey(current_attack.NextAttackKey);
        }
        //Debug.Log("current air attack: " + current_attack.AttackKey + " combo count: " + combo_counter);
        ComboHandler();
    }

    public void doSpecialAttack()
    {

        //Attack new_attack = null;

        if (combo_counter == 0)
        {
            current_attack = getAttackByKey(first_special_attack);
        }
        else
        {
            current_attack = getAttackByKey(current_attack.NextSpecialAttackKey);
        }

        ComboHandler();
    }

    public void doAirSpecialAttack()
    {
        if (combo_counter == 0)
        {
            current_attack = getAttackByKey(first_air_special_attack);
        }
        else
        {
            current_attack = getAttackByKey(current_attack.NextSpecialAttackKey);
        }
        ComboHandler();
    }

    public void doUpSpecialAttack()
    {

        //Attack new_attack = null;

        if (combo_counter == 0)
        {
            current_attack = getAttackByKey(first_up_special_attack);
        }
        else
        {
            current_attack = getAttackByKey(current_attack.NextUpSpecialAttackKey);
        }

        ComboHandler();
    }

    public void doAirUpSpecialAttack()
    {
        if (combo_counter == 0)
        {
            current_attack = getAttackByKey(first_air_up_special_attack);
        }
        else
        {
            current_attack = getAttackByKey(current_attack.NextUpSpecialAttackKey);
        }
        ComboHandler();
    }

    public void doDownSpecialAttack()
    {

        //Attack new_attack = null;

        if (combo_counter == 0)
        {
            current_attack = getAttackByKey(first_down_special_attack);
        }
        else
        {
            current_attack = getAttackByKey(current_attack.NextUpSpecialAttackKey);
        }

        ComboHandler();
    }

    public void doAirDownSpecialAttack()
    {
        if (combo_counter == 0)
        {
            current_attack = getAttackByKey(first_air_down_special_attack);
        }
        else
        {
            current_attack = getAttackByKey(current_attack.NextDownSpecialAttackKey);
        }
        ComboHandler();
    }

    public void doForwardSpecialAttack()
    {

        //Attack new_attack = null;

        if (combo_counter == 0)
        {
            current_attack = getAttackByKey(first_forward_special_attack);
        }
        else
        {
            current_attack = getAttackByKey(current_attack.NextForwardSpecialAttackKey);
        }

        ComboHandler();
    }

    public void doAirForwardSpecialAttack()
    {
        if (combo_counter == 0)
        {
            current_attack = getAttackByKey(first_air_forward_special_attack);
        }
        else
        {
            current_attack = getAttackByKey(current_attack.NextForwardSpecialAttackKey);
        }
        ComboHandler();
    }


    //currently unusued but potentially useful...
    Attack getAttackByKey(string key)
    {
        Attack next_attack = null;

        try
        {
            next_attack = attacks[key];
        }
        catch (KeyNotFoundException e)
        {
            Debug.Log(e.Message);
        }

        return next_attack;
    }

    public int getHackColour()
    {
        return current_attack.HackColour;
    }


    public bool canDoANormalAttack()
    {
        if (combo_counter == 0)
        {
            if (attacks.ContainsKey(first_normal_attack))
            {
                return true;
            }
        }
        else if (current_attack.NextAttackKey != "")
        {
            if (attacks.ContainsKey(current_attack.NextAttackKey))
            {
                return true;
            };
        }

        return false;
    }

    public bool canDoAirNormalAttack()
    {
        if (combo_counter == 0)
        {
            if (attacks.ContainsKey(first_air_normal_attack))
            {
                return true;
            }
        }
        else if (current_attack.NextAttackKey != "")
        {
            if (attacks.ContainsKey(current_attack.NextAttackKey))
            {
                return true;
            };
        }

        return false;
    }

    public bool canDoASpecialAttack()
    {
        if (combo_counter == 0)
        {
            if (attacks.ContainsKey(first_special_attack))
            {
                return true;
            }
        }
        else if (current_attack.NextSpecialAttackKey != "")
        {
            if (attacks.ContainsKey(current_attack.NextSpecialAttackKey))
            {
                return true;
            };
        }

        return false;
    }

    public bool canDoAirSpecialAttack()
    {
        if (combo_counter == 0)
        {
            if (attacks.ContainsKey(first_air_special_attack))
            {
                return true;
            }
        }
        else if (current_attack.NextSpecialAttackKey != "")
        {
            if (attacks.ContainsKey(current_attack.NextSpecialAttackKey))
            {
                return true;
            };
        }

        return false;
    }

    public int getAttackAnim()
    {
        if (current_attack == null)
        {
            return 0;
        }

        return current_attack.animation;
    }

    public int getAttackPower()
    {
        if (current_attack == null)
        {
            return 0;
        }

        return current_attack.Power;
    }

    public bool isXinputLocked()
    {
        if (current_attack == null)
        {
            return false;
        }

        return current_attack.isXinputLocked;
    }

    public bool isJumpInputLocked()
    {
        if (current_attack == null)
        {
            return false;
        }

        return current_attack.isJumpInputLocked;
    }

    public int getHitStopFrames()
    {
        if (current_attack == null)
        {
            return 0;
        }

        return current_attack.Hitstop_frames;
    }

    public bool inflictsKnockback()
    {
        return current_attack.Inflicts_knockback;
    }

    public Vector2 getKnockbackVector()
    {
        return current_attack.Knockback_vector;
    }

    public void endAttack()
    {
        current_attack.EndAttack();
        //ComboHandler();
    }

    public void resetAttackCombo()
    {
        combo_counter = 0;
    }
}