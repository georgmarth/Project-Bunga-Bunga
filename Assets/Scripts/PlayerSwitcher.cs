using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwitcher : MonoBehaviour
{
    public GameObject[] ModelPrefabs;

    public GameObject CurrentModel;
    private int _index = 0;

    public Animator Switch(bool right)
    {
        _index += right ? 1 : -1;
        _index %= ModelPrefabs.Length;
        var parent = CurrentModel.transform.parent;
        Vector3 position = CurrentModel.transform.position;
        Quaternion rotation = CurrentModel.transform.rotation;
        Destroy(CurrentModel);
        CurrentModel = Instantiate(ModelPrefabs[_index], position, rotation, parent);
        return CurrentModel.GetComponent<Animator>();
    }
}
