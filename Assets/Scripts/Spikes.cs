using UnityEngine;
using System.Collections;
using System;

public class Spikes : MonoBehaviour, IHackableActor
{

    MonoBehaviour script;

    //Hackable Actor stuff.
    public ParticleSystem redhack_particles;
    public ParticleSystem bluehack_particles;
    public ParticleSystem cyanhack_particles;
    public ParticleSystem purplehack_particles;

    public BoxCollider2D solidbox;
    public SpriteRenderer spikeColor;

    ParticleSystem.EmissionModule red_emitter;
    ParticleSystem.EmissionModule blue_emitter;
    ParticleSystem.EmissionModule cyan_emitter;
    ParticleSystem.EmissionModule purple_emitter;

    bool redhack_active = false;
    bool bluehack_active = false;
    bool cyanhack_active = false;
    bool purplehack_active = false;

    public AudioClip sound_redhack;
    public AudioClip sound_bluehack;
    public AudioClip sound_cyanhack;
    public AudioClip sound_purplehack;

    public AudioSource _audiosource;

    // Use this for initialization
    void Start()
    {
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

        _audiosource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        spikeCollision(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        spikeCollision(collision);
    }

    void spikeCollision(Collision2D collision)
    {
        if (cyanhack_active)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Actor"))
        {
            script = collision.gameObject.GetComponentInParent<MonoBehaviour>();
            if (script is IAttackableActor)
            {
                (script as IAttackableActor).knockBack(new Vector2(10.0f, 10.0f));
                (script as IAttackableActor).takeDamage(4);
            }
        }
    }

    public void onHackRed()
    {
        
    }

    public void onHackBlue()
    {
        //throw new NotImplementedException();
    }

    public void onHackCyan()
    {
        red_emitter.enabled = false;
        blue_emitter.enabled = false;
        purple_emitter.enabled = false;
        redhack_active = false;
        bluehack_active = false;
        purplehack_active = false;

        if (cyanhack_active)
        {
            cyanhack_active = false;
            spikeColor.color = Color.white;
        }
        else
        {
            cyanhack_active = true;
            spikeColor.color = Color.cyan;
        }

        onHack();
        _audiosource.clip = sound_cyanhack;
        _audiosource.Play();
    }


    public void onHackPurple()
    {
        red_emitter.enabled = false;
        blue_emitter.enabled = false;
        cyan_emitter.enabled = false;
        redhack_active = false;
        bluehack_active = false;
        cyanhack_active = false;

        if (purplehack_active)
        {
            purplehack_active = false;
            spikeColor.color = Color.white;
        }
        else
        {
            purplehack_active = true;
            spikeColor.color = new Color(0.6f, 0.0f, 0.6f);
        }

        onHack();

        _audiosource.clip = sound_purplehack;
        _audiosource.Play();

    }

    void onHack()
    {
        purple_emitter.enabled = purplehack_active;
        cyan_emitter.enabled = cyanhack_active;
        solidbox.enabled = !purplehack_active;
    }
}
