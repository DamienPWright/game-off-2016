using UnityEngine;
using System.Collections;

public class MovingPlatform : Actor {

    public float movement_distance = 10.0f;
    public float cycle_time = 1.0f;
    public int movement_type = 0;
    public float cycle_timer = 0.0f;
    float xvel = 0.0f;
    float yvel = 0.0f;
    float initial_x = 0.0f;
    float dist = 0.0f;

	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {
        cycle_timer += Time.fixedDeltaTime;


        switch (movement_type)
        {
            case 0:
                xvel = 0.0f;
                yvel = 0.0f;
                break;
            case 1:
                xvel = HelperClass.MoveBackForthSine(movement_distance, cycle_timer, cycle_time);
                yvel = 0.0f;
                break;
            case 2:
                yvel = HelperClass.MoveBackForthSine(movement_distance, cycle_timer, cycle_time);
                xvel = 0.0f;
                break;
            case 3:
                yvel = HelperClass.MoveBackForthCosine(movement_distance, cycle_timer, cycle_time);
                xvel = HelperClass.MoveBackForthCosine(movement_distance, cycle_timer, cycle_time);
                break;
            case 4:
                yvel = HelperClass.MoveBackForthCosine(movement_distance, cycle_timer, cycle_time);
                xvel = -HelperClass.MoveBackForthCosine(movement_distance, cycle_timer, cycle_time);
                break;
            case 5:
                yvel = HelperClass.MoveBackForthCosine(movement_distance, cycle_timer, cycle_time);
                xvel = HelperClass.MoveBackForthSine(movement_distance, cycle_timer, cycle_time);
                break;
            case 6:
                yvel = HelperClass.MoveBackForthCosine(movement_distance, 2 * cycle_timer, cycle_time);
                xvel = HelperClass.MoveBackForthSine(movement_distance, cycle_timer, cycle_time);
                break;
            default:
                xvel = 0.0f;
                yvel = 0.0f;
                break;
        }
        
        //_rigidbody.MovePosition(new Vector2(xvel, _rigidbody.transform.position.y));
        _rigidbody.velocity = new Vector2(xvel, yvel);
        //Debug.Log(xvel);
    }
}
