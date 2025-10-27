using UnityEngine;
using UnityEngine.UI;

public class FuelBarColor : MonoBehaviour
{
    public Slider fuelSlider;
    public Image fillImage;

    // Define your custom "full" color
    private Color boostBlue = new Color32(0x14, 0xB5, 0xFF, 0xFF); // #14b5ff

    void Update()
    {
        float t = fuelSlider.value / fuelSlider.maxValue;

        // Lerp from red → yellow → blue
        if (t > 0.5f)
            fillImage.color = Color.Lerp(Color.yellow, boostBlue, (t - 0.5f) * 2f);
        else
            fillImage.color = Color.Lerp(Color.red, Color.yellow, t * 2f);
    }
}
