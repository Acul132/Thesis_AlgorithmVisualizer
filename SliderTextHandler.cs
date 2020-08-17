using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//Handler class for the sliders in the settings menu
public class SliderTextHandler : MonoBehaviour
{
    //Retrieve references of the gameobjects
    public TMP_Text maxHeightText;
    public TMP_Text minHeightText;
    public TMP_Text numOfBarsText;

    public Slider maxHeightSlider;
    public Slider minHeightSlider;
    public Slider numOfBarsSlider;

    //Text values without the value of the slider added
    private static string defaultMaxHeightText = "Maximum Value (11-30): ";
    private static string defaultMinHeightText = "Minimum Value (1-10): ";
    private static string defaultNumOfBarsText = "Number of Bars (4-60): ";

    public void Start()
    {
        maxHeightText.SetText(defaultMaxHeightText + maxHeightSlider.value);
        minHeightText.SetText(defaultMinHeightText + minHeightSlider.value);
        numOfBarsText.SetText(defaultNumOfBarsText + numOfBarsSlider.value);
    }

    public void updateMaxText()
    {
        maxHeightText.SetText(defaultMaxHeightText + maxHeightSlider.value);
    }
    
    public void updateMinText()
    {
        minHeightText.SetText(defaultMinHeightText + minHeightSlider.value);
    }

    public void updateNumOfBarsText()
    {
        numOfBarsText.SetText(defaultNumOfBarsText + numOfBarsSlider.value);
    }
}
