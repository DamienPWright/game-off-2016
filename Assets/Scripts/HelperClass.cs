using UnityEngine;
using System.Collections;

public static class HelperClass {

	public enum HackColorIds
    {
        None=0,
        Red,
        Cyan,
        Purple,
        Blue,
        Yellow
    }


    public static float MoveBackForthSine(float movement_distance, float cycle_timer, float cycle_time)
    {
        return movement_distance * Mathf.Cos((cycle_timer * Mathf.PI / cycle_time)) * (Mathf.PI / cycle_time);
    }

    public static float MoveBackForthCosine(float movement_distance, float cycle_timer, float cycle_time)
    {
        return movement_distance * Mathf.Sin((cycle_timer * Mathf.PI / cycle_time)) * (Mathf.PI / cycle_time);
    }

    public static float MoveBackForthParabola(float movement_distance, float cycle_timer, float cycle_time)
    {
        //movement_distance = d
        //cycle_timer = c
        //cycle_time = t
        //d and t are constants
        //x = c*2/t
        //x = c

        //return movement_distance * Mathf.Pow((cycle_timer / cycle_time), 2);
        return movement_distance * ((2 * cycle_timer) / Mathf.Pow(cycle_time, 2));
    }
}
