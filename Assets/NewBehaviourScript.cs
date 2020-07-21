using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public float t;

    // Start is called before the first frame update
    void Start()
    {
        t = ProjectBS.Combat.CombatUtility.Calculate(
            new ProjectBS.Combat.CombatUtility.CalculateData
            {
                caster = null,
                target = null,
                formula = "1.4*1.3+1.2"
            }
            );
    }
}
