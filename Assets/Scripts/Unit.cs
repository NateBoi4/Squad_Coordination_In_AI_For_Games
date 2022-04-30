using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum UnitType { NONE, HUNTER, GUARD, CAPTAIN, SUPPORT, TANK, SNIPER }

public struct ForwardLine
{
    public Vector3 combatDir;
    public float width;
    public Rect neutralArea;
    public Rect enemyArea;
}

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
    [SerializeField]
    private GameObject currentTarget;
    [SerializeField]
    private bool leader;
    [SerializeField]
    private GameObject leadUnit;
    [SerializeField]
    private Transform dest;
    [SerializeField]
    private bool moving;
    [SerializeField]
    private float aggro;
    [SerializeField]
    private ForwardLine forwardLine;
    [SerializeField]
    private float averageConfidence;
    [SerializeField]
    private int unitCount;

    private Confidence confidence;

    public UnitManager manager;

    public void SetUnitType(UnitType _type) { type = _type; }

    public void SetTeam(int _team) { team = _team; }
    public int GetTeam() { return team; }

    private void Start()
    {
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<UnitManager>();
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
            case 5:
                GetComponent<MeshRenderer>().material.SetColor("_Color", Color.magenta);
                break;
            case 6:
                GetComponent<MeshRenderer>().material.SetColor("_Color", Color.cyan);
                break;
            default:
                break;
        }
        currentTerrain = CheckTerrain();
        if(currentTerrain)
            SetTerrainConfidence(currentTerrain);
        if (type == UnitType.CAPTAIN)
        {
            unitCount = manager.numUnits;
            SetLeader(unitCount);
        }
        else
        {
            foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit"))
            {
                if (unit)
                {
                    Unit _unit = unit.GetComponent<Unit>();
                    if (_unit)
                    {
                        if (_unit.GetTeam() == team)
                        {
                            if (_unit.type == UnitType.CAPTAIN)
                            {
                                leadUnit = unit;
                            }
                        }
                    }
                }
            }
        }
    }

    private Vector3 GetMeanVector(List<Vector3> positions)
    {
        if (positions.Count == 0)
        {
            return Vector3.zero;
        }

        Vector3 meanVector = Vector3.zero;

        foreach (Vector3 pos in positions)
        {
            meanVector += pos;
        }

        return (meanVector / positions.Count);
    }

    private void Update()
    {
        Transform newTerrian = CheckTerrain();
        if (newTerrian != currentTerrain)
        {
            //Debug.Log("Diff");
            if (newTerrian)
            {
                SetTerrainConfidence(newTerrian);
                currentTerrain = newTerrian;
            }
        }
        else
        {
            //Debug.Log("Same");
            if (leader)
            {
                if (!moving)
                {
                    StartCoroutine(Wander());
                    moving = true;
                }
                foreach (Transform t in transform.parent.GetComponentInChildren<Transform>())
                {
                    averageConfidence += t.gameObject.GetComponent<Confidence>().GetConfidence();
                }
                averageConfidence /= manager.numUnits;
            }
            else
            {
                switch (type)
                {
                    case UnitType.HUNTER:
                        dest = leadUnit.transform.GetChild(0);
                        break;
                    case UnitType.GUARD:
                        dest = leadUnit.transform.GetChild(1);
                        break;
                    case UnitType.SUPPORT:
                        dest = leadUnit.transform.GetChild(2);
                        break;
                    case UnitType.SNIPER:
                        dest = leadUnit.transform.GetChild(3);
                        break;
                    case UnitType.TANK:
                        dest = leadUnit.transform.GetChild(4);
                        break;
                    default:
                        break;
                }
                SetDestination(dest.position);
            }
        }
        CheckEnemies();
    }

    private void CheckEnemies()
    {
        foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit"))
        {
            if (unit)
            {
                Unit _unit = unit.GetComponent<Unit>();
                if (_unit)
                {
                    if (_unit.GetTeam() != team)
                    {
                        if (Vector3.Distance(unit.transform.position, transform.position) < aggro)
                        {
                            //Debug.Log("Target Found");
                            currentTarget = unit;
                            StopAllCoroutines();
                            if (leader) 
                            {
                                transform.LookAt(currentTarget.GetComponent<Unit>().leadUnit.transform);
                                CalculateCombatDirection();
                                forwardLine.width = 6;
                                //forwardLine.neutralArea = new Rect(transform.position.x, transform.position.z - forwardLine.width,
                                //    forwardLine.width, forwardLine.combatDir.magnitude);
                                AdjustCondfidence();
                                DetermineAction();
                            }
                        }
                    }
                }
            }
        }
    }

    private void AdjustCondfidence()
    {
        int enemyUnits = currentTarget.GetComponent<Unit>().leadUnit.GetComponent<Unit>().unitCount;
        int diff = unitCount - enemyUnits;
        switch (diff)
        {
            case 0:
                break;
            case 1:
                averageConfidence -= 0.1f;
                break;
            case 2:
                averageConfidence -= 0.2f;
                break;
            case 3:
                averageConfidence -= 0.3f;
                break;
            default:
                break;
        }
    }

    private void DetermineAction()
    {
        float enemyConfidence = currentTarget.GetComponent<Unit>().leadUnit.gameObject.GetComponent<Unit>().averageConfidence;
        if(averageConfidence < (enemyConfidence - enemyConfidence * 0.5))
        {
            Retreat();
        }
        else if(averageConfidence > (enemyConfidence + enemyConfidence * 0.5))
        {
            PressAttack();
        }
        else
        {
            EngageCombat();
        }
    }

    private void EngageCombat()
    {
        Debug.Log("Combat");
    }

    private void Retreat()
    {
        Debug.Log("Retreat");
    }

    private void PressAttack()
    {
        Debug.Log("Advantage");
    }

    private void CalculateCombatDirection()
    {
        List<Vector3> positions = new List<Vector3>();
        foreach (Transform t in transform.parent.GetComponentsInChildren<Transform>())
        {
            positions.Add(t.position);
        }
        Vector3 averagePos = GetMeanVector(positions);
        positions = new List<Vector3>();
        foreach (Transform t in currentTarget.transform.parent.GetComponentsInChildren<Transform>())
        {
            positions.Add(t.position);
        }
        Vector3 averageEnemyPos = GetMeanVector(positions);
        forwardLine.combatDir = averagePos - averageEnemyPos;
        Debug.DrawLine(averagePos, averageEnemyPos);
    }

    private void SetLeader(int unitNum)
    {
        leader = true;
        for (int i = 0; i < unitNum; i++)
        {
            GameObject pos = new GameObject();
            pos.transform.parent = transform;
            pos.transform.localPosition = Vector3.zero;
            pos.transform.Translate(new Vector3(-i - 0.5f, 0.0f, -(Mathf.Pow(-1.0f, i)) - 0.5f));
            pos.name = "Tracking Location: " + (i + 1);
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

    private IEnumerator Wander()
    {
        while (leader)
        {
            Vector3 newPos = RandomNavSphere(transform.position, radius, -1);
            SetDestination(newPos);
            yield return new WaitForSeconds(3.0f);
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

    void OnDrawGizmos()
    {
        // Green
        Gizmos.color = new Color(0.0f, 1.0f, 0.0f);
        if(forwardLine.neutralArea != null)
            DrawRect(forwardLine.neutralArea);
    }

    void OnDrawGizmosSelected()
    {
        // Orange
        Gizmos.color = new Color(1.0f, 0.5f, 0.0f);
        if(forwardLine.neutralArea != null)
            DrawRect(forwardLine.neutralArea);
    }

    void DrawRect(Rect rect)
    {
        Gizmos.DrawWireCube(new Vector3(rect.center.x, 0.01f, rect.center.y), new Vector3(rect.size.x, 0.01f, rect.size.y));
    }
}
