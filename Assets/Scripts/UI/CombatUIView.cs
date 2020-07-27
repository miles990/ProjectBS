﻿using ProjectBS.Combat;
using KahaGameCore.Interface;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBS.UI
{
    public class CombatUIView : UIView
    {
        public enum SelectType
        {
            SameSide,
            Opponent,
            All
        }

        public override bool IsShowing => throw new NotImplementedException();

        public event Action<int> OnSkillSelected = null;

        public class SelectTargetData
        {
            public SelectType selectType = SelectType.All;
            public CombatUnit attacker = null;
            public int needCount = 0;
            public bool random = false;
            public Action<List<CombatUnit>> onSelected = null;
        }

        // 0~3:Player 4~8:Boss
        private Dictionary<int, CombatUnit> m_indexToCombatUnit = new Dictionary<int, CombatUnit>();
        private Dictionary<int, bool> m_indexToEnableState = new Dictionary<int, bool>();

        private SelectTargetData m_currentSelectData = null;
        private List<CombatUnit> m_currentTargets = new List<CombatUnit>();

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

            for(int i = 0; i < units.Count; i++)
            {
                Debug.Log("Set Unit UI------------------------------");
                Debug.LogFormat("units[{0}].camp={1}", i, units[i].camp.ToString());
                Debug.LogFormat("units[{0}].name={1}", i , units[i].name);
                Debug.LogFormat("units[{0}].hp={1}", i, units[i].HP);
                Debug.LogFormat("units[{0}].sp={1}", i, units[i].SP);
                Debug.Log("------------------------------");

                if (units[i].camp == CombatUnit.Camp.Player)
                {
                    m_indexToCombatUnit.Add(_currentPlayerIndex, units[i]);
                    m_indexToEnableState.Add(_currentPlayerIndex, false);
                    _currentPlayerIndex++;
                }
                else
                {
                    m_indexToCombatUnit.Add(_currentBossIndex, units[i]);
                    m_indexToEnableState.Add(_currentBossIndex, false);
                    _currentBossIndex++;
                }
            }
        }

        public void RefreshCurrentSkillMenu(CombatUnit actor)
        {
            Debug.Log("------------------------------");
            Debug.Log("Current Actor:" + actor.name);

            string[] _skillIDs = actor.skills.Split(',');
            for(int i = 0; i < _skillIDs.Length; i++)
            {
                Debug.LogFormat("Skill {0}: ContextID={1}", i, GameDataLoader.Instance.GetSkill(_skillIDs[i]).NameContextID);
            }
        }

        public void StartSelectTarget(SelectTargetData data)
        {
            if (m_currentSelectData != null)
                return;

            m_currentTargets.Clear();
            m_currentSelectData = data;

            DoSelect();
        }

        public void Button_SelectSkill(int index)
        {
            Debug.Log("Select Skill Index " + index);
            OnSkillSelected?.Invoke(index);
        }

        public void Button_SelectCharacter(int index)
        {
            if(!m_indexToCombatUnit.ContainsKey(index))
            {
                return;
            }

            if(!m_indexToEnableState[index])
            {
                return;
            }

            m_currentTargets.Add(m_indexToCombatUnit[index]);

            if(m_currentTargets.Count < m_currentSelectData.needCount)
            {
                DoSelect();
            }
            else
            {
                EnableSelectBossButton(false);
                EnableSelectPlayerButton(false);
                m_currentSelectData.onSelected?.Invoke(m_currentTargets);
                m_currentSelectData = null;
            }
        }

        private void DoSelect()
        {
            if(m_currentSelectData.needCount == -1)
            {
                SelectAll();
            }
            else if(m_currentSelectData.random)
            {
                RandomSelect();
            }
            else
            {
                WaitPlayerSelect();
            }
        }

        private void SelectAll()
        {
            List<int> _allKeys = new List<int>(m_indexToCombatUnit.Keys);
            switch (m_currentSelectData.selectType)
            {
                case SelectType.All:
                    {
                        for(int i = 0; i < _allKeys.Count; i++)
                        {
                            m_currentTargets.Add(m_indexToCombatUnit[_allKeys[i]]);
                        }
                        break;
                    }
                case SelectType.Opponent:
                    {
                        if(m_currentSelectData.attacker.camp == CombatUnit.Camp.Boss)
                        {
                            for (int i = 0; i < _allKeys.Count; i++)
                            {
                                if(_allKeys[i] <= 3)
                                {
                                    m_currentTargets.Add(m_indexToCombatUnit[_allKeys[i]]);
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < _allKeys.Count; i++)
                            {
                                if (_allKeys[i] > 3)
                                {
                                    m_currentTargets.Add(m_indexToCombatUnit[_allKeys[i]]);
                                }
                            }
                        }
                        break;
                    }
                case SelectType.SameSide:
                    {
                        if (m_currentSelectData.attacker.camp == CombatUnit.Camp.Boss)
                        {
                            for (int i = 0; i < _allKeys.Count; i++)
                            {
                                if (_allKeys[i] > 3)
                                {
                                    m_currentTargets.Add(m_indexToCombatUnit[_allKeys[i]]);
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < _allKeys.Count; i++)
                            {
                                if (_allKeys[i] <= 3)
                                {
                                    m_currentTargets.Add(m_indexToCombatUnit[_allKeys[i]]);
                                }
                            }
                        }
                        break;
                    }
            }

            m_currentSelectData.onSelected?.Invoke(m_currentTargets);
            m_currentSelectData = null;
        }

        private void RandomSelect()
        {
            int _min = 0;
            int _max = 0;
            switch (m_currentSelectData.selectType)
            {
                case SelectType.All:
                    {
                        _min = 0;
                        _max = 9;
                        break;
                    }
                case SelectType.Opponent:
                    {
                        if (m_currentSelectData.attacker.camp == CombatUnit.Camp.Boss)
                        {
                            _min = 0;
                            _max = 4;
                        }
                        else
                        {
                            _min = 4;
                            _max = 9;
                        }
                        break;
                    }
                case SelectType.SameSide:
                    {
                        if (m_currentSelectData.attacker.camp == CombatUnit.Camp.Boss)
                        {
                            _min = 4;
                            _max = 9;
                        }
                        else
                        {
                            _min = 0;
                            _max = 4;
                        }
                        break;
                    }
            }

            List<int> _randomPool = new List<int>();
            for(int i = 0; i < (_max - _min); i++)
            {
                _randomPool.Add(_min + i);
            }

            while(_randomPool.Count > 0)
            {
                int _roll = UnityEngine.Random.Range(0, _randomPool.Count);
                if(m_indexToCombatUnit.ContainsKey(_randomPool[_roll])
                    && m_indexToCombatUnit[_randomPool[_roll]] != m_currentSelectData.attacker)
                {
                    m_currentTargets.Add(m_indexToCombatUnit[_randomPool[_roll]]);

                    if(m_currentTargets.Count == m_currentSelectData.needCount)
                    {
                        m_currentSelectData.onSelected?.Invoke(m_currentTargets);
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

            m_currentSelectData.onSelected?.Invoke(m_currentTargets);
            m_currentSelectData = null;
        }

        private void WaitPlayerSelect()
        {
            switch (m_currentSelectData.selectType)
            {
                case SelectType.All:
                    {
                        EnableSelectBossButton(true);
                        EnableSelectPlayerButton(true);
                        break;
                    }
                case SelectType.Opponent:
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
                case SelectType.SameSide:
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
                if(m_indexToCombatUnit.ContainsKey(i))
                {
                    if(enable)
                        Debug.Log("Enable Key Index:" + i);
                    m_indexToEnableState[i] = enable;
                }
            }
        }

        private void EnableSelectPlayerButton(bool enable)
        {
            for (int i = 0; i < 4; i++)
            {
                if (m_indexToCombatUnit.ContainsKey(i))
                {
                    if (enable)
                        Debug.Log("Enable Key Index:" + i);
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
        }
    }
}
