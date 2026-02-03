using UnityEngine;
using System.Collections.Generic;

public class DroneFormationManager : MonoBehaviour
{
    public List<Transform> droneTargets;

    [Header("Grid Settings")]
    [SerializeField] float spacing = 2.0f;
    [SerializeField] Vector3 centerOffset; 

    [Header("Animation")]
    [SerializeField] Texture2D[] formations;
    [SerializeField] float transitionSpeed = 5f;

    private List<Vector3> currentTargetPositions = new List<Vector3>();
    private int curFormation = -1;


    void Update()
    {
        // Move to new pos
        for (int i = 0; i < droneTargets.Count; i++)
        {
            if (i < currentTargetPositions.Count)
            {
                droneTargets[i].position = Vector3.Lerp(
                    droneTargets[i].position,
                    currentTargetPositions[i],
                    Time.deltaTime * transitionSpeed
                );
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            curFormation = curFormation + 1 % formations.Length;
            print(curFormation);
            UpdateFormation(formations[curFormation]);
        }
    }

    public void UpdateFormation(Texture2D image)
    {
        currentTargetPositions.Clear();
        int droneIndex = 0;
        int width = image.width;
        int height = image.height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (image.GetPixel(x, y).a > 0.5f && droneIndex < droneTargets.Count)
                {
                    Vector3 newPos = new Vector3(x * spacing, y * spacing, 0) + centerOffset;
                    currentTargetPositions.Add(newPos);
                    droneIndex++;
                    
                }
            }
        }

        // Handle unused 
        for (int i = droneIndex; i < droneTargets.Count; i++)
        {
            currentTargetPositions.Add(new Vector3(i * spacing, 0, 0)); 
        }
    }
}