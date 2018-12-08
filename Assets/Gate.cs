using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public GameObject raceObject;
    public int GateValue;

    void Start()
    {
        Debug.Log("G Added Gate: " + GateValue);
        raceObject.GetComponent<Race>().AddGate(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        raceObject.GetComponent<Race>().GateTriggered(GateValue);
    }
}
