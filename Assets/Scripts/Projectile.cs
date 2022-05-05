using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public int team;

    private void OnCollisionEnter(Collision collision)
    {
        Unit _unit = collision.gameObject.GetComponent<Unit>();
        if (_unit)
        {
            if(_unit.GetTeam() != team)
            {
                //Debug.Log("Ow");
                _unit.DamageUnit(10);
                Destroy(this.gameObject);
            }
        }
        Destroy(this.gameObject, 2.0f);
    }
}
