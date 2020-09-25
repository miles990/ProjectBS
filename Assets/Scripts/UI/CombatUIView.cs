using ProjectBS.Combat;
using KahaGameCore.Interface;
using System;
using System.Collections.Generic;
using UnityEngine;
using KahaGameCore.Static;
using UnityEngine.UI;

namespace ProjectBS.UI
{
    public class CombatUIView : UIView
    {
        public override bool IsShowing { get { return m_isShowing; } }
        private bool m_isShowing = false;

        public event Action OnTurnStartAnimationEnded = null;
        public event Action OnActionAnimationEnded = null;
        public event Action<Data.SkillData> OnSkillSelected = null;

        public class SelectTargetData
        {
            public CombatTargetSelecter.SelectRange selectRange = CombatTargetSelecter.SelectRange.All;
            public CombatUnit attacker = null;
            public int needCount = 0;
            public bool inculdeAttacker = false;
            public Action<List<CombatUnit>> onSelected = null;
        }

        [SerializeField] private CombatUI_CharacterPanel[] m_characterPanels = null;
        [SerializeField] private GameObject m_skillPanel = null;
        [SerializeField] private Button[] m_skillButtons = null;
        [SerializeField] private Text[] m_skillTexts = null;

        private List<Data.SkillData> m_currentShowingSkills = null;

        // 0~3:Player 4~8:Boss
        private Dictionary<int, CombatUnit> m_indexToUnit = new Dictionary<int, CombatUnit>();
        private Dictionary<CombatUnit, int> m_unitToIndex = new Dictionary<CombatUnit, int>();

        private SelectTargetData m_currentSelectData = null;
        private List<CombatUnit> m_currentSelectedTargets = new List<CombatUnit>();
        private Action<List<CombatUnit>> m_onSelected = null;

        private CombatUnit m_currentActor = null;

        public override void ForceShow(Manager manager, bool show)
        {
            throw new NotImplementedException();
        }

        public override void Show(Manager manager, bool show, Action onCompleted)
        {
            m_isShowing = show;
            onCompleted?.Invoke();
        }

        public void InitBattleUnits(List<CombatUnit> units)
        {
            for(int i = 0; i < m_characterPanels.Length; i++)
            {
                m_characterPanels[i].gameObject.SetActive(false);
            }

            int _currentPlayerIndex = 0;
            int _currentBossIndex = 4;

            for (int i = 0; i < units.Count; i++)
            { 
                if (units[i].camp == CombatUnit.Camp.Player)
                {
                    m_indexToUnit.Add(_currentPlayerIndex, units[i]);
                    m_unitToIndex.Add(units[i], _currentPlayerIndex);
                    m_characterPanels[_currentPlayerIndex].SetUp(units[i]);
                    m_characterPanels[_currentPlayerIndex].gameObject.SetActive(true);
                    _currentPlayerIndex++;
                }
                else
                {
                    m_indexToUnit.Add(_currentBossIndex, units[i]);
                    m_unitToIndex.Add(units[i], _currentBossIndex);
                    m_characterPanels[_currentBossIndex].SetUp(units[i]);
                    m_characterPanels[_currentBossIndex].gameObject.SetActive(true);
                    _currentBossIndex++;
                }
            }
        }

        public void ShowForceEndAction(CombatUnit actor)
        {
            Debug.LogFormat("{0} 強制結束行動", actor.name);
        }

        public void ShowUnitDied(CombatUnit dyingUnit)
        {
            Debug.LogFormat("{0} 死亡", dyingUnit.name);
        }

        public void ShowTurnStart(int turnCount)
        {
            Debug.LogFormat("第 {0} 回合 開始", turnCount);
            TimerManager.Schedule(1f, OnTurnStartAnimationEnded);
        }

        public void ShowActorActionStart(CombatUnit actor)
        {
            Debug.LogFormat("{0} 開始行動 UI character index={1}", actor.name, m_unitToIndex[actor]);
            Debug.LogFormat("HP:{0}/{1}, Atk:{2}, Def:{3}, Spd={4}, Hatred={5}", actor.HP, actor.GetMaxHP(), actor.GetAttack(), actor.GetDefence(), actor.GetSpeed(), actor.hatred);
            m_currentActor = actor;
            TimerManager.Schedule(1f, OnActionAnimationEnded);
        }

        public void ShowGameEnd(bool playerWin)
        {
            if (playerWin)
            {
                Debug.Log("Player Win");
            }
            else
            {
                Debug.Log("Player Lose");
            }
        }

        public void RefreshCurrentSkillMenu(List<Data.SkillData> datas)
        {
            for(int i = 0; i < m_skillButtons.Length; i++)
            {
                m_skillButtons[i].gameObject.SetActive(false);
            }
            m_currentShowingSkills = datas;
            for(int i = 0; i < m_currentShowingSkills.Count; i++)
            {
                if (!m_currentShowingSkills[i].Command.Contains(EffectProcesser.TriggerTiming.OnActived.ToString())
                    || m_currentShowingSkills[i].SP > m_currentActor.SP)
                {
                    m_skillButtons[i].interactable = false;
                }
                m_skillTexts[i].text = ContextConverter.Instance.GetContext(m_currentShowingSkills[i].NameContextID);
                m_skillButtons[i].gameObject.SetActive(true);
            }
            m_skillPanel.SetActive(true);
        }

        public void StartSelectTarget(SelectTargetData data)
        {
            if (m_currentSelectData != null)
                return;

            m_currentSelectedTargets.Clear();
            m_currentSelectData = data;
            m_onSelected = data.onSelected;

            WaitPlayerSelect();
        }

        public void Button_SelectSkill(int index)
        {
            if (m_currentShowingSkills == null
                || m_currentShowingSkills.Count <= index
                || m_currentShowingSkills[index] == null)
                return;

            Data.SkillData _selectedSkill = m_currentShowingSkills[index];

            m_currentShowingSkills = null;
            m_skillPanel.SetActive(false);

            OnSkillSelected?.Invoke(_selectedSkill);
        }

        public void Button_SelectCharacter(int index)
        {
            if(!m_indexToUnit.ContainsKey(index))
            {
                return;
            }

            m_currentSelectedTargets.Add(m_indexToUnit[index]);

            if(m_currentSelectedTargets.Count < m_currentSelectData.needCount)
            {
                WaitPlayerSelect();
            }
            else
            {
                EnableSelectBossButton(false);
                EnableSelectPlayerButton(false);

                m_currentSelectData = null;
                m_onSelected?.Invoke(m_currentSelectedTargets);
            }
        }

        public class DisplayDamageData
        {
            public CombatUnit taker = null;
            public int damageValue = 0;
        }

        public void DisplayDamage(DisplayDamageData data)
        {
            Debug.LogFormat("{0} 受到 {1} 點傷害",
                    data.taker.name,
                    data.damageValue
                );

            m_characterPanels[m_unitToIndex[data.taker]].SetUp(data.taker);
        }

        public class DisplayHealData
        {
            public CombatUnit taker = null;
            public int healValue = 0;
        }

        public void DisplayHeal(DisplayHealData data)
        {
            Debug.LogFormat("{0} 恢復 {1} 點生命",
                    data.taker.name,
                    data.healValue
                );

            m_characterPanels[m_unitToIndex[data.taker]].SetUp(data.taker);
        }

        public class DisplayGainBuffData
        {
            public CombatUnit taker = null;
            public string buffName = "Unknown Buff";
        }

        public void DisplayGainBuff(DisplayGainBuffData data)
        {
            Debug.LogFormat("{0} 得到狀態: {1}",
                data.taker.name,
                data.buffName
            );
        }

        private void WaitPlayerSelect()
        {
            switch (m_currentSelectData.selectRange)
            {
                case CombatTargetSelecter.SelectRange.All:
                    {
                        EnableSelectBossButton(true);
                        EnableSelectPlayerButton(true);
                        break;
                    }
                case CombatTargetSelecter.SelectRange.Opponent:
                    {
                        if (m_currentSelectData.attacker.camp == CombatUnit.Camp.Boss)
                        {
                            EnableSelectPlayerButton(true);
                        }
                        else
                        {
                            EnableSelectBossButton(true);
                        }
                        break;
                    }
                case CombatTargetSelecter.SelectRange.SameSide:
                    {
                        if (m_currentSelectData.attacker.camp == CombatUnit.Camp.Boss)
                        {
                            EnableSelectBossButton(true);
                        }
                        else
                        {
                            EnableSelectPlayerButton(true);
                        }
                        break;
                    }
            }
        }

        private void EnableSelectBossButton(bool enable)
        {
            for(int i = 4; i < 9; i++)
            {
                if(m_indexToUnit.ContainsKey(i))
                {
                    m_characterPanels[i].SetEnable(enable);
                }
            }
        }

        private void EnableSelectPlayerButton(bool enable)
        {
            for (int i = 0; i < 4; i++)
            {
                if (m_indexToUnit.ContainsKey(i))
                {
                    m_characterPanels[i].SetEnable(enable);
                }
            }
        }
    }
}
