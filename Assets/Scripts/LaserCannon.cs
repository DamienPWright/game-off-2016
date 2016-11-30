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
    float dist = 0.0f;
    public Transform laser_emitter;
    public GameObject Laserbeam;
    public SpriteRenderer LaserbeamSprite;
    public Transform LaserbeamTransform;
    public GameObject LaserbeamImpact;
    public SpriteRenderer LaserbeamImpactSprite;
    public Transform LaserBeamImpactTransform;
    public GameObject hurtbox_ref;

    public bool red_hack_active = false;
    public bool purple_hack_active = false;


    float laserbeamInitialLength;

    public void onHackBlue()
    {
        
    }

    public void onHackCyan()
    {
        
    }

    public void onHackPurple()
    {
       


        if (gameObject.layer == 8)
        {
            gameObject.layer = 9;
            Laserbeam.layer = 9;
            //_spriteRenderer.color = new Color(1, 0, 1);
            purple_hack_active = true;
            LaserbeamSprite.color = new Color(1, 0, 1);
            LaserbeamImpactSprite.color = new Color(1, 0, 1);
        }
        else
        {
            gameObject.layer = 8;
            Laserbeam.layer = 0;
            purple_hack_active = false;
            //_spriteRenderer.color = Color.white;
            if (red_hack_active)
            {
                LaserbeamSprite.color = Color.red;
                LaserbeamImpactSprite.color = Color.red;
            }
            else
            {
                LaserbeamSprite.color = Color.white;
                LaserbeamImpactSprite.color = Color.white;
            }
        }

        _audiosource.clip = sound_purplehack;
        _audiosource.Play();
    }

    public void onHackRed()
    {
        if (red_hack_active)
        {
            red_hack_active = false;
            LaserbeamSprite.color = Color.white;
            LaserbeamImpactSprite.color = Color.white;
        }
        else
        {
            red_hack_active = true;
            LaserbeamSprite.color = Color.red;
            LaserbeamImpactSprite.color = Color.red;
        }
        _audiosource.clip = sound_redhack;
        _audiosource.Play();
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        max_health = 5;
        cur_health = max_health;
        initial_x = transform.position.x;
        laserbeamInitialLength = 1;
    }

    float getDistanceToGround()
    {
        float distance = 0.0f;
        RaycastHit2D hit = Physics2D.Raycast(laser_emitter.position, -Vector3.up, 50.0f, LayerMask.GetMask("env_solid"));
        //Debug.DrawLine(laser_emitter.position, laser_emitter.position + Vector3.right, Color.cyan);
        //Debug.DrawRay(laser_emitter.position, -Vector3.up * dist, Color.green);
        //Debug.DrawLine(hit.transform.position, hit.transform.position + Vector3.right, Color.cyan);
        //Debug.DrawLine(hit.point, hit.point + Vector2.right, Color.cyan);
        //Debug.Log(transform.position.y + " " + laser_emitter.position.y);
        if(hit.collider != null)
        {
            distance = Mathf.Abs(hit.point.y - laser_emitter.position.y);
            LaserbeamImpactSprite.enabled = true;
            LaserBeamImpactTransform.position = hit.point;
            LaserbeamSprite.transform.localScale = new Vector3(LaserbeamSprite.transform.localScale.x,
                distance,
                LaserbeamSprite.transform.localScale.z);

            LaserbeamTransform.localPosition = new Vector3(
                    LaserbeamTransform.localPosition.x,
                    -distance / 2,
                    LaserbeamTransform.localPosition.z
                );
            
        }
        //Debug.Log("Beamlength:  " + laserbeamInitialLength + " * " + distance);
        return (distance);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (purple_hack_active)
        {
            return;
        }
        
        //Debug.Log("something entered trigger");
        if (other.gameObject.CompareTag("Attackable"))
        {
            MonoBehaviour script = other.gameObject.GetComponentInParent<MonoBehaviour>();

            if(script is Actor)
            {
                if((script as Actor).IsPlayer)
                {
                    damageTarget(script);
                }
                else if(red_hack_active)
                {
                    damageTarget(script);
                }
            }
        }
    }

    void damageTarget(MonoBehaviour script)
    {
        if(script is IAttackableActor)
        {
            (script as IAttackableActor).takeDamage(2);
            (script as IAttackableActor).knockBack(new Vector2(1, 1));
        }
    }

    // Update is called once per frame
    void Update()
    {
        dist = getDistanceToGround();
        
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
        //xvel = HelperClass.MoveBackForthSine(movement_distance, cycle_timer, cycle_time); //<- Differential of position
        //yvel = movement_distance * Mathf.Sin(2*(cycle_timer * Mathf.PI / cycle_time)) * (Mathf.PI / cycle_time);
        //xvel = movement_distance * 0.5f * Mathf.Cos((cycle_timer * Mathf.PI / cycle_time));

        if(cycle_timer >= cycle_time)
        {
            cycle_timer = 0;
            movement_distance = -movement_distance;
        }

        xvel = HelperClass.MoveBackForthParabola(movement_distance, cycle_timer, cycle_time);

        //_rigidbody.MovePosition(new Vector2(xvel, _rigidbody.transform.position.y));
        _rigidbody.velocity = new Vector2(xvel, yvel);
        //Debug.Log(xvel);
    }
}
