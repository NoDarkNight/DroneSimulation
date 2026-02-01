using System;
using UnityEngine;
using UnityEngine.Rendering;

public class PropScript : MonoBehaviour
{
    [SerializeField] Rigidbody droneRB;
    [SerializeField] Constants constants;
    [SerializeField] Transform propTransform;
    public double rotationSpeed;

    public void ApplyPower(double power) {
        // W = cube root(p/d)
        if (power < 0) {
            rotationSpeed = -Math.Clamp(Math.Pow(-power/constants.drag, 1f/3f), 0, constants.maxAngularVelocity);
        } else {
            rotationSpeed = Math.Clamp(Math.Pow(power/constants.drag, 1f/3f), 0, constants.maxAngularVelocity);
        }

        // T = kw^2
        double thrust;
        if (rotationSpeed < 0) {
            thrust = -constants.efficency * rotationSpeed * rotationSpeed;
        } else {
            thrust = constants.efficency * rotationSpeed * rotationSpeed;
        }

        Vector3 forceDirection = propTransform.up; // Force direction
        Vector3 forcePoint = propTransform.position; // Apply force at propeller's position
        droneRB.AddForceAtPosition(forceDirection * (float)thrust, forcePoint, ForceMode.Force);
    }
}