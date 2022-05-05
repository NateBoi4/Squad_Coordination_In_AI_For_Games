using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct squad
{
    public string squadName;
    public string favoredTerrain;
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
    public string[] terrains;

    public Light lightSource;
    public float rotationSpeed = 5.0f;

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
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("SampleScene");
        }
        lightSource.transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));
    }

    public void SpawnSquads()
    {
        squads = new squad[numSquads];
        for(int i = 0; i < numSquads; i++)
        {
            GameObject[] newSquad = new GameObject[numUnits];
            for(int j = 0; j < numUnits; j++)
            {
                Vector2 randPos = Random.insideUnitCircle;
                Vector3 spawnPos = unitLocations[i].position + new Vector3(randPos.x, 0.0f, randPos.y);
                newSquad[j] = Instantiate(unit, spawnPos, Quaternion.identity, unitLocations[i]);
                newSquad[j].GetComponent<Unit>().SetTeam(i + 1);
                newSquad[j].GetComponent<Unit>().SetUnitType(unitTypes[j]);
                newSquad[j].name = unitTypes[j].ToString();
                newSquad[j].GetComponent<Confidence>().SetConfidence(unitTypeCon[j]);
            }
            squads[i].squadName = "Squad " + (i + 1).ToString();
            squads[i].favoredTerrain = terrains[i];
            squads[i].squadMembers = newSquad;
        }
    }
}
