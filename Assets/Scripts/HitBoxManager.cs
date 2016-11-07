using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HitBoxManager : MonoBehaviour
{

    public Actor owner;

    public PolygonCollider2D[] poly2dHitboxes;
    public CircleCollider2D[] circleHitboxes;
    public BoxCollider2D[] boxHitboxes;
    string[] _polynames;
    public string[] polynames;
    public string[] circlenames;
    public string[] boxnames;
    bool hitboxes_initialized = false;

    Dictionary<string, HitboxWrapper> __hitboxes;
    public HitboxWrapper current_hitbox_wrapper;
    public PolygonCollider2D active_poly_collider;
    public PolygonCollider2D local_polycollider;
    public CircleCollider2D active_circle_collider;
    public CircleCollider2D local_circlecollider;
    public BoxCollider2D active_box_collider;
    public BoxCollider2D local_boxcollider;
    public bool hitbox_activated = false;
    int hitbox_lifecounter = 0;
    int hitbox_life = 3; //how many frames the hitbox will persist for.

    // Use this for initialization
    void Start()
    {

        __hitboxes = new Dictionary<string, HitboxWrapper>();

        local_polycollider = gameObject.AddComponent<PolygonCollider2D>();
        local_polycollider.isTrigger = true;
        local_polycollider.enabled = false;
        local_polycollider.pathCount = 0;

        local_circlecollider = gameObject.AddComponent<CircleCollider2D>();
        local_circlecollider.isTrigger = true;
        local_circlecollider.enabled = false;
        local_circlecollider.offset = new Vector2(0, 0);
        local_circlecollider.radius = 0;

        local_boxcollider = gameObject.AddComponent<BoxCollider2D>();
        local_boxcollider.isTrigger = true;
        local_boxcollider.enabled = false;
        local_boxcollider.offset = new Vector2(0, 0);
        local_boxcollider.size = new Vector2(0, 0);

    }

    // Update is called once per frame
    void Update()
    {

        if (!hitboxes_initialized)
        {
            if ((polynames.Length > 0) || (circlenames.Length > 0) || (boxnames.Length > 0))
            {
                hitboxes_initialized = true;
                initializeHitboxDictionary();
            }
        }



        if (hitbox_lifecounter >= hitbox_life)
        {
            hitbox_activated = false;
            hitbox_lifecounter = 0;
            current_hitbox_wrapper.ClearHitbox();
        }
        if (hitbox_activated)
        {
            hitbox_lifecounter++;
            //Debug.Log("hitbox alive");
        }

    }

    void initializeHitboxDictionary()
    {
        //Debug.Log("polynames length: " + polynames.Length);
        for (int i = 0; i < poly2dHitboxes.Length; i++)
        {
            __hitboxes.Add(polynames[i], new PolygonHitboxWrapper(poly2dHitboxes[i], this));
            Debug.Log(polynames[i]);
        }

        for (int i = 0; i < circleHitboxes.Length; i++)
        {
            __hitboxes.Add(circlenames[i], new CircleHitboxWrapper(circleHitboxes[i], this));
            Debug.Log(circlenames[i]);
        }

        for (int i = 0; i < boxHitboxes.Length; i++)
        {
            __hitboxes.Add(boxnames[i], new BoxHitboxWrapper(boxHitboxes[i], this));
            Debug.Log(boxnames[i]);
            Debug.Log(__hitboxes[boxnames[i]]);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Attackable")
        {
            Debug.Log("Collider hit attackable");
            //there may be a better way to do this part... 
            MonoBehaviour script = other.GetComponentInParent<MonoBehaviour>();
            Debug.Log(script);
            if (script is IAttackableActor)
            {
                Debug.Log("Attackable Actor hit");
                Debug.Log(owner);
                (script as IAttackableActor).takeDamage(owner._attack_manager.getAttackPower());
                if (owner._attack_manager.inflictsKnockback())
                {
                    Vector2 vector = new Vector2(owner._attack_manager.getKnockbackVector().x, 
                        owner._attack_manager.getKnockbackVector().y);

                    if (!owner.IsFacingRight)
                    {
                        vector.x = -vector.x;
                    }

                    (script as IAttackableActor).knockBack(vector);
                }
                //(script as Actor).ApplyHitStop(owner._attack_manager.getHitStopFrames());
                //owner.ApplyHitStop(owner._attack_manager.getHitStopFrames());
            }

            if (script is IHackableActor)
            {
                Debug.Log("Hackable actor hit");
                Debug.Log("Hack color: " + owner._attack_manager.getHackColour());
                switch (owner._attack_manager.getHackColour())
                {
                    case (int)HelperClass.HackColorIds.Blue:
                        (script as IHackableActor).onHackBlue();
                        break;
                    case (int)HelperClass.HackColorIds.Red:
                        (script as IHackableActor).onHackRed();
                        break;
                    case (int)HelperClass.HackColorIds.Cyan:
                        (script as IHackableActor).onHackCyan();
                        break;
                    case (int)HelperClass.HackColorIds.Purple:
                        (script as IHackableActor).onHackPurple();
                        break;
                    case (int)HelperClass.HackColorIds.None:
                        break;
                }
            }
        }
    }

    public void SetCollider(string hitboxKey)
    {
        //Debug.Log(polynames.Length);
        try
        {
            current_hitbox_wrapper = __hitboxes[hitboxKey];
            current_hitbox_wrapper.SetHitbox();
        }
        catch (KeyNotFoundException)
        {
            Debug.Log("Key: " + hitboxKey + " was not found");
        }
    }

    public void ClearHitbox()
    {
        current_hitbox_wrapper.ClearHitbox();
    }
}

public abstract class HitboxWrapper
{
    public abstract void SetHitbox();
    public abstract void ClearHitbox();
}

public class PolygonHitboxWrapper : HitboxWrapper
{
    PolygonCollider2D _poly;
    HitBoxManager _hbm;

    public PolygonHitboxWrapper(PolygonCollider2D poly, HitBoxManager hbm)
    {
        _poly = poly;
        _hbm = hbm;
    }

    public override void SetHitbox()
    {
        _hbm.active_poly_collider = _poly; //needs to be made active_poly_collider or something
        _hbm.local_polycollider.SetPath(0, _hbm.active_poly_collider.GetPath(0)); //needs to be made local_poly_collider or something
        _hbm.local_polycollider.enabled = true;
        _hbm.hitbox_activated = true;
    }

    public override void ClearHitbox()
    {
        _hbm.local_polycollider.pathCount = 0;
        _hbm.local_polycollider.enabled = false;
    }
}

public class BoxHitboxWrapper : HitboxWrapper
{
    BoxCollider2D _box;
    HitBoxManager _hbm;

    public BoxHitboxWrapper(BoxCollider2D box, HitBoxManager hbm)
    {
        _box = box;
        _hbm = hbm;
    }

    public override void SetHitbox()
    {
        _hbm.active_box_collider = _box;
        _hbm.local_boxcollider.size = _box.size;
        _hbm.local_boxcollider.offset = _box.offset;
        _hbm.local_boxcollider.enabled = true;
        _hbm.hitbox_activated = true;
    }

    public override void ClearHitbox()
    {
        _hbm.local_boxcollider.enabled = false;
    }
}

public class CircleHitboxWrapper : HitboxWrapper
{
    CircleCollider2D _circle;
    HitBoxManager _hbm;

    public CircleHitboxWrapper(CircleCollider2D circle, HitBoxManager hbm)
    {
        _circle = circle;
        _hbm = hbm;
    }

    public override void SetHitbox()
    {
        _hbm.active_circle_collider = _circle;
        _hbm.local_circlecollider.radius = _circle.radius;
        _hbm.local_circlecollider.offset = _circle.offset;
        _hbm.local_circlecollider.enabled = true;
        _hbm.hitbox_activated = true;
    }

    public override void ClearHitbox()
    {
        _hbm.local_circlecollider.enabled = false;
    }
}