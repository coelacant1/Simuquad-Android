using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets;

//Used to calculate the current state of the quadcopter object
public class StateCalculation : MonoBehaviour {
    private Quadcopter quadcopter;
    private Rigidbody rb;

    Controls controls = new Controls
    {
        Thrust = 0.2625,
        Pitch = 0.0,
        Yaw = 0.0,
        Roll = 0.0
    };

    // Use this for initialization
    void Start () {
        quadcopter = new Quadcopter(true);

        rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {

        controls.Pitch  = Slider2DRight.value.y;
        controls.Yaw   += Slider2DLeft.value.x / 10;
        controls.Roll   = Slider2DRight.value.x;
        controls.Thrust = Slider2DLeft.value.y;

        quadcopter.EstimateState(controls, Time.deltaTime);//DT ~= 0.016s
        
        rb.MovePosition(quadcopter.GetUnityPosition());
        rb.MoveRotation(quadcopter.GetUnityQuaternion());
    }
}
