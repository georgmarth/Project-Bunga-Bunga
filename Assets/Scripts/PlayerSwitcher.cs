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
        _index = (_index + (right ? 1 : -1) + ModelPrefabs.Length) % ModelPrefabs.Length;
        var parent = CurrentModel.transform.parent;
        Vector3 position = CurrentModel.transform.position;
        Quaternion rotation = CurrentModel.transform.rotation;
        int layer = CurrentModel.layer;
        Destroy(CurrentModel);
        CurrentModel = Instantiate(ModelPrefabs[_index], position, rotation, parent);
        // set preview layer
        CurrentModel.gameObject.layer = layer;
        foreach (var transform in CurrentModel.GetComponentsInChildren<Transform>(true))
        {
            transform.gameObject.layer = layer;
        }
        //return CurrentModel.GetComponent<Animator>();
        return null;
    }
}
