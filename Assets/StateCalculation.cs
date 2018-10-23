using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets;

//Used to calculate the current state of the quadcopter object
public class StateCalculation : MonoBehaviour {
    Quadcopter quadcopter;

	// Use this for initialization
	void Start () {
        quadcopter = new Quadcopter(true);
    }
	
	// Update is called once per frame
	void Update () {
        Controls controls = new Controls();

        quadcopter.EstimateState(controls, Time.deltaTime);
        //quadcopter.ApplyCollision();
	}
}
