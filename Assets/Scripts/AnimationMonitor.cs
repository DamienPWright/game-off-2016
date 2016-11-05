using UnityEngine;
using System.Collections;

public class AnimationMonitor : MonoBehaviour
{

    bool animation_complete = false;
    bool interruptable = false;
    bool super_armor = false;
    bool i_frames = false;
    bool guard_frames = false;
    bool parry_frames = false;

    public Actor _actor;

    public void reset()
    {
        //Should be caled at the start of a state to ensure that the animation monitor can do its job properly
        //Be aware that animations with transition times may not work with this; the previous animation's
        //complete state may trigger AFTER this has been reset, which will cause the next animation
        //to end prematurely.
        animation_complete = false;
        interruptable = false;
        super_armor = false;
        i_frames = false;
        guard_frames = false;
        parry_frames = false;
    }

    public bool isAnimationComplete()
    {
        return animation_complete;
    }

    public bool isAnimCompleteAndReset()
    {
        if (animation_complete)
        {
            reset();
        }
        return animation_complete;
    }

    public void setComplete()
    {
        animation_complete = true;
    }

    public void setInterruptable()
    {
        interruptable = true;
    }

    public bool isInterruptable()
    {
        return interruptable;
    }

    public void setParrying(bool parry)
    {
        parry_frames = parry;
    }

    public bool isParryFrames()
    {
        return parry_frames;
    }

    public void setGuarding(bool guard)
    {
        guard_frames = guard;
    }

    public bool isGuarding()
    {
        return guard_frames;
    }


    //Movement
    public void applyImpulse(int index)
    {
        if (_actor != null)
        {
            _actor.applyImpulse(index);
        }
    }

    public void applyFacingImpulse(int index)
    {
        if (_actor != null)
        {
            _actor.applyControlledImpulse(index);
        }
    }


    public void animationDebugMessage(string msg)
    {
        //Debug.Log(msg);
    }
}