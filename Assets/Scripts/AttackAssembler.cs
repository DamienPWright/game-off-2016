using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;
using System.Collections.Generic;

public class AttackAssembler
{

    string filename;
    string filedata;
    BinaryReader br;
    string path;

    AttackContainer attackContainer;

    public AttackAssembler()
    {
        path = Directory.GetCurrentDirectory() + "\\Assets\\Data";
    }

    public bool getFileData(string name)
    {
        try
        {
            StreamReader reader = new StreamReader(@path + "\\" + name, Encoding.Default);

            using (reader)
            {
                filedata = reader.ReadToEnd();
                reader.Close();
                assembleAttackContainer();
                Debug.Log("File Data: " + filedata);
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
    }

    void assembleAttackContainer()
    {
        attackContainer = JsonUtility.FromJson<AttackContainer>(filedata);
    }

   
    public string getFirstNormalAttack()
    {
        return attackContainer.first_normal_attack;
    }

    public string getFirstAirNormalAttack()
    {
        return attackContainer.first_air_normal_attack;
    }

    public string getFirstSpecialAttack()
    {
        return attackContainer.first_special_attack;
    }

    public string getFirstAirSpecialAttack()
    {
        return attackContainer.first_air_special_attack;
    }

    public string getFirstDownSpecialAttack()
    {
        return attackContainer.first_down_special_attack;
    }

    public string getFirstUpSpecialAttack()
    {
        return attackContainer.first_up_special_attack;
    }

    public string getFirstForwardSpecialAttack()
    {
        return attackContainer.first_forward_special_attack;
    }

    public Dictionary<string, Attack> getAttacks()
    {
        Dictionary<string, Attack> attacks = new Dictionary<string, Attack>();

        foreach (AttackElementContainer element in attackContainer.attacks)
        {
            Attack attack = new Attack();

            attack.animation = element._animation;
            attack.AttackKey = element._attackKey;
            attack.NextAttackKey = element._nextNormalAttackKey;
            attack.NextSpecialAttackKey = element._nextSpecialAttackKey;
            attack.NextUpSpecialAttackKey = element._nextUpSpecialAttackKey;
            attack.NextDownSpecialAttackKey = element._nextDownSpecialAttackKey;
            attack.NextForwardSpecialAttackKey = element._nextForwardSpecialAttackKey;

            attack.HackColour = element._hack_colour;

            attack.isXinputLocked = element._xinput_lock;
            attack.isJumpInputLocked = element._jumpinput_lock;

            attack.Hitstop_frames = element.hitstop_frames;
            attack.Inflicts_knockback = element._inflicts_knockback;
            attack.Knockback_vector = new Vector2(element._knockback_vector.x, element._knockback_vector.y);

            attack.Power = element.power;

            attacks.Add(attack.AttackKey, attack);
        }

        return attacks;
    }
}