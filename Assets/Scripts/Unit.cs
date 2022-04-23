using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum UnitType { NONE, HUNTER, GUARD, CAPTAIN, SUPPORT }

public class Unit : MonoBehaviour
{
    [SerializeField]
    private UnitType type;
    [SerializeField]
    private float radius;
    [SerializeField]
    private int team;
    [SerializeField]
    private Transform currentTerrain;

    private Confidence confidence;

    public void SetUnitType(UnitType _type) { type = _type; }

    public void SetTeam(int _team) { team = _team; }

    private void Start()
    {
        confidence = GetComponent<Confidence>();
        switch (team) 
        {
            case 1:
                GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
                break;
            case 2:
                GetComponent<MeshRenderer>().material.SetColor("_Color", Color.green);
                break;
            case 3:
                GetComponent<MeshRenderer>().material.SetColor("_Color", Color.blue);
                break;
            case 4:
                GetComponent<MeshRenderer>().material.SetColor("_Color", Color.yellow);
                break;
            default:
                break;
        }
        currentTerrain = CheckTerrain();
        if(currentTerrain)
            SetTerrainConfidence(currentTerrain);
    }

    private void Update()
    {
        Transform newTerrian = CheckTerrain();
        if (newTerrian != currentTerrain)
        {
            Debug.Log("Diff");
            if (newTerrian)
            {
                SetTerrainConfidence(newTerrian);
                currentTerrain = newTerrian;
            }
        }
        else
        {
            Debug.Log("Same");
            Vector3 newPos = RandomNavSphere(transform.position, radius, -1);
            SetDestination(newPos);
        }
    }

    private Transform CheckTerrain()
    {
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("Ground");
        //Debug.Log(mask.value);
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            //Debug.Log("Hit");
            //Debug.Log(hit.collider.gameObject.layer);
            if (hit.collider.gameObject.layer == mask.value)
            {
                return hit.collider.gameObject.transform;
            }
        }
        return null;
    }

    private void SetTerrainConfidence(Transform _terrain)
    {
        switch (_terrain.gameObject.tag)
        {
            case "Tundra":
                confidence.DecreaseCon(0.2f);
                break;
            case "City":
                confidence.IncreaseCon(0.3f);
                break;
            case "Desert":
                confidence.DecreaseCon(0.1f);
                break;
            case "Forest":
                confidence.IncreaseCon(0.2f);
                break;
            case "Swamp":
                confidence.DecreaseCon(0.3f);
                break;
            case "Canyon":
                confidence.IncreaseCon(0.4f);
                break;
            case "Beach":
                confidence.IncreaseCon(0.1f);
                break;
            case "Lake":
                confidence.DecreaseCon(0.4f);
                break;
            case "Hills":
                confidence.IncreaseCon(0.25f);
                break;
            default:
                break;
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    private void SetDestination(Vector3 destination)
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        if (agent)
        {
            agent.destination = destination;
        }
    }
}
