using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartLocations : MonoBehaviour
{
    public List<string> locations;

    public int squad;

    public GameObject manager;

    private void Start()
    {
        var dropdown = transform.GetComponent<Dropdown>();

        dropdown.options.Clear();

        foreach(var location in locations)
        {
            dropdown.options.Add(new Dropdown.OptionData(location));
        }

        DropdownItemSelected(dropdown);

        dropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(dropdown); });
    }

    private void DropdownItemSelected(Dropdown dropdown)
    {
        int index = dropdown.value;

        GameObject terrain = GameObject.FindGameObjectWithTag(locations[index]);

        //Debug.Log(terrain.name);

        manager.GetComponent<UnitManager>().unitLocations[squad] = terrain.transform;
    }
}
