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

        private class AnimationCommand
        {
            public string commandName = "";
            public List<string> commandParas = new List<string>();
            public Vector3 casterPos = default;
            public Vector3 targetPos = default;

            public void Process()
            {
                switch(commandName)
                {
                    case "Fire":
                        {
                            break;
                        }
                    case "Show":
                        {
                            GameObject _clone = Instantiate(Resources.Load<GameObject>(commandParas[0]));
                            Destroy(_clone, 1f);

                            if (commandParas[1] == "Target")
                                _clone.transform.position = targetPos;
                            else if (commandParas[1] == "Caster")
                                _clone.transform.position = casterPos;
                            else
                                _clone.transform.position = Vector3.zero;

                            break;
                        }
                }
            }
        }

        [SerializeField] private Animator m_animator = null;
        [SerializeField] private Animator m_infoAnimator = null;
        [SerializeField] private TextMeshProUGUI m_infoText = null;
        [SerializeField] private Image m_hpBar = null;
        [SerializeField] private Image m_spBar = null;
        [SerializeField] private TextMeshProUGUI m_hpText = null;
        [SerializeField] private TextMeshProUGUI m_spText = null;
        [SerializeField] private TextMeshProUGUI m_hatePersentText = null;
        [SerializeField] private GameObject m_actingHint = null;

        private string m_refUnitUDID = "";

        private float m_hpBarTartgetValue = 1f;
        private float m_spBarTargetValue = 1f;

        public void EnableActingHint(bool enable)
        {
            m_actingHint.SetActive(enable);
        }

        public void ShowInfo(string info)
        {
            m_infoText.text = info;
            m_infoAnimator.gameObject.SetActive(true);
            m_infoAnimator.Play("Show", 0, 0f);
            TimerManager.Schedule(1.26f, delegate { m_infoAnimator.gameObject.SetActive(false); });
        }

        public void PlayAni(AnimationClipName name)
        {
            m_animator.enabled = true;
            m_animator.Play(name.ToString(), 0, 0f);

            TimerManager.Schedule(1f, 
                delegate 
                {
                    m_animator.enabled = false;
                });

            if (name == AnimationClipName.Appear)
            {
                m_hpBar.fillAmount = 0.25f;
                m_spBar.fillAmount = 0.15f;
                TimerManager.Schedule(1.02f,
                delegate
                {
                    SetUp(m_refUnitUDID);
                });
            }
        }

        public class AnimationData
        {
            public Vector3 casterPos = default;
            public Vector3 targetPos = default;
            public int skillID = 0;
            public System.Action onEnded = null;
        }

        public void PlayAni(AnimationData animationData)
        {
            string _info = GameDataManager.GetGameData<Data.SkillData>(animationData.skillID).AnimationInfo;

            if(string.IsNullOrEmpty(_info))
            {
                animationData.onEnded?.Invoke();
                return;
            }

            string[] _infoParts = _info.Split(';');
            
            string _aniName = _infoParts[0];

            for(int _infoIndex = 1; _infoIndex < _infoParts.Length; _infoIndex++)
            {
                string[] _commandPart = _infoParts[_infoIndex].Split('_');
                float _time = float.Parse(_commandPart[0]);

                AnimationCommand _commandObj = new AnimationCommand
                {
                    commandName = _commandPart[1].Trim(),
                    casterPos = animationData.casterPos,
                    targetPos = animationData.targetPos
                };
                for (int _commandPartIndex = 2; _commandPartIndex < _commandPart.Length; _commandPartIndex++)
                {
                    _commandObj.commandParas.Add(_commandPart[_commandPartIndex].Trim());
                }

                TimerManager.Schedule(_time, _commandObj.Process);
            }

            m_animator.enabled = true;
            m_animator.Play(_aniName.ToString(), 0, 0f);

            TimerManager.Schedule(1.02f,
                delegate 
                {
                    m_animator.enabled = false;
                    animationData.onEnded?.Invoke();
                });
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

            m_hpBarTartgetValue = (float)_unit.HP / (float)_unit.GetMaxHP();
            m_spBarTargetValue = (float)_unit.SP / 100f;

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

        protected override void Update()
        {
            base.Update();
            UpdateBar(m_hpBar, m_hpBarTartgetValue, 0.035f);
            UpdateBar(m_spBar, m_spBarTargetValue, 0.035f);
        }

        private void UpdateBar(Image bar, float target, float spd)
        {
            bar.fillAmount = Mathf.Lerp(bar.fillAmount, target, spd);
            if(Mathf.Abs(bar.fillAmount - target) <= 0.01f)
            {
                bar.fillAmount = target;
            }
        }
    }
}
