using UnityEngine;
using System.Collections;

public class CameraFollow2D : MonoBehaviour
{

	//Written by Scott Kovacs via UnityAnswers.com; Oct 5th 2010
    //2DCameraFollow - Platformer Script
 
    float dampTime = 0.3f; //offset from the viewport center to fix damping
    private Vector3 velocity = Vector3.zero;
    [HideInInspector]
    public Transform target;

    public float fixedYPosition = 0;

    Camera cameraComp;
 

    void Awake() {
        cameraComp = GetComponent<Camera>();
    }

    void Update() {
        if(target) {
            Vector3 point = cameraComp.WorldToViewportPoint(target.position);
            Vector3 delta = target.position - cameraComp.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
            Vector3 destination = transform.position + delta;
         
            // Set this to the Y position you want the camera locked to
            destination.y = fixedYPosition;



            transform.position = Vector3.SmoothDamp(this.transform.position, destination, ref velocity, dampTime);
        }
    }

}
