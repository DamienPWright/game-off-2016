using UnityEngine;

public interface IAttackableActor
{
    void takeDamage(int damage);
    void knockBack(Vector2 knockback);
}