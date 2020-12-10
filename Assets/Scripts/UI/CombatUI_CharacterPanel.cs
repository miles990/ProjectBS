using UnityEngine;
using UnityEngine.UI;
using KahaGameCore.Static;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace ProjectBS.UI
{
    public class CombatUI_CharacterPanel : MonoBehaviour, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler
    {
        [SerializeField] private Button m_button = null;
        [SerializeField] private Text m_infoText = null;
        [SerializeField] private Text m_hatePersentText = null;
        [SerializeField] private GameObject m_actingHint = null;

        private string m_refUnitUDID = "";

        private float m_showInfoTimer = 0f;

        public void SetEnable(bool enable)
        {
            m_button.interactable = enable;
        }

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

            m_infoText.text = string.Format("{0}\nHP: {1} / {2}\nSP: {3} / {4}\nATK: {5}\nDEF: {6}\nSPD: {7}\n{8}\n{9}\n{10}\n{11}",
                _unit.name,
                _unit.HP,
                _unit.GetMaxHP(),
                _unit.SP,
                100,
                _unit.GetAttack(),
                _unit.GetDefense(),
                _unit.GetSpeed(),
                _skillName[0],
                _skillName[1],
                _skillName[2],
                _skillName[3]);

            List<Combat.CombatUnit> _allUnits = Combat.CombatUtility.ComabtManager.AllUnit;
            if (_allUnits.Contains(Combat.CombatUtility.ComabtManager.GetUnitByUDID(m_refUnitUDID)))
            {
                int _totalHaterd = 0;
                for (int i = 0; i < _allUnits.Count; i++)
                {
                    if (_allUnits[i].camp == _unit.camp)
                    {
                        _totalHaterd += _allUnits[i].Hatred;
                    }
                }

                float _haterdPersent = (float)_unit.Hatred / (float)_totalHaterd;
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
            SetUp(m_refUnitUDID);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            m_showInfoTimer = GameDataManager.GameProperties.PressDownShowInfoTime;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_showInfoTimer = 0f;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            m_showInfoTimer = 0f;
        }

        private void Update()
        {
            if (m_showInfoTimer > 0f)
            {
                m_showInfoTimer -= Time.deltaTime;
                if (m_showInfoTimer <= 0f)
                {
                    Combat.CombatUnit _unit = Combat.CombatUtility.ComabtManager.GetUnitByUDID(m_refUnitUDID);

                    m_showInfoTimer = 0f;
                    string _buffString = "";
                    for(int i = 0; i < _unit.OwnBuffCount; i++)
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
    }
}
