using UnityEngine;
using System.Collections;

public class BackGround : MonoBehaviour {

    public float lUnityPlaneSize = 10.0f; // 10 for a Unity3d plane
    void Update()
    {
        Camera lCamera = Camera.main;

        transform.position = new Vector3(lCamera.transform.position.x, lCamera.transform.position.y, transform.position.z);
    }
}
