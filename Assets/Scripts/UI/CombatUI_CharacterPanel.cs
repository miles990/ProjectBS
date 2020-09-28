using UnityEngine;
using UnityEngine.UI;
using KahaGameCore.Static;
using System.Collections.Generic;

namespace ProjectBS.UI
{
    public class CombatUI_CharacterPanel : MonoBehaviour
    {
        [SerializeField] private Button m_button = null;
        [SerializeField] private Text m_infoText = null;
        [SerializeField] private Text m_hatePersentText = null;

        private Combat.CombatUnit m_currnetDisplayingUnit = null;

        public void SetEnable(bool enable)
        {
            m_button.interactable = enable;
        }

        public void SetUp(Combat.CombatUnit unit)
        {
            m_currnetDisplayingUnit = unit;

            string[] _skills = unit.skills.Split(',');
            string[] _skillName = new string[4];
            for(int i = 0; i < _skills.Length; i++)
            {
                if (_skills[i] == "")
                    _skillName[i] = "無";
                else
                    _skillName[i] = ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(int.Parse(_skills[i])).NameContextID);
            }

            m_infoText.text = string.Format("{0}\nHP: {1} / {2}\nSP: {3} / {4}\nATK: {5}\nDEF: {6}\nSPD: {7}\n{8}\n{9}\n{10}\n{11}",
                unit.name,
                unit.HP,
                unit.GetMaxHP(),
                unit.SP,
                100,
                unit.GetAttack(),
                unit.GetDefence(),
                unit.GetSpeed(),
                _skillName[0],
                _skillName[1],
                _skillName[2],
                _skillName[3]);

            if(m_currnetDisplayingUnit.HP > 0)
            {
                List<Combat.CombatUnit> _allUnits = Combat.CombatUtility.CurrentComabtManager.AllUnit;
                int _totalHaterd = 0;
                for (int i = 0; i < _allUnits.Count; i++)
                {
                    if (_allUnits[i].camp == m_currnetDisplayingUnit.camp)
                    {
                        _totalHaterd += _allUnits[i].Hatred;
                    }
                }

                float _haterdPersent = (float)m_currnetDisplayingUnit.Hatred / (float)_totalHaterd;
                m_hatePersentText.text = System.Convert.ToInt32((_haterdPersent * 100f)) + "%";
                m_hatePersentText.gameObject.SetActive(true);
            }
            else
            {
                m_hatePersentText.gameObject.SetActive(false);
            }
        }

        public void RefreshInfo()
        {
            SetUp(m_currnetDisplayingUnit);
        }
    }
}
