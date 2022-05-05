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

        string text = locations[index].Trim(new char[] {' ', '-', '+'});

        GameObject terrain = GameObject.FindGameObjectWithTag(text);

        manager.GetComponent<UnitManager>().unitLocations[squad] = terrain.transform;
    }
}
