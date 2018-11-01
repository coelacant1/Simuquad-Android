using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets;

//Used to calculate the current state of the quadcopter object
public class StateCalculation : MonoBehaviour {
    private Quadcopter quadcopter;
    private Rigidbody rb;
    public Joystick leftJoyStick;
    public Joystick rightJoyStick;
    private bool horizon = true;

    Controls controls = new Controls
    {
        Thrust = 0.2625,
        Pitch = 0.0,
        Yaw = 0.0,
        Roll = 0.0
    };

    // Use this for initialization
    void Start () {
        quadcopter = new Quadcopter(horizon);

        rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {

        controls.Thrust = leftJoyStick.Vertical;
        controls.Yaw    = horizon ? controls.Yaw + leftJoyStick.Horizontal  :  leftJoyStick.Horizontal  * 0.1;
        controls.Pitch  = horizon ? rightJoyStick.Vertical   * -90          :  rightJoyStick.Vertical   * 0.1;
        controls.Roll   = horizon ? rightJoyStick.Horizontal * -90          : -rightJoyStick.Horizontal * 0.1;

        if (controls.Thrust <= 0)
        {
            controls.Thrust = 0;
        }

        //Debug.Log(controls.Pitch + " " + controls.Yaw + " " + controls.Roll + " " + controls.Thrust);

        quadcopter.EstimateState(controls, Time.deltaTime);//DT ~= 0.016s
        
        rb.MovePosition(quadcopter.GetUnityPosition());
        rb.MoveRotation(quadcopter.GetUnityQuaternion());
    }

    public void ResetQuadcopter()
    {
        quadcopter.Reset();
    }

    public void SetMode()
    {
        if (horizon)
        {
            horizon = false;
        }
        else
        {
            horizon = true;
        }

        quadcopter = new Quadcopter(horizon);
    }
}
