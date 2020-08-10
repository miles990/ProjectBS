using ProjectBS.Data;
using System.Collections.Generic;

namespace ProjectBS.Combat
{
    public class CombatUnit
    {
        public enum Camp
        {
            Player,
            Boss
        }

        public class Buff
        {
            public int effectID = 0;
            public int remainingTime = 1;
            public int stackCount = 1;
            public CombatUnit from = null;
        }

        public class StatusAdder
        {
            public string statusType = "HP";
            public string valueString = "0";
            public Buff parentBuff = null;
        }

        public string name = "";
        public UnityEngine.Sprite sprite = null;
        public Camp camp = Camp.Player;
        public int HP 
        {
            get { return m_hp; }
            set 
            {
                m_hp = value;
                if (m_hp > GetMaxHP())
                    m_hp = GetMaxHP();
            }
        }
        private int m_hp = 100;
        public int rawMaxHP = 100;
        public int SP 
        {
            get { return m_sp; }
            set 
            {
                m_sp = value;
                if (m_sp > 100)
                    m_sp = 100;
            }
        }
        private int m_sp = 100;
        public int rawAttack = 10;
        public int rawDefence = 10;
        public int rawSpeed = 1;
        public int hatred = 0;
        public OwningEquipmentData head = null;
        public OwningEquipmentData body = null;
        public OwningEquipmentData hand = null;
        public OwningEquipmentData foot = null;
        public string skills = "";
        public string ai = "";
        public List<Buff> buffs = new List<Buff>();
        public List<StatusAdder> statusAdders = new List<StatusAdder>();

        public Dictionary<CombatUnit, int> targetToDmg = new Dictionary<CombatUnit, int>();

        public int GetMaxHP()
        {
            return GetResult(rawMaxHP, Keyword.MaxHP);
        }

        public int GetAttack()
        {
            return GetResult(rawAttack, Keyword.Attack);
        }

        public int GetDefence()
        {
            return GetResult(rawDefence, Keyword.Defence);
        }

        public int GetSpeed()
        {
            return GetResult(rawSpeed, Keyword.Speed);
        }

        private int GetResult(int rawValue, string statusType)
        {
            int _temp = rawValue;

            AddValueByEquipment(head, statusType, ref _temp);
            AddValueByEquipment(body, statusType, ref _temp);
            AddValueByEquipment(hand, statusType, ref _temp);
            AddValueByEquipment(foot, statusType, ref _temp);

            List<StatusAdder> _adderList = new List<StatusAdder>(statusAdders);

            for (int i = 0; i < _adderList.Count; i++)
            {
                if (_adderList[i].statusType == statusType)
                {
                    if (int.TryParse(_adderList[i].valueString, out int _adderValue))
                    {
                        _temp += _adderValue;
                        continue;
                    }
                    else
                    {
                        float _result = CombatUtility.Calculate(new CombatUtility.CalculateData
                        {
                            caster = _adderList[i].parentBuff.from,
                            target = this,
                            formula = _adderList[i].valueString,
                            useRawValue = true
                        });

                        _temp += System.Convert.ToInt32(_result);
                    }
                }
                else
                {
                    _adderList.RemoveAt(i);
                    i--;
                }
            }

            return _temp;
        }

        private void AddValueByEquipment(OwningEquipmentData equipmentData, string statusType, ref int value)
        {
            if (equipmentData == null)
                return;

            switch (statusType)
            {
                case Keyword.HP:
                case Keyword.MaxHP:
                    {
                        value += equipmentData.HP;
                        break;
                    }
                case Keyword.Attack:
                    {
                        value += equipmentData.Attack;
                        break;
                    }
                case Keyword.Defence:
                    {
                        value += equipmentData.Defence;
                        break;
                    }
                case Keyword.Speed:
                    {
                        value += equipmentData.Speed;
                        break;
                    }
                case Keyword.SP:
                    {
                        value += equipmentData.SP;
                        break;
                    }
            }
        }
    }
}
