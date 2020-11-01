using ProjectBS.Data;
using System.Collections.Generic;

namespace ProjectBS.Combat
{
    public class CombatUnit
    {
        public enum Camp
        {
            Player,
            Enemy
        }

        public class Buff
        {
            public int effectID = 0;
            public int remainingTime = 1;
            public int stackCount = 1;
            public CombatUnit owner = null;
            public CombatUnit from = null;

            public SkillEffectData GetSkillEffectData()
            {
                return GameDataManager.GetGameData<SkillEffectData>(effectID);
            }
        }

        public class StatusAdder
        {
            public string statusType = "HP";
            public string valueString = "0";
            public Buff parentBuff = null;
        }

        public class StatusAddLocker
        {
            public string statusType = "HP";
            public Buff parentBuff = null;
        }

        public class Shield
        {
            public int value = 0;
            public int triggerSKillID = 0;
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
                if(value > m_hp && statusAddLockers.Find(x => x.statusType == "HP") != null)
                {
                    return;
                }

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
                if (value > m_sp && statusAddLockers.Find(x => x.statusType == "SP") != null)
                {
                    return;
                }

                m_sp = value;
                if (m_sp > 100)
                    m_sp = 100;
            }
        }
        private int m_sp = 100;
        public int rawAttack = 10;
        public int rawDefense = 10;
        public int rawSpeed = 1;
        public int Hatred 
        {
            get { return m_hatred; }
            set
            {
                if (value > m_hatred && statusAddLockers.Find(x => x.statusType == "Hatred") != null)
                {
                    return;
                }

                m_hatred = value;
                if (m_hatred < 1)
                    m_hatred = 1;
            }
        }
        private int m_hatred = 0;
        public OwningEquipmentData head = null;
        public OwningEquipmentData body = null;
        public OwningEquipmentData hand = null;
        public OwningEquipmentData foot = null;
        public int[] skills = new int[4];
        public string ai = "";
        public List<Buff> buffs = new List<Buff>();
        public List<StatusAdder> statusAdders = new List<StatusAdder>();
        public List<StatusAddLocker> statusAddLockers = new List<StatusAddLocker>();
        public List<Shield> shields = new List<Shield>();

        public bool skipAction = false;
        public bool skipCheckSP = false;

        public int lastSkillID = 0;
        public Dictionary<CombatUnit, int> targetToDmg = new Dictionary<CombatUnit, int>();
        public int lastTakenDamage = 0;

        public int actionIndex = 0;

        public int GetMaxHP()
        {
            return GetResult(rawMaxHP, Keyword.MaxHP);
        }

        public int GetAttack()
        {
            return GetResult(rawAttack, Keyword.Attack);
        }

        public int GetDefense()
        {
            return GetResult(rawDefense, Keyword.Defense);
        }

        public int GetSpeed()
        {
            return GetResult(rawSpeed, Keyword.Speed);
        }

        public void AddBuffStack(Buff buff, int stackCount, System.Action onRemoved, System.Action onNotRemoved)
        {
            if(buff == null)
            {
                onNotRemoved?.Invoke();
                return;
            }

            buff.stackCount += stackCount;
            if (buff.stackCount <= 0)
            {
                RemoveBuff(buff, onRemoved);
                return;
            }

            onNotRemoved?.Invoke();
        }

        public void AddBuffTime(Buff buff, int addTime, System.Action onRemoved, System.Action onNotRemoved)
        {
            if (buff == null)
            {
                onNotRemoved?.Invoke();
                return;
            }

            buff.remainingTime += addTime;
            if (buff.remainingTime <= 0)
            {
                RemoveBuff(buff, onRemoved);
                return;
            }

            onNotRemoved?.Invoke();
        }

        public void RemoveBuff(Buff buff, System.Action onRemoved)
        {
            SkillEffectData _effect = buff.GetSkillEffectData();

            EffectProcessManager.GetSkillEffectProcesser(_effect.ID).Start(new EffectProcesser.ProcessData
            {
                caster = this,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnDeactived,
                allEffectProcesser = CombatUtility.CurrentComabtManager.AllUnitAllEffectProcesser,
                referenceBuff = buff,
                refenceSkill = null,
                onEnded = delegate { OnBuffRemoved(buff, _effect, onRemoved); }
            });
        }

        private void OnBuffRemoved(Buff buff, SkillEffectData _effect, System.Action onRemoved)
        {
            CombatUtility.RemoveEffect(this, _effect.ID);
            buffs.Remove(buff);
            onRemoved?.Invoke();
        }

        private int GetResult(int rawValue, string statusType)
        {
            int _temp = rawValue;

            AddValueByEquipment(head, statusType, ref _temp);
            AddValueByEquipment(body, statusType, ref _temp);
            AddValueByEquipment(hand, statusType, ref _temp);
            AddValueByEquipment(foot, statusType, ref _temp);

            for (int i = 0; i < statusAdders.Count; i++)
            {
                if (statusAdders[i].statusType == statusType)
                {
                    if (int.TryParse(statusAdders[i].valueString, out int _adderValue))
                    {
                        _temp += _adderValue;
                        continue;
                    }
                    else
                    {
                        float _result = CombatUtility.Calculate(new CombatUtility.CalculateData
                        {
                            caster = this,
                            target = this,
                            referenceBuff = statusAdders[i].parentBuff,
                            formula = statusAdders[i].valueString,
                            useRawValue = true
                        });

                        _temp += System.Convert.ToInt32(_result);
                    }
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
                case Keyword.Defense:
                    {
                        value += equipmentData.Defense;
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
