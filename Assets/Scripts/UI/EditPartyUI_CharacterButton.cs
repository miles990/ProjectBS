using UnityEngine;
using KahaGameCore.Static;

namespace ProjectBS.UI
{
    public class EditPartyUI_CharacterButton : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Text m_characterButtonText = null;

        public void SetUp(Data.OwningCharacterData characterData)
        {
            m_characterButtonText.text = 
                string.Format("{0}\nHP: {1}({2}) ATK: {3}({4}) DEF: {5}({6}) SPD: {7}({8})\nSKILL: {9}, {10}, {11}, {12}",
                ContextConverter.Instance.GetContext(characterData.CharacterNameID),
                characterData.HP,
                GameDataManager.GetGameData<Data.AbilityData>(characterData.HPAbilityID).RankString,
                characterData.Attack,
                GameDataManager.GetGameData<Data.AbilityData>(characterData.AttackAbilityID).RankString,
                characterData.Defence,
                GameDataManager.GetGameData<Data.AbilityData>(characterData.DefenceAbilityID).RankString,
                characterData.Speed,
                GameDataManager.GetGameData<Data.AbilityData>(characterData.SpeedAbilityID).RankString,
                ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(characterData.SkillSlot_0).NameContextID),
                ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(characterData.SkillSlot_1).NameContextID),
                ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(characterData.SKillSlot_2).NameContextID),
                ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(characterData.SKillSlot_3).NameContextID));
        }
    }
}
