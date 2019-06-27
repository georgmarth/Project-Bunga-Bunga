using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timertext : MonoBehaviour
{
    public GameEvents GameEvents;

    private TextMeshProUGUI _text;

    private void OnEnable()
    {
        GameEvents.LevelTimer += OnTimerChanged;
    }

    private void OnDisable()
    {
        GameEvents.LevelTimer -= OnTimerChanged;
    }

    // Start is called before the first frame update
    void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    public void OnTimerChanged(float timer)
    {
        int minutes = (int)timer / 60;
        int seconds = (int)timer % 60;
        string formatted = string.Format("{0:D2}:{1:D2}", minutes, seconds);
        _text.SetText(formatted);
    }
}
