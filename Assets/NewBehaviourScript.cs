using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        ProjectBS.GameDataLoader.StartLoad();
        ProjectBS.PlayerManager.Instance.Init();

        ProjectBS.Combat.CombatManager.Instance.StartCombat(
            ProjectBS.PlayerManager.Instance.Player.Party,
            KahaGameCore.Static.GameDataManager.GetGameData<ProjectBS.Data.BossData>(1));
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            List<ProjectBS.Data.OwningCharacterData> _character = ProjectBS.PlayerManager.Instance.Player.Characters;
            Debug.Log("==========DISPLAY INFO OwningCharacterData==========");
            for (int i = 0; i < _character.Count; i++)
            {

            }
            Debug.Log("====================================================");
        }
    }
}
