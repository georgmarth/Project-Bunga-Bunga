using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwitcher : MonoBehaviour
{
    [System.Serializable]
    public class Option
    {
        public string Name;
        public GameObject[] Objects;
    }

    public GameEvents GameEvents;
    public Option[] Options;

    public int CurrentList { get; private set; }
    private int[] _selection;

    public int PlayerIndex = 0;

    public bool ShuffleOnStart = false;

    private void Awake()
    {
        CurrentList = 0;

        _selection = new int[Options.Length];
        for (int i = 0; i < _selection.Length; i++)
        {
            _selection[i] = 0;
        }
    }

    private void Start()
    {
        if (ShuffleOnStart)
            Shuffle();
    }

    public void Shuffle()
    {
        for (int i = 0; i < Options.Length; i++)
        {
            _selection[i] = Random.Range(0, Options[i].Objects.Length);
            for (int j = 0; j < Options[i].Objects.Length; j++)
            {
                Options[i].Objects[j].SetActive(j == _selection[i]);
            }
        }
        GameEvents.OutfitListSwitched?.Invoke(PlayerIndex, Options[CurrentList].Name);
    }

    public void Switch(bool right)
    {
        // deactivate old selection
        Options[CurrentList].Objects[_selection[CurrentList]]?.SetActive(false);
        // switch enumator
        int inc = (right ? 1 : -1);
        int length = Options[CurrentList].Objects.Length;
        _selection[CurrentList] = (_selection[CurrentList] + inc + length) % length;
        // activate new selection
        Options[CurrentList].Objects[_selection[CurrentList]]?.SetActive(true);
    }

    public void SwitchOption(bool up)
    {
        CurrentList = (CurrentList + (up ? 1 : -1) + Options.Length) % Options.Length;
        GameEvents.OutfitListSwitched?.Invoke(PlayerIndex, Options[CurrentList].Name);
    }
}