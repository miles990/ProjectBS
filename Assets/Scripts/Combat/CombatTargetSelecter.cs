using System.Collections.Generic;
using KahaGameCore.Interface;
using System;

namespace ProjectBS.Combat
{
    public class CombatTargetSelecter : Manager
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
            HighestDefense,
            HighestSpeed,
            HighestHatred,
            LowestMaxHP,
            LowestHP,
            LowestSP,
            LowestAttack,
            LowestDefense,
            LowestSpeed,
            LowestHatred,
            RandomByHatred
        }

        public static CombatTargetSelecter Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new CombatTargetSelecter();
                }
                return m_instance;
            }
        }
        private static CombatTargetSelecter m_instance = null;

        private CombatTargetSelecter() { }

        public class SelectTargetData
        {
            public string id = "";
            public CombatUnit attacker = null;
            public string commandString = "";
            public Action<List<CombatUnit>> onSelected = null;
        }

        private CombatUnit m_attacker = null;
        private bool m_inculdeAttacker = false;
        private int m_needCount = 0;
        private List<CombatUnit> m_allUnit = new List<CombatUnit>();

        private SelectRange m_currentSelectRange = SelectRange.All;
        private SelectType m_currentSelectType = SelectType.Random;
        private Action<List<CombatUnit>> m_onSelected = null;

        private Dictionary<string, List<CombatUnit>> m_idToSelected = new Dictionary<string, List<CombatUnit>>();
        private string m_currnetManualSelectingID = "";

        public void StartSelect(SelectTargetData data)
        {
            string[] _commandParts = data.commandString.Split('(');
            string _command = _commandParts[0];
            string _var = "";
            string[] _vars = null;
            if (_commandParts.Length > 1)
            {
                _var = _commandParts[1].Replace(")", "");
                _vars = _var.Split(',');
            }

            switch (_command)
            {
                case "Self":
                    {
                        if(m_idToSelected.ContainsKey(data.id))
                        {
                            m_idToSelected[data.id] = new List<CombatUnit> { data.attacker };
                        }
                        else
                        {
                            m_idToSelected.Add(data.id, new List<CombatUnit> { data.attacker });
                        }
                        data.onSelected?.Invoke(m_idToSelected[data.id]);
                        return;
                    }
                case "CurrentActor":
                    {
                        if (m_idToSelected.ContainsKey(data.id))
                        {
                            m_idToSelected[data.id] = new List<CombatUnit> { CombatUtility.ComabtManager.CurrentActionInfo.actor };
                        }
                        else
                        {
                            m_idToSelected.Add(data.id, new List<CombatUnit> { CombatUtility.ComabtManager.CurrentActionInfo.actor });
                        }

                        data.onSelected?.Invoke(m_idToSelected[data.id]);
                        return;
                    }
                case "Select":
                case "SelectOther":
                    {
                        m_currnetManualSelectingID = data.id;

                        if (m_idToSelected.ContainsKey(data.id))
                        {
                            m_idToSelected[data.id].Clear();
                        }
                        else
                        {
                            m_idToSelected.Add(data.id, new List<CombatUnit>());
                        }

                        m_currentSelectRange = (SelectRange)Enum.Parse(typeof(SelectRange), _vars[0]);
                        m_currentSelectType = (SelectType)Enum.Parse(typeof(SelectType), _vars[1]);
                        m_allUnit = CombatUtility.ComabtManager.AllUnit;
                        m_needCount = int.Parse(_vars[2]);
                        m_attacker = data.attacker;
                        m_onSelected = OnManualSelected;
                        m_onSelected += data.onSelected;
                        m_inculdeAttacker = _command == "Select";

                        if (m_currentSelectType == SelectType.Manual)
                        {
                            UI.CombatUIView.SelectTargetData _selectData = new UI.CombatUIView.SelectTargetData
                            {
                                attacker = m_attacker,
                                inculdeAttacker = m_inculdeAttacker,
                                selectRange = m_currentSelectRange,
                                needCount = m_needCount,
                                onSelected = m_onSelected
                            };
                            GetPage<UI.CombatUIView>().StartSelectTarget(_selectData);
                        }
                        else
                        {
                            
                            DoSelect();
                        }
                        return;
                    }
                case "LastSelected":
                    {
                        if (m_idToSelected.ContainsKey(data.id))
                        {
                            data.onSelected?.Invoke(m_idToSelected[data.id]);
                        }
                        else
                        {
                            data.onSelected?.Invoke(new List<CombatUnit>());
                        }

                        return;
                    }
                default:
                    {
                        throw new Exception("[CombatTargetSelecter][StartSelect] Invaild select command:" + _command);
                    }
            }
        }

        private void OnManualSelected(List<CombatUnit> targets)
        {
            m_idToSelected[m_currnetManualSelectingID] = targets;
        }

        private void DoSelect()
        {
            if (m_needCount == -1)
            {
                SelectAll();
            }
            else
            {
                switch (m_currentSelectType)
                {
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
                    case SelectType.HighestDefense:
                        {
                            SelectByStatus(Keyword.Defense, true);
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
                    case SelectType.LowestDefense:
                        {
                            SelectByStatus(Keyword.Defense, false);
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
            List<CombatUnit> _rollPool = new List<CombatUnit>();
            int _totalHatred = 0;

            for (int i = 0; i < m_allUnit.Count; i++)
            {
                if (m_inculdeAttacker && m_allUnit[i] == m_attacker)
                {
                    continue;
                }

                switch (m_currentSelectRange)
                {
                    case SelectRange.All:
                        {
                            _rollPool.Add(m_allUnit[i]);
                            _totalHatred += m_allUnit[i].Hatred;
                            break;
                        }
                    case SelectRange.Opponent:
                        {
                            if (m_allUnit[i].camp != m_attacker.camp)
                            {
                                _rollPool.Add(m_allUnit[i]);
                                _totalHatred += m_allUnit[i].Hatred;
                            }
                            break;
                        }
                    case SelectRange.SameSide:
                        {
                            if (m_allUnit[i].camp == m_attacker.camp)
                            {
                                _rollPool.Add(m_allUnit[i]);
                                _totalHatred += m_allUnit[i].Hatred;
                            }
                            break;
                        }
                }
            }

            int _roll = UnityEngine.Random.Range(1, _totalHatred + 1);
            for (int i = 0; i < _rollPool.Count; i++)
            {
                _roll -= _rollPool[i].Hatred;
                if (_roll <= 0)
                {
                    m_idToSelected[m_currnetManualSelectingID].Add(_rollPool[i]);
                    break;
                }
            }

            if (m_idToSelected[m_currnetManualSelectingID].Count == m_needCount)
            {
                CompleteSelect();
            }
            else
            {
                SelectByRandomByHatred();
            }
        }

        private void SelectByStatus(string statusType, bool getHighest)
        {
            CombatUnit _currentSelect = null;
            for (int i = 0; i < m_allUnit.Count; i++)
            {
                if (m_currentSelectRange == SelectRange.Opponent
                    && m_allUnit[i].camp == m_attacker.camp)
                {
                    continue;
                }
                if (m_currentSelectRange == SelectRange.SameSide
                    && m_allUnit[i].camp != m_attacker.camp)
                {
                    continue;
                }

                if (_currentSelect == null)
                {
                    _currentSelect = m_allUnit[i];
                    continue;
                }

                if ((getHighest && CombatUtility.GetStatusValue(m_allUnit[i], statusType, false) > CombatUtility.GetStatusValue(_currentSelect, statusType, false))
                    || (!getHighest && CombatUtility.GetStatusValue(m_allUnit[i], statusType, false) < CombatUtility.GetStatusValue(_currentSelect, statusType, false)))
                {
                    _currentSelect = m_allUnit[i];
                }
            }

            m_idToSelected[m_currnetManualSelectingID].Add(_currentSelect);
            if (m_idToSelected[m_currnetManualSelectingID].Count == m_needCount)
            {
                CompleteSelect();
            }
            else
            {
                SelectByStatus(statusType, getHighest);
            }
        }

        private void SelectAll()
        {
            switch (m_currentSelectRange)
            {
                case SelectRange.All:
                    {
                        for (int i = 0; i < m_allUnit.Count; i++)
                        {
                            if (!m_inculdeAttacker && m_allUnit[i] == m_attacker)
                            {
                                continue;
                            }

                            m_idToSelected[m_currnetManualSelectingID].Add(m_allUnit[i]);
                        }
                        break;
                    }
                case SelectRange.Opponent:
                    {
                        for (int i = 0; i < m_allUnit.Count; i++)
                        {
                            if (!m_inculdeAttacker && m_allUnit[i] == m_attacker)
                            {
                                continue;
                            }

                            if (m_attacker.camp != m_allUnit[i].camp)
                                m_idToSelected[m_currnetManualSelectingID].Add(m_allUnit[i]);
                        }
                        break;
                    }
                case SelectRange.SameSide:
                    {
                        for (int i = 0; i < m_allUnit.Count; i++)
                        {
                            if (!m_inculdeAttacker && m_allUnit[i] == m_attacker)
                            {
                                continue;
                            }

                            if (m_attacker.camp == m_allUnit[i].camp)
                                m_idToSelected[m_currnetManualSelectingID].Add(m_allUnit[i]);
                        }
                        break;
                    }
            }

            CompleteSelect();
        }

        private void RandomSelect()
        {
            List<CombatUnit> _randomPool = new List<CombatUnit>();
            switch (m_currentSelectRange)
            {
                case SelectRange.All:
                    {
                        for (int i = 0; i < m_allUnit.Count; i++)
                        {
                            if (!m_inculdeAttacker && m_allUnit[i] == m_attacker)
                            {
                                continue;
                            }

                            _randomPool.Add(m_allUnit[i]);
                        }
                        break;
                    }
                case SelectRange.Opponent:
                    {
                        for (int i = 0; i < m_allUnit.Count; i++)
                        {
                            if (!m_inculdeAttacker && m_allUnit[i] == m_attacker)
                            {
                                continue;
                            }

                            if (m_attacker.camp != m_allUnit[i].camp)
                                _randomPool.Add(m_allUnit[i]);
                        }
                        break;
                    }
                case SelectRange.SameSide:
                    {
                        for (int i = 0; i < m_allUnit.Count; i++)
                        {
                            if (!m_inculdeAttacker && m_allUnit[i] == m_attacker)
                            {
                                continue;
                            }

                            if (m_attacker.camp == m_allUnit[i].camp)
                                _randomPool.Add(m_allUnit[i]);
                        }
                        break;
                    }
            }

            while (_randomPool.Count > 0)
            {
                int _roll = UnityEngine.Random.Range(0, _randomPool.Count);
                m_idToSelected[m_currnetManualSelectingID].Add(_randomPool[_roll]);
                _randomPool.RemoveAt(_roll);
                if(m_idToSelected[m_currnetManualSelectingID].Count == m_needCount)
                {
                    CompleteSelect();
                    return;
                }
            }

            CompleteSelect();
        }

        private void CompleteSelect()
        {
            m_onSelected?.Invoke(m_idToSelected[m_currnetManualSelectingID]);
        }
    }
}

