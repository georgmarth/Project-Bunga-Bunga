using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PatronUI : MonoBehaviour
{
    public Slider PatienceSlider;
    public Slider FulfillmentSlider;

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.identity;
    }

    public void SetPatienceActive(bool value)
    {
        PatienceSlider.gameObject.SetActive(value);
    }

    public void SetPatience(float value)
    {
        PatienceSlider.value = value;
    }

    public void SetFulfillmentActive(bool value)
    {
        FulfillmentSlider.gameObject.SetActive(value);
    }

    public void SetFulfillment(float value)
    {
        FulfillmentSlider.value = value;
    }
}
