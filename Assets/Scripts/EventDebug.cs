using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDebug : MonoBehaviour
{
    public GameEvents GameEvents;

    // Start is called before the first frame update
    void Start()
    {
        GameEvents.PlayerJoined += gameObject => Debug.Log(gameObject.transform);
    }
}
