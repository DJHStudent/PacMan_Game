using UnityEngine;
using UnityEngine.UI;

public class InnovationSettings : MonoBehaviour //how the values the user gives for the innovations settings
{
    public int width, height, ghost1Amount, ghost2Amount, ghost3Amount, ghost4Amount;
    public Slider widthSlider, heightSlider, ghost1Slider, ghost2Slider, ghost3Slider, ghost4Slider;
    public void setValues()
    {
        width = (int)widthSlider.value;
        height = (int)heightSlider.value;
        ghost1Amount = (int)ghost1Slider.value;
        ghost2Amount = (int)ghost2Slider.value;
        ghost3Amount = (int)ghost3Slider.value;
        ghost4Amount = (int)ghost4Slider.value;
    }
}
