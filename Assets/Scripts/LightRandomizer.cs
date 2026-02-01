using UnityEngine;

public class LightRandomizer : MonoBehaviour
{
    [SerializeField] Light pointLight;

    void Start()
    {
        float hue = Random.value; // 0–1
        pointLight.color = Color.HSVToRGB(hue, 1f, 1f);
    }
}
