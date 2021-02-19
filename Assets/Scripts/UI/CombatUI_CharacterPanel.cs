using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using ProjectBS.Combat;
using KahaGameCore.Static;

namespace ProjectBS.UI
{
    public class CombatUI_CharacterPanel : CombatUI_ButtonBase
    {
        public enum AnimationClipName
        {
            Appear
        }

        [SerializeField] private Animator m_animator = null;
        [SerializeField] private Image m_hpBar = null;
        [SerializeField] private Image m_spBar = null;
        [SerializeField] private TextMeshProUGUI m_hpText = null;
        [SerializeField] private TextMeshProUGUI m_spText = null;
        [SerializeField] private TextMeshProUGUI m_hatePersentText = null;
        [SerializeField] private GameObject m_actingHint = null;

        private string m_refUnitUDID = "";

        public void EnableActingHint(bool enable)
        {
            m_actingHint.SetActive(enable);
        }

        public void PlayAni(AnimationClipName name)
        {
            m_animator.enabled = true;
            m_animator.Play(name.ToString(), 0, 0f);

            TimerManager.Schedule(1f, delegate { m_animator.enabled = false; });
        }

        public void SetUp(string unitUDID)
        {
            m_refUnitUDID = unitUDID;
            if(string.IsNullOrEmpty(m_refUnitUDID))
            {
                return;
            }

            CombatUnit _unit = CombatUtility.ComabtManager.GetUnitByUDID(m_refUnitUDID);

            string[] _skillName = new string[4];
            for(int i = 0; i < _unit.skills.Length; i++)
            {
                if (_unit.skills[i] == 0)
                    _skillName[i] = "無";
                else
                    _skillName[i] = ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(_unit.skills[i]).NameContextID);
            }

            List<CombatUnit> _allUnits = CombatUtility.ComabtManager.AllUnit;
            if (_allUnits.Contains(CombatUtility.ComabtManager.GetUnitByUDID(m_refUnitUDID)))
            {
                float _haterdPersent = CombatUtility.GetHatredPersent(_unit);
                m_hatePersentText.text = System.Convert.ToInt32((_haterdPersent * 100f)) + "%";
                m_hatePersentText.gameObject.SetActive(true);
            }
            else
            {
                m_hatePersentText.gameObject.SetActive(false);
            }

            m_hpBar.fillAmount = (float)_unit.HP / (float)_unit.GetMaxHP();
            m_spBar.fillAmount = (float)_unit.SP / 100f;

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
