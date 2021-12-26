using UnityEngine;
using UnityEngine.UI;

public class SliderValueChange : MonoBehaviour//check the sliders value and update the UI text if it changes
{
    Slider slider;
    void Start()
    {
        slider = GetComponent<Slider>();
    }

    public void onSliderChange(Text text)
    {
        text.text = "" + slider.value;
    }
}
