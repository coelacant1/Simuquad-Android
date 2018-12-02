using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets;
using UnityEngine.UI;

//Used to calculate the current state of the quadcopter object
public class StateCalculation : MonoBehaviour {
    private Quadcopter quadcopter;
    private Rigidbody rb;
    public Slider renderSlider;
    public Slider dTSlider;
    public Joystick leftJoyStick;
    public Joystick rightJoyStick;
    private bool horizon = true;
    private bool pause = false;
    private bool wasPaused = false;
    private bool calculate = true;
    private double DTModifier = 1.0;

    public GameObject HUD;
    public GameObject Settings;
    public GameObject PausedHUD;
    public GameObject CrashedHUD;
    public Camera CameraFPV;

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
	void FixedUpdate () {
        if (!pause && calculate)
        {
            controls.Thrust = MathExtension.Exponential(leftJoyStick.Vertical, 1.0, 2.0) * 1.0 + 1.0;
            controls.Yaw = horizon ? controls.Yaw + MathExtension.Exponential(leftJoyStick.Horizontal, 1.0, 3.0) * 4.0 : MathExtension.Exponential(leftJoyStick.Horizontal, 1.0, 3.0) * 7.5;
            controls.Pitch = horizon ? MathExtension.Exponential(rightJoyStick.Vertical, 1.0, 2.5) * -90.0 : MathExtension.Exponential(rightJoyStick.Vertical, 1.0, 2.5) * 7.5;
            controls.Roll = horizon ? MathExtension.Exponential(rightJoyStick.Horizontal, 1.0, 2.5) * -90.0 : MathExtension.Exponential(-rightJoyStick.Horizontal, 1.0, 2.5) * 7.5;

            if (controls.Thrust <= 0)
            {
                controls.Thrust = 0;
            }

            //Debug.Log(controls.Pitch + " " + controls.Yaw + " " + controls.Roll + " " + controls.Thrust);

            quadcopter.EstimateState(controls, Time.deltaTime * DTModifier);//DT ~= 0.016s

            //rb.MovePosition(quadcopter.GetUnityPosition());
            InterpolateMovePosition();
            rb.MoveRotation(quadcopter.GetUnityQuaternion());
        }
        else if (pause)
        {
            rb.MovePosition(quadcopter.GetUnityPosition());
            rb.MoveRotation(quadcopter.GetUnityQuaternion());
        }
    }

    void InterpolateMovePosition()
    {
        Vector3 position = rb.position;

        for (int i = 0; i < 5; i++)
        {
            rb.MovePosition(Vector3.Lerp(position, quadcopter.GetUnityPosition(), i / 5.0f));
        }

        rb.MovePosition(quadcopter.GetUnityPosition());
    }

    void OnCollisionEnter(Collision collision)
    {
        CrashedHUD.SetActive(true);
        //switch to unity's physics engine
        calculate = false;
        rb.useGravity = true;
        rb.velocity = quadcopter.GetUnityVelocity();
        rb.angularVelocity = quadcopter.GetUnityAngularVelocity();
    }

    public void ResetQuadcopter()
    {
        CrashedHUD.SetActive(false);
        quadcopter = new Quadcopter(horizon);//.Reset();

        rb.useGravity = false;
        rb.velocity = new Vector3(0, 0, 0);
        rb.angularVelocity = new Vector3(0, 0, 0);
        calculate = true;
    }

    public void SetMode()
    {
        if (horizon)
        {
            CameraFPV.transform.localEulerAngles = new Vector3(-45, 0, 0);
            horizon = false;
            GameObject.Find("Mode").GetComponentInChildren<Text>().text = "Mode: Acrobatics";
        }
        else
        {
            CameraFPV.transform.localEulerAngles = new Vector3(0, 0, 0);
            horizon = true;
            GameObject.Find("Mode").GetComponentInChildren<Text>().text = "Mode: Horizon";
        }

        quadcopter = new Quadcopter(horizon);
    }

    public void DisplaySettings()
    {
        if (pause)
        {
            wasPaused = true;
        }
        else
        {
            pause = true;
        }

        //Hide HUD and Pause
        HUD.SetActive(false);
        Settings.SetActive(true);
    }

    public void HideSettings()
    {
        if (!wasPaused)
        {
            pause = false;
        }

        HUD.SetActive(true);
        Settings.SetActive(false);
    }

    public void Pause()
    {
        if (pause)
        {
            pause = false;
            PausedHUD.SetActive(false);
        }
        else
        {
            pause = true;
            PausedHUD.SetActive(true);
        }
    }

    public void UpdateRenderDistance()
    {
        CameraFPV.farClipPlane =  renderSlider.value;
    }

    public void UpdateDTModifier()
    {
        DTModifier = dTSlider.value / 9.0;

        ResetQuadcopter();
    }
}
