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
            public List<CombatUI_CharacterPanel> targets = default;
            public Dictionary<CombatUI_CharacterPanel, int> targetToDmg = new Dictionary<CombatUI_CharacterPanel, int>();

            public void Process()
            {
                switch(commandName)
                {
                    case "Fire":
                        {
                            throw new System.Exception("Fire");
                        }
                    case "Show":
                        {
                            if (commandParas[1] == "Target")
                            {
                                for(int i = 0; i < targets.Count; i++)
                                {
                                    GameObject _clone = Instantiate(Resources.Load<GameObject>(commandParas[0]));
                                    Destroy(_clone, 1f);
                                    _clone.transform.position = targets[i].transform.position;
                                }
                            }
                            else if (commandParas[1] == "Caster")
                            {
                                GameObject _clone = Instantiate(Resources.Load<GameObject>(commandParas[0]));
                                Destroy(_clone, 1f);
                                _clone.transform.position = casterPos;
                            }
                            break;
                        }
                    case "Shake":
                        {
                            CombatUtility.ComabtManager.Shake();
                            break;
                        }
                    case "Damage":
                        {
                            for (int i = 0; i < targets.Count; i++)
                            {
                                float _persent = float.Parse(commandParas[0]);
                                float _dmg = (float)targetToDmg[targets[i]] * _persent;

                                if (_dmg < 1f) _dmg = 1f;

                                targets[i].ShowDamage(System.Convert.ToInt32(_dmg));
                            }

                            break;
                        }
                }
            }
        }

        [SerializeField] private Animator m_animator = null;
        [SerializeField] private Animator m_infoAnimator = null;
        [SerializeField] private TextMeshProUGUI m_infoText = null;
        [SerializeField] private Animator m_dmgAnimator = null;
        [SerializeField] private TextMeshProUGUI m_dmgText = null;
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

        public void ShowDamage(int dmg)
        {
            m_dmgText.text = "-" + dmg;
            m_dmgAnimator.gameObject.SetActive(true);
            m_dmgAnimator.Play("Show", 0, 0f);
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
            }
        }

        public class AnimationData
        {
            public Vector3 casterPos = default;
            public List<CombatUI_CharacterPanel> targets = new List<CombatUI_CharacterPanel>();
            public int skillID = 0;
            public Dictionary<CombatUI_CharacterPanel, int> targetToDmg = new Dictionary<CombatUI_CharacterPanel, int>();
            public System.Action onEnded = null;
        }

        public void PlaySkillAni(AnimationData animationData)
        {
            string _info = GameDataManager.GetGameData<Data.SkillData>(animationData.skillID).AnimationInfo;

            if(string.IsNullOrEmpty(_info))
            {
                ForceShowDamageWithAnimationData(animationData);
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
                    targets = animationData.targets,
                    targetToDmg = animationData.targetToDmg
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
                    ForceShowDamageWithAnimationData(animationData);
                });
        }

        private void ForceShowDamageWithAnimationData(AnimationData animationData)
        {
            if (animationData.targetToDmg.Count > 0)
            {
                for (int i = 0; i < animationData.targets.Count; i++)
                {
                    if (!animationData.targets[i].m_dmgAnimator.gameObject.activeSelf)
                    {
                        animationData.targets[i].ShowDamage(animationData.targetToDmg[animationData.targets[i]]);
                        if (i == 0)
                        {
                            TimerManager.Schedule(0.75f, delegate
                            {
                                for (int j = 0; j < animationData.targets.Count; j++)
                                {
                                    animationData.targets[j].SetDamageIn(animationData.onEnded);
                                }
                            });
                        }
                    }
                    else
                    {
                        animationData.targets[i].SetDamageIn(animationData.onEnded);
                    }
                }
            }
            else
            {
                m_animator.enabled = false;
                animationData.onEnded?.Invoke();
            }
        }

        private void SetDamageIn(System.Action onEnded)
        {
            m_dmgAnimator.Play("SetIn", 0, 0f);
            TimerManager.Schedule(0.6f, delegate
            {
                m_dmgAnimator.gameObject.SetActive(false);
                onEnded?.Invoke();
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
