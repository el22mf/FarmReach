using UnityEngine;
using UnityEngine.UI;

public class SliderValueText : MonoBehaviour
{
    public Text valueText;

    public void UpdateValue(float value)
    {
        valueText.text = value.ToString("F0");
    }
}