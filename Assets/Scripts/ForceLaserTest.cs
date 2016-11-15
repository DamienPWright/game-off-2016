using UnityEngine;
using System.Collections;
using System;

public class ForceLaserTest : MonoBehaviour, IHackableActor {

    //Hackable Actor stuff.
    public int initialHack = 0;
    public ParticleSystem redhack_particles;
    public ParticleSystem bluehack_particles;
    public ParticleSystem cyanhack_particles;
    public ParticleSystem purplehack_particles;

    public BoxCollider2D beamBox;
    public SpriteRenderer beamRenderer;

    ParticleSystem.EmissionModule red_emitter;
    ParticleSystem.EmissionModule blue_emitter;
    ParticleSystem.EmissionModule cyan_emitter;
    ParticleSystem.EmissionModule purple_emitter;

    bool redhack_active = false;
    bool bluehack_active = false;
    bool cyanhack_active = false;
    bool purplehack_active = false;

    // Use this for initialization
    void Start () {
        if (redhack_particles != null)
        {
            red_emitter = redhack_particles.emission;
            red_emitter.enabled = false;
        }
        if (bluehack_particles != null)
        {
            blue_emitter = bluehack_particles.emission;
            blue_emitter.enabled = false;
        }
        if (cyanhack_particles != null)
        {
            cyan_emitter = cyanhack_particles.emission;
            cyan_emitter.enabled = false;
        }
        if (purplehack_particles != null)
        {
            purple_emitter = purplehack_particles.emission;
            purple_emitter.enabled = false;
        }

        switch (initialHack)
        {
            case 0:
                break;
            case 1:
                onHackRed();
                break;
            case 2:
                onHackBlue();
                break;
            case 3:
                onHackCyan();
                break;
            case 4:
                onHackPurple();
                break;
            default:
                break;
        }
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnTriggerStay2D(Collider2D other)
    {
        if (purplehack_active)
        {
            return;
        }

        //Debug.Log("Something is in beam");
        if (other.CompareTag("Actor"))
        {
            Debug.Log("Actor is in beam");
        }
        Rigidbody2D rb = other.gameObject.GetComponentInParent<Rigidbody2D>();
        //Debug.Log(rb);
        if(rb != null)
        {
            if (bluehack_active)
            {
                rb.velocity = new Vector2(rb.velocity.x, -10.0f);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, 10.0f);
            }
            
        }
    }

    public void onHackRed()
    {
        throw new NotImplementedException();
    }

    public void onHackBlue()
    {
        redhack_active = false;
        cyanhack_active = false;
        purplehack_active = false;

        if (bluehack_active)
        {
            bluehack_active = false;
        }
        else
        {
            bluehack_active = true;
        }

        onHack();
    }

    public void onHackCyan()
    {
        redhack_active = false;
        bluehack_active = false;
        purplehack_active = false;

        if (cyanhack_active)
        {
            cyanhack_active = false;
        }
        else
        {
            cyanhack_active = true;
        }

        onHack();
    }

    public void onHackPurple()
    {
        redhack_active = false;
        bluehack_active = false;
        cyanhack_active = false;

        if (purplehack_active)
        {
            purplehack_active = false;
        }
        else
        {
            purplehack_active = true;
        }

        onHack();
    }

    void onHack()
    {
        blue_emitter.enabled = bluehack_active;
        cyan_emitter.enabled = cyanhack_active;
        purple_emitter.enabled = purplehack_active;
        red_emitter.enabled = redhack_active;

        beamBox.isTrigger = !cyanhack_active;

        if (cyanhack_active)
        {
            beamBox.gameObject.layer = 8;
        }
        else
        {
            beamBox.gameObject.layer = 0;
        }

        if (bluehack_active)
        {
            beamRenderer.color = Color.blue;
        }else if (cyanhack_active)
        {
            beamRenderer.color = Color.cyan;
        }else if (purplehack_active)
        {
            beamRenderer.color = new Color(0.6f, 0f, 0.6f);
        }else if (redhack_active)
        {
            beamRenderer.color = Color.red;
        }
        else
        {
            beamRenderer.color = Color.white;
        }
    }
}
