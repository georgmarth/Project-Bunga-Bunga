using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillSlider : MonoBehaviour
{
    public TaskLocation TaskLocation;

    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    private void UpdatePercentage(float value)
    {
        _image.fillAmount = value;
    }

    private void OnEnable()
    {
        TaskLocation.UsePercentageLeft += UpdatePercentage;
    }

    private void OnDisable()
    {
        TaskLocation.UsePercentageLeft -= UpdatePercentage;
    }
}
