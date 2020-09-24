using KahaGameCore.Static;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectBS.UI
{
    public class EditPartyUI_EditCharacterUI : MonoBehaviour
    {
        [SerializeField] private Text m_nameText = null;
        [SerializeField] private Text m_statusText = null;
        [SerializeField] private InputField m_skill0Field = null;
        [SerializeField] private Text m_skill0NameText = null;
        [SerializeField] private InputField m_skill1Field = null;
        [SerializeField] private Text m_skill1NameText = null;
        [SerializeField] private InputField m_skill2Field = null;
        [SerializeField] private Text m_skill2NameText = null;
        [SerializeField] private InputField m_skill3Field = null;
        [SerializeField] private Text m_skill3NameText = null;

        public void SetUp(Data.OwningCharacterData characterData)
        {
            m_nameText.text = ContextConverter.Instance.GetContext(characterData.CharacterNameID);
            m_statusText.text = string.Format("HP:{0}({1})\nSP:{2}\nAttack:{3}({4})\nDefence:{5}({6})\nSpeed:{7}({8})",
                characterData.HP,
                GameDataManager.GetGameData<Data.AbilityData>(characterData.HPAbilityID).RankString,
                characterData.SP,
                characterData.Attack,
                GameDataManager.GetGameData<Data.AbilityData>(characterData.AttackAbilityID).RankString,
                characterData.Defence,
                GameDataManager.GetGameData<Data.AbilityData>(characterData.DefenceAbilityID).RankString,
                characterData.Speed,
                GameDataManager.GetGameData<Data.AbilityData>(characterData.SpeedAbilityID).RankString);

            m_skill0Field.text = characterData.SkillSlot_0 == 0 ? "" : characterData.SkillSlot_0.ToString();
            m_skill0NameText.text = characterData.SkillSlot_0 == 0 ? "" : ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(characterData.SkillSlot_0).NameContextID);
            m_skill1Field.text = characterData.SkillSlot_1 == 0 ? "" : characterData.SkillSlot_1.ToString();
            m_skill1NameText.text = characterData.SkillSlot_1 == 0 ? "" : ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(characterData.SkillSlot_1).NameContextID);
            m_skill2Field.text = characterData.SkillSlot_2 == 0 ? "" : characterData.SkillSlot_2.ToString();
            m_skill2NameText.text = characterData.SkillSlot_2 == 0 ? "" : ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(characterData.SkillSlot_2).NameContextID);
            m_skill3Field.text = characterData.SkillSlot_3 == 0 ? "" : characterData.SkillSlot_3.ToString();
            m_skill3NameText.text = characterData.SkillSlot_3 == 0 ? "" : ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(characterData.SkillSlot_3).NameContextID);
        }
    }
}
