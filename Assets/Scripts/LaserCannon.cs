using UnityEngine;
using System.Collections;
using System;

public class LaserCannon : Enemy, IHackableActor {

    public float movement_distance = 10.0f;
    public float cycle_time = 1.0f;
    float cycle_timer = 0.0f;
    float xvel = 0.0f;
    float yvel = 0.0f;
    float initial_x = 0.0f;

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
        throw new NotImplementedException();
    }

    public void onHackRed()
    {
        throw new NotImplementedException();
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        max_health = 5;
        cur_health = max_health;
        initial_x = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        cycle_timer += Time.fixedDeltaTime;
        /*
        if(cycle_timer > cycle_time)
        {
            cycle_timer = 0.0f;
            movement_distance = -movement_distance;
        }
        */
        //xvel = movement_distance * Mathf.Sin((cycle_timer / cycle_time) * Mathf.PI);
        //xvel = movement_distance * (float)Math.Pow(Mathf.Sin((cycle_timer / cycle_time)), 2f);
        //xvel = movement_distance * Mathf.Sin((cycle_timer * Mathf.PI / cycle_time)); <- Position
        xvel = movement_distance * Mathf.Cos((cycle_timer * Mathf.PI / cycle_time)) * (Mathf.PI / cycle_time); //<- Differential of position
        yvel = movement_distance * Mathf.Sin(2*(cycle_timer * Mathf.PI / cycle_time)) * (Mathf.PI / cycle_time);
        //xvel = movement_distance * 0.5f * Mathf.Cos((cycle_timer * Mathf.PI / cycle_time));

        //_rigidbody.MovePosition(new Vector2(initial_x + xvel, _rigidbody.transform.position.y));
        _rigidbody.velocity = new Vector2(xvel, yvel);
        //Debug.Log(xvel);
    }
}
