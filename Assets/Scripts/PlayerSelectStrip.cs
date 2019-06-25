using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectStrip : MonoBehaviour
{
    public Transform ActiveContainer;
    public Transform InactiveContainer;

    public void Join()
    {
        InactiveContainer.gameObject.SetActive(false);
        ActiveContainer.gameObject.SetActive(true);
    }
}
