using UnityEngine;
using System.Collections;
using System;

public class Crate : Actor, IHackableActor, IAttackableActor {

    public BoxCollider2D spikebox;
    public BoxCollider2D solidbox;

    public int initialHack = 0;

    public bool is_bouncy = false;
    bool spikes_out = false;

    public void knockBack(Vector2 knockback)
    {
        Debug.Log("Knockback");
        _rigidbody.AddForce(knockback, ForceMode2D.Impulse);
    }

    public bool GetIsPlayer()
    {
        return false;
    }

    public bool GetIsEnemy()
    {
        return false;
    }

    public void onHackBlue()
    {
        if (is_bouncy)
        {
            is_bouncy = false;
        }
        else
        {
            is_bouncy = true;
        }
        blue_emitter.enabled = is_bouncy;
        //_spriteRenderer.color = new Color(0, 0, 1.0f);
    }

    public void onHackCyan()
    {
        
    }

    public void onHackPurple()
    {
        if (gameObject.layer == 8)
        {
            gameObject.layer = 9;
            purple_emitter.enabled = true;
        }
        else
        {
            gameObject.layer = 8;
            purple_emitter.enabled = false;
        }
        
        //_spriteRenderer.color = new Color(0.7f, 0, 0.7f);
    }

    public void onHackRed()
    {
        if (spikes_out)
        {
            spikes_out = false;
        }
        else
        {
            spikes_out = true;
        }
        red_emitter.enabled = spikes_out;
        spikebox.enabled = spikes_out;
        
        //_spriteRenderer.color = new Color(1.0f, 0, 0);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!spikes_out)
        {
            return;
        }
        if (other.CompareTag("Attackable"))
        {
            Debug.Log("Attackable entered trigger");
            MonoBehaviour script = other.GetComponentInParent<MonoBehaviour>();
            if(script is IAttackableActor)
            {
                (script as IAttackableActor).knockBack(new Vector2(6.0f, 6.0f));
                (script as IAttackableActor).takeDamage(3);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!is_bouncy)
        {
            return;
        }
        if (collision.gameObject.CompareTag("Actor"))
        {
            MonoBehaviour script = collision.gameObject.GetComponentInParent<MonoBehaviour>();
            if (script is Actor)
            {
                    (script as Actor).BounceActor(new Vector2(-collision.contacts[0].normal.x * 5, 
                        -collision.contacts[0].normal.y * 15));
                    //Debug.Log(collision.contacts[0].normal.x + " " + collision.contacts[0].normal.y);
            }
        }
    }

    public void takeDamage(int damage)
    {
        Debug.Log("Crate took damage");
    }

    protected override void Start()
    {
        

        base.Start();

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
}
