using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerSelectStrip : MonoBehaviour
{
    public Transform ActiveContainer;
    public Transform InactiveContainer;

    public TextMeshProUGUI TextMesh;

    public void Join()
    {
        InactiveContainer.gameObject.SetActive(false);
        ActiveContainer.gameObject.SetActive(true);
    }

    public void SetSelectionText(string newText)
    {
        TextMesh.text = newText;
    }

}
