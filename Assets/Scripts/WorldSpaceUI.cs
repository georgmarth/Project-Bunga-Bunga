using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceUI : MonoBehaviour
{
    public Vector3 CanvasRotation;

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(CanvasRotation);
    }
}
