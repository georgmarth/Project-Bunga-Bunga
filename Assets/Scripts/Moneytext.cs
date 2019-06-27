using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Moneytext : MonoBehaviour
{
    public GameEvents GameEvents;

    private TextMeshProUGUI _text;

    private void OnEnable()
    {
        GameEvents.MoneyChanged += OnMoneyChanged;
    }

    private void OnDisable()
    {
        GameEvents.MoneyChanged -= OnMoneyChanged;
    }

    // Start is called before the first frame update
    void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    public void OnMoneyChanged(int money)
    {
        _text.SetText(money.ToString());
    }
}
