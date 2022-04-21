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

    public Transform[] unitLocations;

    public bool start;

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
                newSquad[j].GetComponent<Unit>().SetTeam(i + 1);
                switch (j)
                {
                    case 0:
                        newSquad[j].GetComponent<Confidence>().SetConfidence(0.25f);
                        newSquad[j].GetComponent<Unit>().SetUnitType(UnitType.SUPPORT);
                        break;
                    case 1:
                        newSquad[j].GetComponent<Confidence>().SetConfidence(0.50f);
                        newSquad[j].GetComponent<Unit>().SetUnitType(UnitType.GUARD);
                        break;
                    case 2:
                        newSquad[j].GetComponent<Confidence>().SetConfidence(0.75f);
                        newSquad[j].GetComponent<Unit>().SetUnitType(UnitType.HUNTER);
                        break;
                    case 3:
                        newSquad[j].GetComponent<Confidence>().SetConfidence(1.0f);
                        newSquad[j].GetComponent<Unit>().SetUnitType(UnitType.CAPTAIN);
                        break;
                    default:
                        break;
                }
            }
            squads[i].squadName = "Squad " + (i + 1).ToString();
            squads[i].squadMembers = newSquad;
        }
    }
}
