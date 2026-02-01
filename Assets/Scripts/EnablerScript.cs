using UnityEngine;

public class EnablerScript : MonoBehaviour
{
    [SerializeField] bool ON = true;
    [SerializeField] Controller dronController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (ON)
        {
            dronController.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) dronController.enabled = !dronController.enabled;
    }
}
