using UnityEngine;
using System.Collections;

public class CollectableBit : MonoBehaviour {

    public int points = 0;

    MonoBehaviour script = null;
    Animator _animator;
    BoxCollider2D _collider;
    AudioSource _audiosource;

    // Use this for initialization
    void Start () {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<BoxCollider2D>();
        _audiosource = GetComponent<AudioSource>();
	}
	
    public void Collect()
    {
        _animator.SetBool("collected", true);
        _collider.enabled = false;
        GameManager.UpdateScore(points);
        _audiosource.Play();
        //gameObject.SetActive(false);
    }
}
