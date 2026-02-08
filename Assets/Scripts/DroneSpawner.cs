using UnityEngine;
using System.Collections.Generic;

public class DroneSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject dronePrefab;
    [SerializeField] GameObject targetPrefab;

    [Header("Settings")]
    [SerializeField] int numberOfDrones = 50;
    [SerializeField] float floorSpacing = 1f; 

    [Header("References")]
    [SerializeField] DroneFormationManager formationManager;

    void Start()
    {
        SpawnSwarm();
    }

    void SpawnSwarm()
    {

        int rows = Mathf.CeilToInt(Mathf.Sqrt(numberOfDrones));

        for (int i = 0; i < numberOfDrones; i++)
        {
            // 1. Ground position
            float x = (i % rows) * floorSpacing;
            float z = (i / rows) * floorSpacing + 50f;
            Vector3 spawnPos = new Vector3(x, 0.5f, z); 

            // 2. Instantiate Drone
            GameObject newDrone = Instantiate(dronePrefab, spawnPos, Quaternion.identity);

            // 3. Instantiate Target 
            GameObject newTarget = Instantiate(targetPrefab, spawnPos, Quaternion.identity);

            // 4. Link
            Controller controller = newDrone.GetComponent<Controller>();
            if (controller != null)
            {
                controller.target = newTarget.transform;
            }

            // 5. Add to Formation Manager
            formationManager.droneTargets.Add(newTarget.transform);

            newDrone.transform.parent = this.transform;
            newTarget.transform.parent = this.transform;
        }
    }
}