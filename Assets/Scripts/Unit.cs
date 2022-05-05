using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum UnitType { NONE, HUNTER, GUARD, CAPTAIN, SUPPORT, TANK, SNIPER }

[SerializeField]
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
    private bool engaged;
    [SerializeField]
    private float aggro;
    [SerializeField]
    private ForwardLine forwardLine;
    [SerializeField]
    private float averageConfidence;
    [SerializeField]
    private int unitCount;
    [SerializeField]
    private int health;

    private Confidence confidence;

    public UnitManager manager;

    public GameObject projectile;

    public void SetUnitType(UnitType _type) { type = _type; }

    public void SetTeam(int _team) { team = _team; }
    public int GetTeam() { return team; }

    public void DamageUnit(int amount) { health -= amount; }

    private void Start()
    {
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<UnitManager>();
        confidence = GetComponent<Confidence>();
        health = 100;
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

        averageConfidence += confidence.GetConfidence();
        unitCount = manager.numUnits;

        if (type == UnitType.CAPTAIN)
        {
            leader = true;
        }
        else
        {
            leader = false;
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
        if(health <= 0)
        {
            Die();
        }
        if (engaged)
        {
            if (leader && currentTarget == null)
            {
                RemoveLeader(this.gameObject);
            }
            else if (!leader && leadUnit == null || !leader && dest == null)
            {
                engaged = false;
                moving = false;
            }
        }
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
            if (!moving)
            {
                StartCoroutine(Wander());
                moving = true;
            }
            int layerID = 9;
            int layerMask = 1 << layerID;
            Collider[] colliders = Physics.OverlapSphere(transform.position, aggro, layerMask);
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject != this.gameObject)
                {
                    if(collider.gameObject.GetComponent<Unit>().GetTeam() == team)
                    {
                        averageConfidence += collider.gameObject.GetComponent<Confidence>().GetConfidence();
                    }
                }
            }
            averageConfidence /= unitCount;
        }
        if (!engaged)
        {
            CheckEnemies();
        }
    }

    private void Die()
    {
        if (leader)
        {
            RemoveLeader(this.gameObject);
        }
        Destroy(this.gameObject);
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
                            SetLeader(this.gameObject);
                            if (leader) 
                            {
                                StartCoroutine(Combat());
                            }
                        }
                    }
                }
            }
        }
    }

    private IEnumerator Combat()
    {
        while (currentTarget)
        {
            transform.LookAt(currentTarget.transform);
            CalculateCombatDirection();
            forwardLine.width = 6;
            //forwardLine.neutralArea = new Rect(transform.position.x, transform.position.z - forwardLine.width,
            //    forwardLine.width, forwardLine.combatDir.magnitude);
            AdjustCondfidence();
            Formation(this.gameObject);
            DetermineAction();
            yield return new WaitForSeconds(1.0f);
        }
    }

    private void AdjustCondfidence()
    {
        int enemyUnits = currentTarget.GetComponent<Unit>().unitCount;
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

    private void Formation(GameObject lead)
    {
        int currUnit = 0;
        foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit"))
        {
            if (unit && unit != lead)
            {
                Unit _unit = unit.GetComponent<Unit>();
                if (_unit)
                {
                    if (_unit.GetTeam() == team)
                    {
                        _unit.dest = lead.transform.GetChild(currUnit).transform;
                        _unit.SetTargetDestination(_unit.dest.position);
                        currUnit++;
                    }
                }
            }
        }
    }

    private void DetermineAction()
    {
        float enemyConfidence = currentTarget.GetComponent<Unit>().averageConfidence;
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
        //Debug.Log("Combat");
        foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit"))
        {
            if (unit)
            {
                Unit _unit = unit.GetComponent<Unit>();
                if (_unit)
                {
                    if (_unit.GetTeam() == team)
                    {
                        GameObject proj;
                        float forceMultiplier = 20;
                        if (forceMultiplier > 0)
                        {
                            proj = Instantiate(projectile, _unit.gameObject.transform.position + new Vector3(0, 1, 0), unit.transform.rotation);
                            proj.GetComponent<Projectile>().team = team;
                            //proj.transform.LookAt(currentTarget.transform);
                            Vector3 fireDirection = currentTarget.transform.position - unit.transform.position;
                            proj.GetComponent<Rigidbody>().velocity = fireDirection.normalized * forceMultiplier;
                            //proj.GetComponent<Rigidbody>().velocity.y = 0;
                            //proj.GetComponent<Rigidbody>().AddRelativeForce(fireDirection * forceMultiplier, ForceMode.Acceleration);
                        }
                    }
                }
            }
        }
    }

    private void Retreat()
    {
        //Debug.Log("Retreat");
        Vector3 retreatDest = transform.position - new Vector3(1, 0, 1);
        if (gameObject)
        {
            SetTargetDestination(retreatDest);
        }
        EngageCombat();
    }

    private void PressAttack()
    {
        //Debug.Log("Advantage");
        if (gameObject)
        {
            SetTargetDestination(currentTarget.transform.position);
        }
        EngageCombat();
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

    private void SetLeader(GameObject lead)
    {
        leader = true;
        if (!engaged)
        {
            for (int i = 0; i < (unitCount - 1); i++)
            {
                GameObject pos = new GameObject();
                pos.transform.parent = transform;
                pos.transform.localPosition = Vector3.zero;
                pos.transform.Translate(new Vector3(-i - 0.5f, 0.0f, -(Mathf.Pow(-1.0f, i)) - 0.5f));
                pos.name = "Tracking Location: " + (i + 1);
            }
        }
        engaged = true;
        foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit"))
        {
            if (unit && unit != lead)
            {
                Unit _unit = unit.GetComponent<Unit>();
                if (_unit)
                {
                    if (_unit.GetTeam() == team)
                    {
                        if (_unit.leader)
                        {
                            _unit.leader = false;
                        }
                        _unit.leadUnit = lead;
                        _unit.engaged = true;
                        _unit.StopAllCoroutines();
                    }
                }
            }
        }
    }

    private void RemoveLeader(GameObject lead)
    {
        leader = false;
        engaged = false;
        moving = false;
        for (int i = 0; i < (unitCount - 1); ++i)
        {
            if (i < transform.childCount)
            {
                if (transform.GetChild(i).gameObject)
                    Destroy(transform.GetChild(i).gameObject);
            }
        }
        foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit"))
        {
            if (unit && unit != lead)
            {
                Unit _unit = unit.GetComponent<Unit>();
                if (_unit)
                {
                    if (_unit.GetTeam() == team)
                    {
                        _unit.leadUnit = null;
                        _unit.engaged = false;
                        _unit.moving = false;
                    }
                }
            }
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
        while (!currentTarget)
        {
            Vector3 newPos = RandomNavSphere(transform.position, radius, -1);
            SetTargetDestination(newPos);
            yield return new WaitForSeconds(1.0f);
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

    private void SetTargetDestination(Vector3 destination)
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        if (agent)
        {
            agent.destination = destination;
        }
    }
}
