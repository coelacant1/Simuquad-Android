using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

//Used to calculate the current state of the quadcopter object
public class StateCalculation : MonoBehaviour {
    private Quadcopter quadcopter;
    public Rigidbody rb;
    public Slider renderSlider;
    public Slider dTSlider;
    public Slider camSlider;
    public Slider worldSlider;
    public Slider dragSlider;
    public Joystick leftJoyStick;
    public Joystick rightJoyStick;
    public InputField TMass;
    public InputField TGravity;
    public InputField TTWRatio;
    public InputField TAcroRate;
    public InputField TAcroExpo;
    public InputField THoriExpo;

    public InputField THorizonPitchP;
    public InputField THorizonPitchI;
    public InputField THorizonPitchD;
    
    public InputField THorizonRollP;
    public InputField THorizonRollI;
    public InputField THorizonRollD;

    public InputField THorizonYawP;
    public InputField THorizonYawI;
    public InputField THorizonYawD;


    public InputField TAcrobaticsPitchP;
    public InputField TAcrobaticsPitchI;
    public InputField TAcrobaticsPitchD;

    public InputField TAcrobaticsRollP;
    public InputField TAcrobaticsRollI;
    public InputField TAcrobaticsRollD;

    public InputField TAcrobaticsYawP;
    public InputField TAcrobaticsYawI;
    public InputField TAcrobaticsYawD;


    private bool horizon = true;
    private bool pause = false;
    private bool wasPaused = false;
    private bool calculate = true;
    private double dtModifier = 1.0;
    private double mass = 0.5;
    private double worldScale = 0.5;
    private double cameraAngle = 45;
    private double drag = 0.6;
    private double acroExpo = 2.5;
    private double acroRate = 7.5;
    private double horizonExpo = 1.0;
    private double gravity = -9.81;
    private double twRatio = 10.0;
    private double renderDistance = 1000;
    private VectorPID horizonPID;
    private VectorPID acrobaticsPID;

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
    void Start ()
    {
        Application.targetFrameRate = 60;
        horizonPID = new VectorPID(1.0, 0.25, 0.09, Time.deltaTime);//initialized here for deltatime
        acrobaticsPID = new VectorPID(10.0, 8.0, 0.0, Time.deltaTime);

        quadcopter = new Quadcopter(horizon);

        LoadParameters();
        SetQuadcopterParameters();

        rb.useGravity = false;
        rb.velocity = new Vector3(0, 0, 0);
        rb.angularVelocity = new Vector3(0, 0, 0);
        rb.mass = (float)mass;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!pause && calculate)
        {
            controls.Thrust = MathExtension.Exponential(leftJoyStick.Vertical, 1.0, 2.0) + 1.0;
            controls.Yaw = horizon ? controls.Yaw + MathExtension.Exponential(leftJoyStick.Horizontal, 1.0, horizonExpo) * 4.0 : MathExtension.Exponential(leftJoyStick.Horizontal, 1.0, acroExpo) * acroRate;
            controls.Pitch = horizon ? MathExtension.Exponential(rightJoyStick.Vertical, 1.0, horizonExpo) * -90.0 : MathExtension.Exponential(rightJoyStick.Vertical, 1.0, acroExpo) * acroRate;
            controls.Roll = horizon ? MathExtension.Exponential(rightJoyStick.Horizontal, 1.0, horizonExpo) * -90.0 : MathExtension.Exponential(-rightJoyStick.Horizontal, 1.0, acroExpo) * acroRate;

            if (controls.Thrust <= 0)
            {
                controls.Thrust = 0;
            }

            //Debug.Log(controls.Pitch + " " + controls.Yaw + " " + controls.Roll + " " + controls.Thrust);

            quadcopter.EstimateState(controls, Time.deltaTime * dtModifier);//DT ~= 0.016s

            rb.MovePosition(quadcopter.GetUnityPosition());
            //InterpolateMovePosition();
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
        rb.mass = (float)mass;
    }

    public void ResetQuadcopter()
    {
        CrashedHUD.SetActive(false);
        quadcopter = new Quadcopter(horizon);//.Reset();
        SetQuadcopterParameters();

        rb.useGravity = false;
        rb.velocity = new Vector3(0, 0, 0);
        rb.angularVelocity = new Vector3(0, 0, 0);
        calculate = true;
    }

    public void SetMode()
    {
        if (horizon)
        {
            horizon = false;
            GameObject.Find("Mode").GetComponentInChildren<Text>().text = "Mode: Acrobatics";
        }
        else
        {
            horizon = true;
            GameObject.Find("Mode").GetComponentInChildren<Text>().text = "Mode: Horizon";
        }

        ResetQuadcopter();
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

        //Writes currents parameters to UI
        DisplayParameters();
    }

    public void HideSettings()
    {
        if (!wasPaused)
        {
            pause = false;
        }

        HUD.SetActive(true);
        Settings.SetActive(false);

        //Reads from UI and Saves to file
        ReadParameters();
        SaveParameters();
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
        renderDistance = renderSlider.value;

        CameraFPV.farClipPlane =  renderSlider.value;
    }

    public void UpdateDTModifier()
    {
        dtModifier = dTSlider.value / 9.0;

        ResetQuadcopter();
    }

    public void UpdateWorldScale()
    {
        worldScale = worldSlider.value;

        ResetQuadcopter();
    }

    public void UpdateCameraAngle()
    {
        cameraAngle = camSlider.value;
        
        CameraFPV.transform.localEulerAngles = new Vector3((float)-cameraAngle, 0, 0);
    }

    public void UpdateDragCoefficient()
    {
        drag = dragSlider.value;

        ResetQuadcopter();
    }

    //Writes the parameters to the quadcopter
    private void SetQuadcopterParameters()
    {
        quadcopter.SetMass(mass);
        quadcopter.SetWorldScale(worldScale);
        quadcopter.SetDragCoefficient(drag);
        quadcopter.SetGravity(gravity);
        quadcopter.SetHorizonPids(horizonPID);
        quadcopter.SetAcrobaticsPids(acrobaticsPID);
        quadcopter.SetTWRatio(twRatio);
    }

    //Reads parameters from the UI
    private void ReadParameters()
    {
        horizonPID = new VectorPID(
            new BetterVector(
                MathExtension.Constrain(S2D(THorizonPitchP.text), 0, 20),
                MathExtension.Constrain(S2D(THorizonRollP.text), 0, 5),
                MathExtension.Constrain(S2D(THorizonYawP.text), 0, 20)),
            new BetterVector(
                MathExtension.Constrain(S2D(THorizonPitchI.text), 0, 20),
                MathExtension.Constrain(S2D(THorizonRollI.text), 0, 5),
                MathExtension.Constrain(S2D(THorizonYawI.text), 0, 20)),
            new BetterVector(
                MathExtension.Constrain(S2D(THorizonPitchD.text), 0, 20),
                MathExtension.Constrain(S2D(THorizonRollD.text), 0, 5),
                MathExtension.Constrain(S2D(THorizonYawD.text), 0, 20)),
            new BetterVector(Time.deltaTime, Time.deltaTime, Time.deltaTime)
        );

        acrobaticsPID = new VectorPID(
            new BetterVector(
                MathExtension.Constrain(S2D(TAcrobaticsPitchP.text), 0, 20),
                MathExtension.Constrain(S2D(TAcrobaticsRollP.text), 0, 5),
                MathExtension.Constrain(S2D(TAcrobaticsYawP.text), 0, 20)),
            new BetterVector(
                MathExtension.Constrain(S2D(TAcrobaticsPitchI.text), 0, 20),
                MathExtension.Constrain(S2D(TAcrobaticsRollI.text), 0, 5),
                MathExtension.Constrain(S2D(TAcrobaticsYawI.text), 0, 20)),
            new BetterVector(
                MathExtension.Constrain(S2D(TAcrobaticsPitchD.text), 0, 20),
                MathExtension.Constrain(S2D(TAcrobaticsRollD.text), 0, 5),
                MathExtension.Constrain(S2D(TAcrobaticsYawD.text), 0, 20)),
            new BetterVector(Time.deltaTime, Time.deltaTime, Time.deltaTime)
        );

        mass = MathExtension.Constrain(S2D(TMass.text), 0.1, 20);
        gravity = MathExtension.Constrain(S2D(TGravity.text), -20, 20);
        twRatio = MathExtension.Constrain(S2D(TTWRatio.text), 1, 20);
        acroExpo = MathExtension.Constrain(S2D(TAcroExpo.text), 0.5, 5);
        acroRate = MathExtension.Constrain(S2D(TAcroRate.text), 0.5, 20);
        horizonExpo = MathExtension.Constrain(S2D(THoriExpo.text), 0.5, 5);
        drag = MathExtension.Constrain(dragSlider.value, 0.2, 1.6);
        worldScale = MathExtension.Constrain(worldSlider.value, 0.2, 5.0);
        cameraAngle = MathExtension.Constrain(camSlider.value, 0, 90);
        dtModifier = MathExtension.Constrain(dTSlider.value, 0.1, 10);
        renderDistance = MathExtension.Constrain(renderSlider.value, 100, 10000);

        ResetQuadcopter();
    }

    //Writes parameters to the UI
    private void DisplayParameters()
    {
        TMass.text = mass.ToString();
        TGravity.text = gravity.ToString();
        TTWRatio.text = twRatio.ToString();
        TAcroExpo.text = acroExpo.ToString();
        TAcroRate.text = acroRate.ToString();
        THoriExpo.text = horizonExpo.ToString();

        dragSlider.value = (float)drag;
        worldSlider.value = (float)worldScale;
        camSlider.value = (float)cameraAngle;
        dTSlider.value = (float)dtModifier;
        renderSlider.value = (float)renderDistance;
        
        BetterVector hKP = horizonPID.GetKP();
        BetterVector hKI = horizonPID.GetKI();
        BetterVector hKD = horizonPID.GetKD();

        BetterVector aKP = acrobaticsPID.GetKP();
        BetterVector aKI = acrobaticsPID.GetKI();
        BetterVector aKD = acrobaticsPID.GetKD();

        THorizonPitchP.text = hKP.X.ToString();
        THorizonPitchI.text = hKI.X.ToString();
        THorizonPitchD.text = hKD.X.ToString();

        THorizonRollP.text = hKP.Y.ToString();
        THorizonRollI.text = hKI.Y.ToString();
        THorizonRollD.text = hKD.Y.ToString();

        THorizonYawP.text = hKP.Z.ToString();
        THorizonYawI.text = hKI.Z.ToString();
        THorizonYawD.text = hKD.Z.ToString();
        
        TAcrobaticsPitchP.text = aKP.X.ToString();
        TAcrobaticsPitchI.text = aKI.X.ToString();
        TAcrobaticsPitchD.text = aKD.X.ToString();

        TAcrobaticsRollP.text = aKP.Y.ToString();
        TAcrobaticsRollI.text = aKI.Y.ToString();
        TAcrobaticsRollD.text = aKD.Y.ToString();

        TAcrobaticsYawP.text = aKP.Z.ToString();
        TAcrobaticsYawI.text = aKI.Z.ToString();
        TAcrobaticsYawD.text = aKD.Z.ToString();
    }

    //Writes the default parameters to the UI
    public void SetDefaultParameters()
    {
        TMass.text = "0.5";
        TGravity.text = "-9.81";
        TTWRatio.text = "10.0";
        TAcroExpo.text = "2.5";
        TAcroRate.text = "7.5";
        THoriExpo.text = "1.0";
        dragSlider.value = 0.6f;
        worldSlider.value = 0.5f;
        camSlider.value = 45f;
        dTSlider.value = 9;
        renderSlider.value = 1000f;

        string hp = "1.0", hi = "0.25", hd = "0.09";//1.0, 0.25, 0.09
        string ap = "10.0", ai = "8.0", ad = "0.0";//10.0, 8.0, 0.0

        THorizonPitchP.text = hp;
        THorizonPitchI.text = hi;
        THorizonPitchD.text = hd;

        THorizonRollP.text = hp;
        THorizonRollI.text = hi;
        THorizonRollD.text = hd;

        THorizonYawP.text = hp;
        THorizonYawI.text = hi;
        THorizonYawD.text = hd;
        
        TAcrobaticsPitchP.text = ap;
        TAcrobaticsPitchI.text = ai;
        TAcrobaticsPitchD.text = ad;

        TAcrobaticsRollP.text = ap;
        TAcrobaticsRollI.text = ai;
        TAcrobaticsRollD.text = ad;

        TAcrobaticsYawP.text = ap;
        TAcrobaticsYawI.text = ai;
        TAcrobaticsYawD.text = ad;
    }

    //Saves the configuration to a file
    private void SaveParameters()
    {
        SaveData sd = new SaveData
        {
            HKP = horizonPID.GetKP(),
            HKI = horizonPID.GetKI(),
            HKD = horizonPID.GetKD(),
            AKP = acrobaticsPID.GetKP(),
            AKI = acrobaticsPID.GetKI(),
            AKD = acrobaticsPID.GetKD(),
            Mass = this.mass,
            Gravity = this.gravity,
            TWRatio = this.twRatio,
            Drag = this.drag,
            WorldScale = this.worldScale,
            CameraAngle = this.cameraAngle,
            DTMultiplier = this.dtModifier,
            RenderDistance = this.renderDistance,
            AcrobaticsExpo = this.acroExpo,
            AcrobaticsRate = this.acroRate,
            HorizonExpo = this.horizonExpo
        };

        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);
        
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, sd);
        file.Close();
    }

    //Loads the configuration from a file
    private void LoadParameters()
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination))
        {
            file = File.OpenRead(destination);
            
            BinaryFormatter bf = new BinaryFormatter();
            SaveData sd = (SaveData)bf.Deserialize(file);
            file.Close();

            horizonPID = new VectorPID(sd.HKP, sd.HKI, sd.HKD, new BetterVector(Time.deltaTime, Time.deltaTime, Time.deltaTime));
            acrobaticsPID = new VectorPID(sd.AKP, sd.AKI, sd.AKD, new BetterVector(Time.deltaTime, Time.deltaTime, Time.deltaTime));
            mass = sd.Mass;
            gravity = sd.Gravity;
            twRatio = sd.TWRatio;
            drag = sd.Drag;
            worldScale = sd.WorldScale;
            cameraAngle = sd.CameraAngle;
            dtModifier = sd.DTMultiplier;
            renderDistance = sd.RenderDistance;
            acroExpo = sd.AcrobaticsExpo;
            acroRate = sd.AcrobaticsRate;
            horizonExpo = sd.HorizonExpo;

            ResetQuadcopter();
        }
        else
        {
            Debug.LogError("File not found");
            return;
        }
    }
    
    private double S2D(string s)
    {
        return double.Parse(s, System.Globalization.CultureInfo.InvariantCulture);
    }
}
