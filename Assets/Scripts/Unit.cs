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
    }

    private void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        LayerMask mask = LayerMask.GetMask("Ground");
        if (Physics.CheckSphere(transform.position, radius, mask))
        {
            if (collision.gameObject.layer == mask)
            {
                Debug.Log("Hit");
                switch (collision.gameObject.tag)
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
                SetDestination(collision.transform.position);
            }
        }
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
