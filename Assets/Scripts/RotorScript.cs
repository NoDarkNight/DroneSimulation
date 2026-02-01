using System;
using UnityEngine;

public class RotorScript : MonoBehaviour
{
    [SerializeField] Transform propTransform;
    public PropScript propScript;
    public bool spinCounter;

    void FixedUpdate()
    {
        double spinRate = propScript.rotationSpeed*30/Math.PI / Time.fixedDeltaTime;
        if (spinCounter) {
            propTransform.Rotate(0, -Time.fixedDeltaTime*(float)spinRate, 0);
        } else {
            propTransform.Rotate(0, Time.fixedDeltaTime*(float)spinRate, 0);
        }

    }
}
