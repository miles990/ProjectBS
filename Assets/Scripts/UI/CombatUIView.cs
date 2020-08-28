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
        public enum SelectRange
        {
            SameSide,
            Opponent,
            All
        }

        public enum SelectType
        {
            Manual,
            Random,
            HighestMaxHP,
            HighestHP,
            HighestSP,
            HighestAttack,
            HighestDefence,
            HighestSpeed,
            HighestHatred,
            LowestMaxHP,
            LowestHP,
            LowestSP,
            LowestAttack,
            LowestDefence,
            LowestSpeed,
            LowestHatred,
            RandomByHatred
        }

        public override bool IsShowing => throw new NotImplementedException();

        public event Action OnTurnStartAnimationEnded = null;
        public event Action OnActionAnimationEnded = null;
        public event Action<Data.SkillData> OnSkillSelected = null;

        public class SelectTargetData
        {
            public SelectRange selectRange = SelectRange.All;
            public SelectType selectType = SelectType.Random;
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

        public void ShowTurnStart(int turnCount)
        {
            Debug.LogFormat("第 {0} 回合 開始", turnCount);
            TimerManager.Schedule(1f, OnTurnStartAnimationEnded);
        }

        public void ShowActorActionStart(CombatUnit actor)
        {
            Debug.LogFormat("{0} 開始行動 UI character index={1}", actor.name, m_unitToIndex[actor]);
            TimerManager.Schedule(1f, OnActionAnimationEnded);
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

            DoSelect();
        }

        public void Button_SelectSkill(int index)
        {
            if (m_currentShowingSkills == null
                || m_currentShowingSkills.Count <= index
                || m_currentShowingSkills[index] == null)
                return;

            Debug.Log("已選擇技能 " + ContextConverter.Instance.GetContext(m_currentShowingSkills[index].NameContextID));
            OnSkillSelected?.Invoke(m_currentShowingSkills[index]);
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
                DoSelect();
            }
            else
            {
                EnableSelectBossButton(false);
                EnableSelectPlayerButton(false);
                m_currentSelectData.onSelected?.Invoke(m_currentSelectedTargets);
                m_currentSelectData = null;
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

        private void DoSelect()
        {
            if (m_currentSelectData.needCount == -1)
            {
                SelectAll();
            }
            else 
            {
                switch(m_currentSelectData.selectType)
                {
                    case SelectType.Manual:
                        {
                            WaitPlayerSelect();
                            break;
                        }
                    case SelectType.Random:
                        {
                            RandomSelect();
                            break;
                        }
                    case SelectType.HighestHP:
                        {
                            SelectByStatus(Keyword.HP, true);
                            break;
                        }
                    case SelectType.HighestMaxHP:
                        {
                            SelectByStatus(Keyword.MaxHP, true);
                            break;
                        }
                    case SelectType.HighestSP:
                        {
                            SelectByStatus(Keyword.SP, true);
                            break;
                        }
                    case SelectType.HighestAttack:
                        {
                            SelectByStatus(Keyword.Attack, true);
                            break;
                        }
                    case SelectType.HighestDefence:
                        {
                            SelectByStatus(Keyword.Defence, true);
                            break;
                        }
                    case SelectType.HighestSpeed:
                        {
                            SelectByStatus(Keyword.Speed, true);
                            break;
                        }
                    case SelectType.HighestHatred:
                        {
                            SelectByStatus(Keyword.Hatred, true);
                            break;
                        }
                    case SelectType.LowestHP:
                        {
                            SelectByStatus(Keyword.HP, false);
                            break;
                        }
                    case SelectType.LowestMaxHP:
                        {
                            SelectByStatus(Keyword.MaxHP, false);
                            break;
                        }
                    case SelectType.LowestSP:
                        {
                            SelectByStatus(Keyword.SP, false);
                            break;
                        }
                    case SelectType.LowestAttack:
                        {
                            SelectByStatus(Keyword.Attack, false);
                            break;
                        }
                    case SelectType.LowestDefence:
                        {
                            SelectByStatus(Keyword.Defence, false);
                            break;
                        }
                    case SelectType.LowestSpeed:
                        {
                            SelectByStatus(Keyword.Speed, false);
                            break;
                        }
                    case SelectType.LowestHatred:
                        {
                            SelectByStatus(Keyword.Hatred, false);
                            break;
                        }
                    case SelectType.RandomByHatred:
                        {
                            SelectByRandomByHatred();
                            break;
                        }
                }
            }
        }

        private void SelectByRandomByHatred()
        {
            List<CombatUnit> _allUnits = new List<CombatUnit>(m_unitToIndex.Keys);
            List<CombatUnit> _rollPool = new List<CombatUnit>();
            int _totalHatred = 0;

            for (int i = 0; i < _allUnits.Count; i++)
            {
                switch (m_currentSelectData.selectRange)
                {
                    case SelectRange.All:
                        {
                            _rollPool.Add(_allUnits[i]);
                            _totalHatred += _allUnits[i].hatred;
                            break;
                        }
                    case SelectRange.Opponent:
                        {
                            if (_allUnits[i].camp != m_currentSelectData.attacker.camp)
                            {
                                _rollPool.Add(_allUnits[i]);
                                _totalHatred += _allUnits[i].hatred;
                            }
                            break;
                        }
                    case SelectRange.SameSide:
                        {
                            if (_allUnits[i].camp == m_currentSelectData.attacker.camp)
                            {
                                _rollPool.Add(_allUnits[i]);
                                _totalHatred += _allUnits[i].hatred;
                            }
                            break;
                        }
                }
            }

            int _roll = UnityEngine.Random.Range(0, _totalHatred);
            for (int i = 0; i < _rollPool.Count; i++)
            {
                _roll -= _rollPool[i].hatred;
                if(_roll <= 0)
                {
                    m_currentSelectedTargets.Add(_rollPool[i]);
                    break;
                }
            }
            if (m_currentSelectedTargets.Count == m_currentSelectData.needCount)
            {
                m_currentSelectData.onSelected?.Invoke(m_currentSelectedTargets);
            }
            else
            {
                SelectByRandomByHatred();
            }
        }

        private void SelectByStatus(string statusType, bool getHighest)
        {
            GetIndexRange(out int _min, out int _max);

            List<int> _allKeys = new List<int>(m_indexToUnit.Keys);

            int _currentHighestIndex = _min;
            for (int i = _min + 1; i < _max; i++)
            {
                if ((getHighest && CombatUtility.GetStatusValue(m_indexToUnit[i], statusType, false) > CombatUtility.GetStatusValue(m_indexToUnit[_currentHighestIndex], statusType, false))
                    || (!getHighest && CombatUtility.GetStatusValue(m_indexToUnit[i], statusType, false) < CombatUtility.GetStatusValue(m_indexToUnit[_currentHighestIndex], statusType, false)))
                {
                    _currentHighestIndex = i;
                }
            }

            m_currentSelectedTargets.Add(m_indexToUnit[_currentHighestIndex]);
            if(m_currentSelectedTargets.Count == m_currentSelectData.needCount)
            {
                m_currentSelectData.onSelected?.Invoke(m_currentSelectedTargets);
            }
            else
            {
                SelectByStatus(statusType, getHighest);
            }
        }

        private void SelectAll()
        {
            List<int> _allKeys = new List<int>(m_indexToUnit.Keys);
            switch (m_currentSelectData.selectRange)
            {
                case SelectRange.All:
                    {
                        for(int i = 0; i < _allKeys.Count; i++)
                        {
                            m_currentSelectedTargets.Add(m_indexToUnit[_allKeys[i]]);
                        }
                        break;
                    }
                case SelectRange.Opponent:
                    {
                        if(m_currentSelectData.attacker.camp == CombatUnit.Camp.Boss)
                        {
                            for (int i = 0; i < _allKeys.Count; i++)
                            {
                                if(_allKeys[i] <= 3)
                                {
                                    m_currentSelectedTargets.Add(m_indexToUnit[_allKeys[i]]);
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < _allKeys.Count; i++)
                            {
                                if (_allKeys[i] > 3)
                                {
                                    m_currentSelectedTargets.Add(m_indexToUnit[_allKeys[i]]);
                                }
                            }
                        }
                        break;
                    }
                case SelectRange.SameSide:
                    {
                        if (m_currentSelectData.attacker.camp == CombatUnit.Camp.Boss)
                        {
                            for (int i = 0; i < _allKeys.Count; i++)
                            {
                                if (_allKeys[i] > 3)
                                {
                                    m_currentSelectedTargets.Add(m_indexToUnit[_allKeys[i]]);
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < _allKeys.Count; i++)
                            {
                                if (_allKeys[i] <= 3)
                                {
                                    m_currentSelectedTargets.Add(m_indexToUnit[_allKeys[i]]);
                                }
                            }
                        }
                        break;
                    }
            }

            m_currentSelectData.onSelected?.Invoke(m_currentSelectedTargets);
            m_currentSelectData = null;
        }

        private void RandomSelect()
        {
            GetIndexRange(out int _min, out int _max);

            List<int> _randomPool = new List<int>();
            for(int i = 0; i < (_max - _min); i++)
            {
                _randomPool.Add(_min + i);
            }

            while(_randomPool.Count > 0)
            {
                int _roll = UnityEngine.Random.Range(0, _randomPool.Count);
                if(m_indexToUnit.ContainsKey(_randomPool[_roll])
                    && m_indexToUnit[_randomPool[_roll]] != m_currentSelectData.attacker)
                {
                    m_currentSelectedTargets.Add(m_indexToUnit[_randomPool[_roll]]);

                    if(m_currentSelectedTargets.Count == m_currentSelectData.needCount)
                    {
                        m_currentSelectData.onSelected?.Invoke(m_currentSelectedTargets);
                        return;
                    }
                    else
                    {
                        _randomPool.RemoveAt(_roll);
                    }
                }
                else
                {
                    _randomPool.RemoveAt(_roll);
                }
            }

            m_currentSelectData.onSelected?.Invoke(m_currentSelectedTargets);
            m_currentSelectData = null;
        }

        private void WaitPlayerSelect()
        {
            switch (m_currentSelectData.selectRange)
            {
                case SelectRange.All:
                    {
                        EnableSelectBossButton(true);
                        EnableSelectPlayerButton(true);
                        break;
                    }
                case SelectRange.Opponent:
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
                case SelectRange.SameSide:
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

        private void GetIndexRange(out int min, out int max)
        {
            switch (m_currentSelectData.selectRange)
            {
                case SelectRange.All:
                    {
                        min = 0;
                        max = 9;
                        break;
                    }
                case SelectRange.Opponent:
                    {
                        if (m_currentSelectData.attacker.camp == CombatUnit.Camp.Boss)
                        {
                            min = 0;
                            max = 4;
                        }
                        else
                        {
                            min = 4;
                            max = 9;
                        }
                        break;
                    }
                case SelectRange.SameSide:
                    {
                        if (m_currentSelectData.attacker.camp == CombatUnit.Camp.Boss)
                        {
                            min = 4;
                            max = 9;
                        }
                        else
                        {
                            min = 0;
                            max = 4;
                        }
                        break;
                    }
                default:
                    {
                        min = 0;
                        max = 9;
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
                for(int i = 0; i < units.Count; i++)
                {
                    for(int j = 0; j < units[i].buffs.Count; j++)
                    {
                        Debug.Log(units[i].buffs[j].effectID);
                    }
                }
            }
        }
    }
}
