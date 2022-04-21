using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct squad
{
    public string squadName;
    public GameObject[] squadMembers;
}

public class UnitManager : MonoBehaviour
{
    public GameObject unit;

    public squad[] squads;

    public int numSquads;
    public int numUnits;

    public Transform[] unitLocations;

    public bool start;

    public float[] unitTypeCon;
    public UnitType[] unitTypes;

    private void Start()
    {
        start = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!start)
            {
                SpawnSquads();
            }
            start = true;
        }
    }

    private void SpawnSquads()
    {
        squads = new squad[numSquads];
        for(int i = 0; i < numSquads; i++)
        {
            GameObject[] newSquad = new GameObject[4];
            for(int j = 0; j < 4; j++)
            {
                newSquad[j] = Instantiate(unit, unitLocations[i]);
                newSquad[j].transform.Translate(new Vector3(i, 0.0f, j));
                newSquad[j].GetComponent<Unit>().SetTeam(i + 1);
                newSquad[j].GetComponent<Unit>().SetUnitType(unitTypes[j]);
                newSquad[j].GetComponent<Confidence>().SetConfidence(unitTypeCon[j]);
            }
            squads[i].squadName = "Squad " + (i + 1).ToString();
            squads[i].squadMembers = newSquad;
        }
    }
}
