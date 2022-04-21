using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (Physics.CheckSphere(transform.position, radius, 3))
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
        }
    }
}
