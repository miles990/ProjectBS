﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //ProjectBS.GameDataLoader.StartLoad();
        //ProjectBS.PlayerManager.Instance.Init();
        //ProjectBS.Combat.CombatManager.Instance.StartCombat(ProjectBS.PlayerManager.Instance.Player.Party, new ProjectBS.Data.BossData());

        float _v = ProjectBS.Combat.CombatUtility.Calculate(
            new ProjectBS.Combat.CombatUtility.CalculateData
            {
                caster = null,
                target = null,
                formula = "-2 + 3 * 4 / (-1 * (5 - 3))",
                useRawValue = false
            });

        Debug.Log(_v);
    }
}
