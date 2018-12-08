using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Race : MonoBehaviour
{
    private List<GameObject> gateList = new List<GameObject>();
    public int GateCount;
    public int TargetGate = 1;
    public int CurrentLap = 1;
    private bool isRacing = false;
    private bool lastGateHit = false;
    private DateTime start;
    private DateTime lap1Start;
    private DateTime lap1End;
    private DateTime lap2Start;
    private DateTime lap2End;
    private DateTime lap3Start;
    private DateTime lap3End;

    public GameObject RaceInformation;
    public GameObject EndRaceInformation;
    public GameObject StartRaceButton;
    public GameObject StopRaceButton;

    public Text TotalRaceDisplay;
    public Text LapOneDisplay;
    public Text LapTwoDisplay;
    public Text LapThrDisplay;

    public Text startTime;
    public Text lapTime;

    public Material NormalGateMaterial;
    public Material TargetGateMaterial;

    public void Update()
    {
        if (isRacing)
        {
            startTime.text = FormatTimeSpan(DateTime.Now - start);

            if (CurrentLap == 1)
            {
                lapTime.text = "One:   " + FormatTimeSpan(DateTime.Now - lap1Start);
            }
            else if (CurrentLap == 2)
            {
                lapTime.text = "Two:   " + FormatTimeSpan(DateTime.Now - lap2Start);
            }
            else if (CurrentLap == 3)
            {
                lapTime.text = "Three: " + FormatTimeSpan(DateTime.Now - lap3Start);
            }
        }
        else
        {
            startTime.text = "00:00.000";
            lapTime.text = "00:00.000";

            StopRace();
        }
    }

    public void StartRace()
    {
        HidePostRaceInfo();
        StartRaceButton.SetActive(false);
        StopRaceButton.SetActive(true);
        RaceInformation.SetActive(true);
        TargetGate = 1;
        CurrentLap = 1;
        start = DateTime.Now;
        lap1Start = DateTime.Now;
        isRacing = true;
        ColorGreenGate(1);
    }

    public void StopRace()
    {
        StartRaceButton.SetActive(true);
        StopRaceButton.SetActive(false);
        RaceInformation.SetActive(false);

        isRacing = true;
        ClearGates();
    }

    public void AddGate(GameObject gate)
    {
        gateList.Add(gate);
    }

    private void ColorGreenGate(int gateNumber)
    {
        foreach (GameObject gate in gateList)
        {
            if (gate.GetComponent<Gate>().GateValue == gateNumber)
            {
                gate.GetComponent<Renderer>().material = TargetGateMaterial;
            }
            else
            {
                gate.GetComponent<Renderer>().material = NormalGateMaterial;
            }
        }
    }

    private void ClearGates()
    {
        foreach (GameObject gate in gateList)
        {
            gate.GetComponent<Renderer>().material = NormalGateMaterial;
        }
    }

    public void GateTriggered(int gate)
    {
        if (TargetGate == gate && isRacing)
        {
            if (lastGateHit == true && TargetGate == 1)
            {
                CurrentLap++;

                if (CurrentLap == 2)
                {
                    lap1End = DateTime.Now;
                    lap2Start = DateTime.Now;
                }
                else if (CurrentLap == 3)
                {
                    lap2End = DateTime.Now;
                    lap3Start = DateTime.Now;
                }

                lastGateHit = false;
            }

            TargetGate++;

            if (TargetGate > GateCount)
            {
                TargetGate = 1;

                lastGateHit = true;
            }

            ColorGreenGate(TargetGate);

            if (CurrentLap == 4)
            {
                lap3End = DateTime.Now;
                ClearGates();
                isRacing = false;
                DisplayPostRaceInfo();
            }
        }
    }

    public void DisplayPostRaceInfo()
    {
        EndRaceInformation.SetActive(true);
        TotalRaceDisplay.text = FormatTimeSpan(lap3End - start);
        LapOneDisplay.text = FormatTimeSpan(lap1End - lap1Start);
        LapTwoDisplay.text = FormatTimeSpan(lap2End - lap2Start);
        LapThrDisplay.text = FormatTimeSpan(lap3End - lap3Start);
    }

    public void HidePostRaceInfo()
    {
        EndRaceInformation.SetActive(false);
        TotalRaceDisplay.text = "00:00.000";
        LapOneDisplay.text = "00:00.000";
        LapTwoDisplay.text = "00:00.000";
        LapThrDisplay.text = "00:00.000";
    }

    public string FormatTimeSpan(TimeSpan tS)
    {
        string mins = tS.Minutes.ToString();
        string secs = tS.Seconds.ToString();
        string mils = tS.Milliseconds.ToString();

        if (tS.Minutes < 10)
        {
            mins = "0" + tS.Minutes;
        }

        if (tS.Seconds < 10)
        {
            secs = "0" + tS.Seconds;
        }

        if (tS.Milliseconds < 10)
        {
            mils = "0" + tS.Milliseconds;
        }

        return String.Format("{0}:{1}.{2}", mins, secs, mils);
    }
}
