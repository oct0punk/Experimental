using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public static CameraScript Instance;
    public Transform target;
    public Vector3 offset = Vector3.zero;
    public int sensitivity = 2;

    private int distance = 6;
    private float rotationY = 0;
    private float rotationX = 0;

    private void Start()
    {
        Instance = this;
        transform.LookAt(target, Vector3.up);
    }

    private void Update()
    {
        OrbitalCam();
    }

    // Update is called once per frame
    void OrbitalCam()
    {
        // Defines camera rotation
        rotationX -= Input.GetAxis("MouseY") * sensitivity;
        rotationX = Mathf.Clamp(rotationX, -90, 90);
        rotationY += Input.GetAxis("MouseX") * sensitivity;
        Vector3 direction = new Vector3(rotationX, rotationY, 0);

        // Zoom
        distance -= (int)Input.mouseScrollDelta.y;
        distance = Mathf.Clamp(distance, 1, 100);
        
        
        Vector3 position = target.position 
            - (Quaternion.Euler(direction) *  Vector3.forward * distance)   // Takes account the camera rotation
            + (Quaternion.Euler(0, rotationY, 0) * offset);                 // Takes account the offset

        transform.SetPositionAndRotation( position, Quaternion.Euler(direction));
    }
}
