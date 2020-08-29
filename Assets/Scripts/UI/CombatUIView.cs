using ProjectBS.Combat;
using KahaGameCore.Interface;
using System;
using System.Collections.Generic;
using UnityEngine;
using KahaGameCore.Static;

namespace ProjectBS.UI
{
    public class CombatUIView : UIView
    {
        public override bool IsShowing => throw new NotImplementedException();

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

        private List<Data.SkillData> m_currentShowingSkills = null;

        // 0~3:Player 4~8:Boss
        private Dictionary<int, CombatUnit> m_indexToUnit = new Dictionary<int, CombatUnit>();
        private Dictionary<CombatUnit, int> m_unitToIndex = new Dictionary<CombatUnit, int>();
        private Dictionary<int, bool> m_indexToEnableState = new Dictionary<int, bool>();

        private SelectTargetData m_currentSelectData = null;
        private List<CombatUnit> m_currentSelectedTargets = new List<CombatUnit>();
        private Action<List<CombatUnit>> m_onSelected = null;

        public override void ForceShow(Manager manager, bool show)
        {
            throw new NotImplementedException();
        }

        public override void Show(Manager manager, bool show, Action onCompleted)
        {
            onCompleted?.Invoke();
        }

        public void InitBattleUnits(List<CombatUnit> units)
        {
            int _currentPlayerIndex = 0;
            int _currentBossIndex = 4;

            for (int i = 0; i < units.Count; i++)
            { 
                if (units[i].camp == CombatUnit.Camp.Player)
                {
                    m_indexToUnit.Add(_currentPlayerIndex, units[i]);
                    m_unitToIndex.Add(units[i], _currentPlayerIndex);
                    m_indexToEnableState.Add(_currentPlayerIndex, false);
                    _currentPlayerIndex++;
                }
                else
                {
                    m_indexToUnit.Add(_currentBossIndex, units[i]);
                    m_unitToIndex.Add(units[i], _currentBossIndex);
                    m_indexToEnableState.Add(_currentBossIndex, false);
                    _currentBossIndex++;
                }
            }
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
            Debug.LogFormat("HP:{0}/{1}, Atk:{2}, Def:{3}, Spd={4}", actor.HP, actor.GetMaxHP(), actor.GetAttack(), actor.GetDefence(), actor.GetSpeed());
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
            Debug.Log("------------------------------");
            Debug.Log("開始選擇技能");

            m_currentShowingSkills = datas;

            for (int i = 0; i < m_currentShowingSkills.Count; i++)
            {
                Debug.LogFormat("Skill {0}[Key={1}]: {2}\n{3}", 
                    i, 
                    i + 1,
                    ContextConverter.Instance.GetContext(m_currentShowingSkills[i].NameContextID),
                    ContextConverter.Instance.GetContext(m_currentShowingSkills[i].DescriptionContextID));
            }
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

            Debug.Log("已選擇技能 " + ContextConverter.Instance.GetContext(_selectedSkill.NameContextID));
            OnSkillSelected?.Invoke(_selectedSkill);
        }

        public void Button_SelectCharacter(int index)
        {
            if(!m_indexToUnit.ContainsKey(index))
            {
                return;
            }

            if(!m_indexToEnableState[index])
            {
                return;
            }

            Debug.Log("已選擇目標 " + m_indexToUnit[index].name);
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
            public string attackerName = "Unknown Character";
            public string skillName = "Unknown Skill";
            public string defenderName = "Unknown Character";
            public int damageValue = 0;
        }

        public void DisplayDamage(DisplayDamageData data)
        {
            Debug.LogFormat("{0} 使用 {1} 攻擊 {2}，造成 {3} 傷害",
                    data.attackerName,
                    data.skillName,
                    data.defenderName,
                    data.damageValue
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

        private string TEST_GetKeyBoardKey(int index)
        {
            switch(index)
            {
                case 0:
                    return "Q";
                case 1:
                    return "W";
                case 2:
                    return "E";
                case 3:
                    return "R";
                case 4:
                    return "T";
                case 5:
                    return "Y";
                case 6:
                    return "U";
                case 7:
                    return "I";
                default:
                    return "";
            }
        }

        private void EnableSelectBossButton(bool enable)
        {
            for(int i = 4; i < 9; i++)
            {
                if(m_indexToUnit.ContainsKey(i))
                {
                    if(enable)
                        Debug.LogFormat("可選擇目標[Key={0}]:" + m_indexToUnit[i].name, TEST_GetKeyBoardKey(i));
                    m_indexToEnableState[i] = enable;
                }
            }
        }

        private void EnableSelectPlayerButton(bool enable)
        {
            for (int i = 0; i < 4; i++)
            {
                if (m_indexToUnit.ContainsKey(i))
                {
                    if (enable)
                        Debug.LogFormat("可選擇目標[Key={0}]:" + m_indexToUnit[i].name, TEST_GetKeyBoardKey(i));
                    m_indexToEnableState[i] = enable;
                }
            }
        }

        private void CompleteSelect()
        {
            m_currentSelectData = null;
            m_onSelected?.Invoke(m_currentSelectedTargets);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                Button_SelectSkill(0);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Button_SelectSkill(1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Button_SelectSkill(2);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Button_SelectSkill(3);
            }

            if(Input.GetKeyDown(KeyCode.Q))
            {
                Button_SelectCharacter(0);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                Button_SelectCharacter(1);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Button_SelectCharacter(2);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Button_SelectCharacter(3);
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                Button_SelectCharacter(4);
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                Button_SelectCharacter(5);
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                Button_SelectCharacter(6);
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                Button_SelectCharacter(7);
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                Button_SelectCharacter(8);
            }

            if(Input.GetKeyDown(KeyCode.Space))
            {
                List<CombatUnit> units = new List<CombatUnit>(m_unitToIndex.Keys);
                Debug.Log("==========DISPLAY INFO==========");
                for(int i = 0; i < units.Count; i++)
                {
                    Debug.LogFormat("{0}\nHP:{1}/{2}, Atk:{3}, Def:{4}, Spd={5}", units[i].name, units[i].HP, units[i].GetMaxHP(), units[i].GetAttack(), units[i].GetDefence(), units[i].GetSpeed());

                    for (int j = 0; j < units[i].buffs.Count; j++)
                    {
                        Data.SkillEffectData _effect = GameDataManager.GetGameData<Data.SkillEffectData>(units[i].buffs[j].effectID);
                        Debug.LogFormat("{0}x{1}[{2}]\n{3}", ContextConverter.Instance.GetContext(_effect.NameContextID), units[i].buffs[j].stackCount, units[i].buffs[j].remainingTime, ContextConverter.Instance.GetContext(_effect.DescriptionContextID));
                    }
                }
                Debug.Log("================================");
            }
        }
    }
}
