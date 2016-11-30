using UnityEngine;
using System.Collections;

public class Enemy : Actor {

    public AudioClip sound_redhack;
    public AudioClip sound_bluehack;
    public AudioClip sound_cyanhack;
    public AudioClip sound_purplehack;


    protected override void Start()
    {
        base.Start();
        isEnemy = true;
    }
}
