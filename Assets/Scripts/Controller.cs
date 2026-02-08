using System;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [Header("Drone References")]
    [SerializeField] Rigidbody droneRB;
    [SerializeField] Transform droneTransform;
    [Header("Drone Target")]
    public Transform target;

    [Header("Propellor References")]
    [SerializeField] PropScript FRControl;
    private double FRPower;
    [SerializeField] PropScript FLControl;
    private double FLPower;
    [SerializeField] PropScript BRControl;
    private double BRPower;
    [SerializeField] PropScript BLControl;
    private double BLPower;

    [Header("Scripts")]
    [SerializeField] Constants constants;
    [SerializeField] RotationController rotationController;
    [SerializeField] WindController windController;

    [Header("PID Constants")]
    //////// MOVEMENT PID
    public double proportionalConst;
    public double integralConst;
    public double derivativeConst;

    [Header("Flight Settings")]
    public bool smoothed;
    public bool stabilize;
    public bool windEnabled;
    public bool drawDebug;

    [Header("Safety Settings")]
    [Tooltip("Height in meters below which rotation is disabled to prevent flipping.")]
    public float minFlightAltitude = 0.5f;

    // Store total of errors
    private Vector3 integral;
    private Vector3 smoothedTarget;

    double ThrustToPower(double thrust)
    {
        if (thrust < 0)
        {
            return -Math.Pow(-thrust / constants.efficency, 3f / 2f) * constants.drag;
        }
        return Math.Pow(thrust / constants.efficency, 3f / 2f) * constants.drag;
    }

    void FixedUpdate()
    {
        // Apply wind
        Vector3 wind = windEnabled ? windController.GetWind(droneTransform.position) : Vector3.zero;
        droneRB.AddForce(wind);
        if (drawDebug) Debug.DrawRay(droneTransform.position, wind, Color.cyan);

        // Use smoothed target if smoothed is checked on
        if (smoothed)
        {
            smoothedTarget = Vector3.Lerp(smoothedTarget, target.position, 0.1f);
        }
        else
        {
            smoothedTarget = target.position;
        }

        ///// GET MOVEMENT VECTOR
        Vector3 positionDiff = smoothedTarget - droneTransform.position;

        if (droneTransform.position.y > minFlightAltitude)
        {
            integral += positionDiff;
        }
        else
        {
            integral = Vector3.zero; 
        }

        Vector3 proportional = positionDiff;                // P term
        Vector3 derivative = -droneRB.linearVelocity;       // D term 

        Vector3 gravityVector = 9.81f * droneRB.mass * Vector3.up;  // Gravity vector

        Vector3 pidAdjust = proportional * (float)proportionalConst + integral * (float)integralConst + derivative * (float)derivativeConst;

        // Add vectors to get movement vector
        Vector3 movementVector = gravityVector + pidAdjust;

        // Check for necessary stabilization if turned on
        if (stabilize && Vector3.Angle(Vector3.up, droneTransform.up) > 40)
        {
            float angleDiffRads = (Vector3.Angle(Vector3.up, droneTransform.up) - 40) / 180.0f * Mathf.PI;
            movementVector = Vector3.RotateTowards(movementVector.normalized, Vector3.up, angleDiffRads, 0.0f) * movementVector.magnitude;
        }

        if (drawDebug)
        {
            Debug.DrawRay(droneTransform.position, movementVector, Color.green);
        }

        // Get rotational acceleration
        Vector3 rotationalAcceleration = rotationController.RotateTo(movementVector);

        // Get additional thrust for movement to target
        double baseThrust = movementVector.magnitude / 4f;

        
        double rotationInfluence = (droneTransform.position.y < minFlightAltitude) ? 0.0 : 1.0;

        // Assign thrust
        double FRThrust = (rotationController.getRotation(rotationalAcceleration)[0] * rotationInfluence) + baseThrust;
        double FLThrust = (rotationController.getRotation(rotationalAcceleration)[1] * rotationInfluence) + baseThrust;
        double BRThrust = (rotationController.getRotation(rotationalAcceleration)[2] * rotationInfluence) + baseThrust;
        double BLThrust = (rotationController.getRotation(rotationalAcceleration)[3] * rotationInfluence) + baseThrust;

        // Get in terms of power
        FRPower = ThrustToPower(FRThrust);
        FLPower = ThrustToPower(FLThrust);
        BRPower = ThrustToPower(BRThrust);
        BLPower = ThrustToPower(BLThrust);

        // Apply power
        FRControl.ApplyPower(FRPower);
        FLControl.ApplyPower(FLPower);
        BRControl.ApplyPower(BRPower);
        BLControl.ApplyPower(BLPower);
    }
}