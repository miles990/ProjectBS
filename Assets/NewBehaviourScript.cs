using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ProjectBS.PlayerManager.Instance.Init();
        ProjectBS.Combat.CombatManager.Instance.StartCombat(ProjectBS.PlayerManager.Instance.Player.Party, new ProjectBS.Data.BossData());
    }
}
