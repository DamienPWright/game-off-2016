using UnityEngine;
using System.Collections;

public class HeartPickup : MonoBehaviour {

    int restored_health = 5;

    public AudioSource _audiosource;
    public SpriteRenderer _spriteRenderer;
    bool pickedup = false;

    void Start()
    {
        _audiosource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnPickUp(Player player)
    {
        if (pickedup)
        {
            return;
        }

        if (player.cur_health < player.max_health)
        {
            player.restoreHealth(restored_health);
            pickedup = true;
            _spriteRenderer.enabled = false;
            _audiosource.Play();
        }        
    }

    void Update()
    {
        if (pickedup)
        {
            if (!_audiosource.isPlaying)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
