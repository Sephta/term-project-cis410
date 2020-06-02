using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bartender : MonoBehaviour
{
    public Slider slider;

    public void setMax(float value)
    {
        slider.maxValue = value;
        slider.value = value;
    }

    public void setValue(float value)
    {
        slider.value = value;
    }
}
