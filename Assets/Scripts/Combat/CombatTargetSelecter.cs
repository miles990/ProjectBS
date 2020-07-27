using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KahaGameCore.Interface;
using System;

namespace ProjectBS.Combat
{
    public class CombatTargetSelecter : Manager
    {
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
            public CombatUnit attacker = null;
            public string commandString = "";
            public Action<List<CombatUnit>> onSelected = null;
        }

        public void StartSelect(SelectTargetData data)
        {
            string[] _targetSelection = data.commandString.Replace(")", "").Split('(');
            UI.CombatUIView.SelectTargetData _uiSelectData = null;

            switch (_targetSelection[0])
            {
                case "Self":
                    {
                        data.onSelected?.Invoke(new List<CombatUnit> { data.attacker });
                        break;
                    }
                case "SelectSameSide":
                    {
                        _uiSelectData = new UI.CombatUIView.SelectTargetData
                        {
                            attacker = data.attacker,
                            needCount = int.Parse(_targetSelection[1]),
                            random = false,
                            selectType = UI.CombatUIView.SelectType.SameSide,
                            onSelected = data.onSelected
                        };
                        break;
                    }
                case "SelectOpponent":
                    {
                        _uiSelectData = new UI.CombatUIView.SelectTargetData
                        {
                            attacker = data.attacker,
                            needCount = int.Parse(_targetSelection[1]),
                            random = false,
                            selectType = UI.CombatUIView.SelectType.Opponent,
                            onSelected = data.onSelected
                        };
                        break;
                    }
                case "SelectAllSide":
                    {
                        _uiSelectData = new UI.CombatUIView.SelectTargetData
                        {
                            attacker = data.attacker,
                            needCount = int.Parse(_targetSelection[1]),
                            random = false,
                            selectType = UI.CombatUIView.SelectType.All,
                            onSelected = data.onSelected
                        };
                        break;
                    }
                case "AllSameSide":
                    {
                        _uiSelectData = new UI.CombatUIView.SelectTargetData
                        {
                            attacker = data.attacker,
                            needCount = -1,
                            random = false,
                            selectType = UI.CombatUIView.SelectType.SameSide,
                            onSelected = data.onSelected
                        };
                        break;
                    }
                case "AllOpponent":
                    {
                        _uiSelectData = new UI.CombatUIView.SelectTargetData
                        {
                            attacker = data.attacker,
                            needCount = -1,
                            random = false,
                            selectType = UI.CombatUIView.SelectType.Opponent,
                            onSelected = data.onSelected
                        };
                        break;
                    }
                case "AllBattleField":
                    {
                        _uiSelectData = new UI.CombatUIView.SelectTargetData
                        {
                            attacker = data.attacker,
                            needCount = -1,
                            random = false,
                            selectType = UI.CombatUIView.SelectType.All,
                            onSelected = data.onSelected
                        };
                        break;
                    }
                case "RandomAll":
                    {
                        _uiSelectData = new UI.CombatUIView.SelectTargetData
                        {
                            attacker = data.attacker,
                            needCount = int.Parse(_targetSelection[1]),
                            random = true,
                            selectType = UI.CombatUIView.SelectType.All,
                            onSelected = data.onSelected
                        };
                        break;
                    }
                case "RandomSameSide":
                    {
                        _uiSelectData = new UI.CombatUIView.SelectTargetData
                        {
                            attacker = data.attacker,
                            needCount = int.Parse(_targetSelection[1]),
                            random = true,
                            selectType = UI.CombatUIView.SelectType.SameSide,
                            onSelected = data.onSelected
                        };
                        break;
                    }
                case "RandomOpponent":
                    {
                        _uiSelectData = new UI.CombatUIView.SelectTargetData
                        {
                            attacker = data.attacker,
                            needCount = int.Parse(_targetSelection[1]),
                            random = true,
                            selectType = UI.CombatUIView.SelectType.Opponent,
                            onSelected = data.onSelected
                        };
                        break;
                    }
                default:
                    {
                        throw new Exception("Invaild Command:" + _targetSelection[0]);
                    }
            }

            GetPage<UI.CombatUIView>().StartSelectTarget(_uiSelectData);
        }
    }
}

