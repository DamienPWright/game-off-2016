using UnityEngine;
using System.Collections;

public class LevelGoal : MonoBehaviour
{
    public bool level_cleared = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Actor"))
        {
            MonoBehaviour script = other.GetComponent<MonoBehaviour>();
            if(script is IAttackableActor)
            {
                if((script as IAttackableActor).GetIsPlayer())
                {
                    level_cleared = true;
                }
            }
            
        }
    }
}
