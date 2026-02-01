using System;
using UnityEngine;

public class WindController : MonoBehaviour
{
    [Header("Scale by Simulation Time")]
    public float timeScale;
    [Header("Scale by Spatial Location")]
    public float positionScale;

    public float maxWindStrength;

    public Vector3 GetWind(Vector3 position)
    {
        float t = Time.time * timeScale;

        float xNoise = Mathf.PerlinNoise(position.x * positionScale + 10, t);
        float yNoise = Mathf.PerlinNoise(position.y * positionScale + 20, t + 10)/10f;
        float zNoise = Mathf.PerlinNoise(position.z * positionScale + 30, t + 20);

        // Remap from [0,1] to [-1,1]
        Vector3 windSpeed = new Vector3(xNoise, yNoise + 0.45f, zNoise) * 2f - Vector3.one;
        Debug.DrawRay(position, windSpeed, Color.grey);

        return windSpeed * maxWindStrength;
    }

}
