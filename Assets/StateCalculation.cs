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

        controls.Thrust = MathExtension.Exponential(leftJoyStick.Vertical, 1.0, 2.0) * 1.0;
        controls.Yaw    = horizon ? controls.Yaw + MathExtension.Exponential(leftJoyStick.Horizontal, 1.0, 3.0) * 4.0 : MathExtension.Exponential(leftJoyStick.Horizontal, 1.0, 3.0)   * 5.0;
        controls.Pitch  = horizon ? MathExtension.Exponential(rightJoyStick.Vertical, 1.0, 2.0)   * -45.0             : MathExtension.Exponential(rightJoyStick.Vertical, 1.0, 2.0)    * 5.0;
        controls.Roll   = horizon ? MathExtension.Exponential(rightJoyStick.Horizontal, 1.0, 2.0) * -45.0             : MathExtension.Exponential(-rightJoyStick.Horizontal, 1.0, 2.0) * 5.0;

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
            //Camera.main.transform.rotation.eulerAngles.Set(-45, 0, 0);
            horizon = false;
        }
        else
        {
            //Camera.main.transform.rotation.eulerAngles.Set(0, 0, 0);
            horizon = true;
        }

        quadcopter = new Quadcopter(horizon);
    }
}
