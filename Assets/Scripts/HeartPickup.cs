using UnityEngine;
using System.Collections;

public class HeartPickup : MonoBehaviour {

    int restored_health = 5;

    public void OnPickUp(Player player)
    {
        if (player.cur_health < player.max_health)
        {
            player.restoreHealth(restored_health);
            gameObject.SetActive(false);
        }        
    }
}
