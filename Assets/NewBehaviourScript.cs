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
            new List<ProjectBS.Data.BossData> 
            {
                KahaGameCore.Static.GameDataManager.GetGameData<ProjectBS.Data.BossData>(1),
                KahaGameCore.Static.GameDataManager.GetGameData<ProjectBS.Data.BossData>(1),
                KahaGameCore.Static.GameDataManager.GetGameData<ProjectBS.Data.BossData>(1)
            });
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            List<ProjectBS.Data.OwningCharacterData> _character = ProjectBS.PlayerManager.Instance.Player.Characters;
            //Debug.Log("==========DISPLAY INFO OwningCharacterData==========");
            for (int i = 0; i < _character.Count; i++)
            {
                Debug.LogFormat("{0}\nHP:{1}({2}), Atk:{3}({4}), Def:{5}({6}), Spd={7}({8})",
                    _character[i].CharacterNameID,
                    _character[i].HP, 
                    KahaGameCore.Static.GameDataManager.GetGameData<ProjectBS.Data.AbilityData>(_character[i].HPAbilityID).RankString,
                    _character[i].Attack,
                    KahaGameCore.Static.GameDataManager.GetGameData<ProjectBS.Data.AbilityData>(_character[i].AttackAbilityID).RankString, 
                    _character[i].Defence,
                    KahaGameCore.Static.GameDataManager.GetGameData<ProjectBS.Data.AbilityData>(_character[i].DefenceAbilityID).RankString, 
                    _character[i].Speed,
                    KahaGameCore.Static.GameDataManager.GetGameData<ProjectBS.Data.AbilityData>(_character[i].SpeedAbilityID).RankString);
            }
            //Debug.Log("====================================================");
        }

        if(Input.GetKeyDown(KeyCode.A))
        {
            ProjectBS.Data.OwningCharacterData _newChar = ProjectBS.CharacterUtility.CreateNewCharacter();
            ProjectBS.CharacterUtility.LevelUp(_newChar, 100);
            Debug.LogFormat("{0}\nHP:{1}({2}), Atk:{3}({4}), Def:{5}({6}), Spd={7}({8})",
                _newChar.CharacterNameID,
                _newChar.HP,
                KahaGameCore.Static.GameDataManager.GetGameData<ProjectBS.Data.AbilityData>(_newChar.HPAbilityID).RankString,
                _newChar.Attack,
                KahaGameCore.Static.GameDataManager.GetGameData<ProjectBS.Data.AbilityData>(_newChar.AttackAbilityID).RankString,
                _newChar.Defence,
                KahaGameCore.Static.GameDataManager.GetGameData<ProjectBS.Data.AbilityData>(_newChar.DefenceAbilityID).RankString,
                _newChar.Speed,
                KahaGameCore.Static.GameDataManager.GetGameData<ProjectBS.Data.AbilityData>(_newChar.SpeedAbilityID).RankString);
        }
    }
}
