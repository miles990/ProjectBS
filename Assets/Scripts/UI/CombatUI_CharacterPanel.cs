using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using TMPro;
using System;

namespace ProjectBS.Combat
{
    public class CombatUI_CharacterPanel : CombatUI_ButtonBase
    {
        [SerializeField] private TextMeshProUGUI m_hpText = null;
        [SerializeField] private TextMeshProUGUI m_spText = null;
        [SerializeField] private TextMeshProUGUI m_hatePersentText = null;
        [SerializeField] private GameObject m_actingHint = null;

        private string m_refUnitUDID = "";

        public void EnableActingHint(bool enable)
        {
            m_actingHint.SetActive(enable);
        }

        public void SetUp(string unitUDID)
        {
            m_refUnitUDID = unitUDID;
            if(string.IsNullOrEmpty(m_refUnitUDID))
            {
                return;
            }

            Combat.CombatUnit _unit = Combat.CombatUtility.ComabtManager.GetUnitByUDID(m_refUnitUDID);

            string[] _skillName = new string[4];
            for(int i = 0; i < _unit.skills.Length; i++)
            {
                if (_unit.skills[i] == 0)
                    _skillName[i] = "無";
                else
                    _skillName[i] = ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(_unit.skills[i]).NameContextID);
            }

            List<Combat.CombatUnit> _allUnits = Combat.CombatUtility.ComabtManager.AllUnit;
            if (_allUnits.Contains(Combat.CombatUtility.ComabtManager.GetUnitByUDID(m_refUnitUDID)))
            {
                float _haterdPersent = Combat.CombatUtility.GetHatredPersent(_unit);
                m_hatePersentText.text = System.Convert.ToInt32((_haterdPersent * 100f)) + "%";
                m_hatePersentText.gameObject.SetActive(true);
            }
            else
            {
                m_hatePersentText.gameObject.SetActive(false);
            }

            m_hpText.text = _unit.HP.ToString();
            m_spText.text = _unit.SP.ToString();
        }

        public void RefreshInfo()
        {
            SetUp(m_refUnitUDID);
        }

        protected override void OnNeedToShowDetail()
        {
            CombatUnit _unit = CombatUtility.ComabtManager.GetUnitByUDID(m_refUnitUDID);

            string _buffString = "";

            _buffString += string.Format("{0}\nHP: {1} / {2}\nSP: {3} / {4}\nATK: {5}\nDEF: {6}\nSPD: {7}\n\n",
            _unit.name,
            _unit.HP,
            _unit.GetMaxHP(),
            _unit.SP,
            100,
            _unit.GetAttack(),
            _unit.GetDefense(),
            _unit.GetSpeed());

            for (int i = 0; i < _unit.OwnBuffCount; i++)
            {
                Data.BuffData _effect = _unit.GetBuffByIndex(i).GetBuffSourceData();
                _buffString += ContextConverter.Instance.GetContext(_effect.NameContextID) + " x" + _unit.GetBuffByIndex(i).stackCount
                    + " (" + (_unit.GetBuffByIndex(i).remainingTime == -1 ? "永久" : "剩餘 " + _unit.GetBuffByIndex(i).remainingTime.ToString() + " 回合") + ")\n"
                    + ContextConverter.Instance.GetContext(_effect.DescriptionContextID);

                if (i != _unit.OwnBuffCount - 1)
                    _buffString += "\n\n";
            }

            GameManager.Instance.MessageManager.ShowCommonMessage(_buffString, _unit.name, null);
        }
    }
}
