using System.Collections;
using System.Collections.Generic;
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
            string[] _commandParts = data.commandString.Split('(');
            string _command = _commandParts[0];
            string _var = "";
            string[] _vars = null;
            if (_commandParts.Length > 1)
            {
                _var = _commandParts[1].Replace(")", "");
                _vars = _var.Split(',');
            }

            UI.CombatUIView.SelectTargetData _selectData = null;
            switch (_command)
            {
                case "Self":
                    {
                        data.onSelected?.Invoke(new List<CombatUnit> { data.attacker });
                        return;
                    }
                case "Select":
                case "SelectOther":
                    {
                        _selectData = new UI.CombatUIView.SelectTargetData
                        {
                            attacker = data.attacker,
                            inculdeAttacker = true,
                            needCount = int.Parse(_vars[2]),
                            onSelected = data.onSelected
                        };
                        break;
                    }
            }

            _selectData.selectRange = (UI.CombatUIView.SelectRange)Enum.Parse(typeof(UI.CombatUIView.SelectRange), _vars[0]);
            _selectData.selectType = (UI.CombatUIView.SelectType)Enum.Parse(typeof(UI.CombatUIView.SelectType), _vars[1]);

            GetPage<UI.CombatUIView>().StartSelectTarget(_selectData);
        }
    }
}

