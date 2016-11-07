using UnityEngine;
using System.Collections;

public class Enemy : Actor {

    protected override void Start()
    {
        base.Start();
        isEnemy = true;
    }
}
