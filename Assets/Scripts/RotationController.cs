using System;
using UnityEngine;
using UnityEngine.ProBuilder;

public class RotationController : MonoBehaviour
{
    public bool drawDebug;
    [Header("Prop Transforms")]
    [SerializeField] Transform FRTransform;
    [SerializeField] Transform FLTransform;
    [SerializeField] Transform BRTransform;
    [SerializeField] Transform BLTransform;

    [Header("Drone References")]
    [SerializeField] Rigidbody droneRB; // Drone's Rigidbody
    [SerializeField] Transform droneTransform; // Drone's Transform

    [Header("PID Constants")]

    //////// PID TUNING
    // Constants
    public double proportionalConst;
    public double derivativeConst;

    // Store diff for proportional 
    private double error;
    // Last diff for derivative
    private double lastError;    

    //////// ADJUSTMENT PID
    // Constants
    public double adjProportionalConst;


    void FixedUpdate()
    {
        if (drawDebug)
        {
            Debug.DrawRay(droneTransform.position, droneTransform.up, Color.blue);
        }
    }

    private double GetThrust(Transform propTransform, Vector3 rotationAxis, Vector3 joystickAxis) {
        // Input: Transform of propeller
        // Output: Thrust amount based on proximity to rotationAxis. joystickAxis used to determine thrust direction
        Vector3 propLocation = propTransform.position - droneTransform.position;

        double thrust = Vector3.Magnitude(Vector3.Cross(propLocation, rotationAxis))/Vector3.Magnitude(rotationAxis);
        
        // If dot product is less than zero, invert thrust. (<0 means >90 degrees to joystickAxis meaning prop is on opposite side)
        if (Vector3.Dot(propLocation, joystickAxis) < 0) {
            thrust = -thrust;
        }

        return thrust;
    }

    public Vector3 RotateTo(Vector3 worldSpaceTarget) {

        // Get target 
        Vector3 target = worldSpaceTarget - droneTransform.position;

        // Get spin axis to rotate to target
        Vector3 rotationAxis = getRotationAxis(target);

        // Calculate the angle between the target and the drone's up direction
        error = Vector3.Angle(target.normalized, droneTransform.up);
        // Calculate derivative
        double derivative = (error-lastError)/Time.fixedDeltaTime;
        // Assign errors for next derivative calculation
        lastError = error;


        // Get the "difference" between the drone's intended spin plane and its current heading. This will need to be corrected to 0.
        Vector3 alignedSpin = Vector3.Dot(droneRB.angularVelocity, rotationAxis) * rotationAxis;
        Vector3 perpendicularSpin = droneRB.angularVelocity - alignedSpin;
        if (drawDebug)
        {
            Debug.DrawRay(droneTransform.position, perpendicularSpin, Color.white);
        }
        double perpendicularSpeed = perpendicularSpin.magnitude; // Acts as error

        Vector3 spinAccel = rotationAxis * (float)((proportionalConst * error) + (derivativeConst * derivative));
        Vector3 horizAdjAccel = -perpendicularSpin.normalized * (float)(perpendicularSpeed * adjProportionalConst);


        return spinAccel + horizAdjAccel;
    }

    public double[] getRotation(Vector3 accelerationVector) {
        if (accelerationVector == new Vector3 (0, 0, 0)) {
            return new double[] { 0, 0, 0, 0 };
        }

        Vector3 rotationAxis = accelerationVector.normalized;


        Vector3 joystickAxis = Vector3.Cross(droneTransform.up, rotationAxis);

        if (drawDebug)
        {
            Debug.DrawRay(droneTransform.position, rotationAxis, Color.magenta);
            Debug.DrawRay(droneTransform.position, joystickAxis, Color.yellow);
        }

        double FRThrust = GetThrust(FRTransform, rotationAxis, joystickAxis) * accelerationVector.magnitude;
        double FLThrust = GetThrust(FLTransform, rotationAxis, joystickAxis) * accelerationVector.magnitude;
        double BRThrust = GetThrust(BRTransform, rotationAxis, joystickAxis) * accelerationVector.magnitude;
        double BLThrust = GetThrust(BLTransform, rotationAxis, joystickAxis) * accelerationVector.magnitude;

        return new double[] { FRThrust, FLThrust, BRThrust, BLThrust };
    }

    public Vector3 getRotationAxis(Vector3 targetRot)
    {
        Vector3 cross = Vector3.Cross(droneTransform.up, targetRot);
        return cross.normalized;
    }
}