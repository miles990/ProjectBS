using ProjectBS.Data;
using System.Collections.Generic;

namespace ProjectBS.Combat
{
    [System.Serializable]
    public class CombatUnit
    {
        public class Buff
        {
            public int soruceID = 0;
            public int remainingTime = 1;
            public int stackCount = 1;
            public string ownerUnitUDID = "";
            public string fromUnitUDID = "";

            public BuffData GetBuffSourceData()
            {
                return GameDataManager.GetGameData<BuffData>(soruceID);
            }
        }

        public class StatusAdder
        {
            public string statusType = "HP";
            public string valueString = "0";
            public int parentBuffID = 0;
        }

        public class StatusAddLocker
        {
            public string statusType = "HP";
            public int parentBuffID = 0;
        }

        public class Skiper
        {
            public int parentBuffID = 0;
        }

        public class Shield
        {
            public int value = 0;
            public int triggerSKillID = 0;
            public int parentBuffID = 0;
        }

        public string UDID = "";
        public string name = "";
        public UnityEngine.Sprite sprite = null;
        public int camp = 0;
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
        public string head = null;
        public string body = null;
        public string hand = null;
        public string foot = null;
        public int[] skills = new int[4];
        public string ai = "";
        public List<StatusAdder> statusAdders = new List<StatusAdder>();
        public List<StatusAddLocker> statusAddLockers = new List<StatusAddLocker>();
        public List<Skiper> actionSkipers = new List<Skiper>();
        public List<Skiper> checkSPSkipers = new List<Skiper>();
        public List<Shield> shields = new List<Shield>();

        public int lastSkillID = 0;
        public Dictionary<string, int> targetToDmg = new Dictionary<string, int>();
        public int lastTakenDamage = 0;

        public int actionIndex = 0;

        public bool IsSkipAtion { get => actionSkipers.Count > 0; }
        public bool IsSkipCheckSP { get => checkSPSkipers.Count > 0; }

        public int OwnBuffCount { get { return m_buffs.Count; } }
        private List<Buff> m_buffs = new List<Buff>();
        public List<Buff> BuffDataPassBuffer { get; private set; }
        public class StatusBuffer
        {
            public int hp;
            public int sp;
            public int hatred;
        }
        public StatusBuffer CurrentStatusBuffer { get; private set; } = new StatusBuffer();

        public CombatUnit GetJsonableData()
        {
            // private member can't be read, so use this to set BuffDataPassBuffer for passing buff data
            BuffDataPassBuffer = new List<Buff>(m_buffs);
            CurrentStatusBuffer.hp = m_hp;
            CurrentStatusBuffer.sp = m_sp;
            CurrentStatusBuffer.hatred = m_hatred;
            UnityEngine.Debug.Log(UDID + " send hatred=" + CurrentStatusBuffer.hatred);
            return this;
        }

        public void UpdateData(CombatUnit unit)
        {
            if(UDID != unit.UDID)
            {
                throw new System.Exception("[CombatUnit][UpdateData] is trying to override data with different udid");
            }

            rawMaxHP = unit.rawMaxHP;
            rawAttack = unit.rawAttack;
            rawDefense = unit.rawDefense;
            rawSpeed = unit.rawSpeed;
            m_hp = unit.CurrentStatusBuffer.hp;
            m_sp = unit.CurrentStatusBuffer.sp;
            m_hatred = unit.CurrentStatusBuffer.hatred;
            UnityEngine.Debug.Log(UDID + " receive hatred=" + unit.CurrentStatusBuffer.hatred);
            head = unit.head;
            body = unit.body;
            hand = unit.hand;
            foot = unit.foot;
            skills = unit.skills;
            ai = unit.ai;
            statusAdders = unit.statusAdders;
            statusAddLockers = unit.statusAddLockers;
            actionSkipers = unit.actionSkipers;
            checkSPSkipers = unit.checkSPSkipers;
            shields = unit.shields;
            lastSkillID = unit.lastSkillID;
            targetToDmg = unit.targetToDmg;
            lastTakenDamage = unit.lastTakenDamage;
            actionIndex = unit.actionIndex;
            m_buffs = unit.BuffDataPassBuffer;
        }

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

        public void AddBuff(Buff buff)
        {
            Buff _oldBuff = m_buffs.Find(x => x.soruceID == buff.soruceID);
            if(_oldBuff == null)
            {
                m_buffs.Add(buff);
            }
            else
            {
                if(_oldBuff.stackCount < buff.stackCount
                    || _oldBuff.remainingTime < buff.remainingTime)
                {
                    m_buffs.Remove(_oldBuff);
                    m_buffs.Add(buff);
                }
            }
        }

        public Buff GetBuffByBuffEffectID(int id)
        {
            return m_buffs.Find(x => x.soruceID == id);
        }

        public Buff GetBuffByIndex(int index)
        {
            if(index >= m_buffs.Count)
            {
                return null;
            }

            if(index < 0 || m_buffs.Count == 0)
            {
                return null;
            }

            return m_buffs[index];
        }

        public bool HasBuffWithTag(int tag)
        {
            return m_buffs.Find(x => x.GetBuffSourceData().Tag == tag) != null;
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
            BuffData _effect = buff.GetBuffSourceData();

            EffectProcessManager.GetBuffProcesser(_effect.ID).Start(new EffectProcesser.ProcessData
            {
                caster = this,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnDeactived,
                allEffectProcesser = CombatUtility.ComabtManager.AllUnitAllEffectProcesser,
                referenceBuff = buff,
                refenceSkill = null,
                onEnded = delegate { OnBuffRemoved(buff, _effect, onRemoved); }
            });
        }

        private void OnBuffRemoved(Buff buff, BuffData _effect, System.Action onRemoved)
        {
            CombatUtility.RemoveEffect(this, _effect.ID);
            m_buffs.Remove(buff);
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
                            referenceBuff = GetBuffByBuffEffectID(statusAdders[i].parentBuffID),
                            formula = statusAdders[i].valueString,
                            useRawValue = true
                        });

                        _temp += System.Convert.ToInt32(_result);
                    }
                }
            }

            return _temp;
        }

        private void AddValueByEquipment(string equipmentData, string statusType, ref int value)
        {
            if (string.IsNullOrEmpty(equipmentData))
                return;

            OwningEquipmentData _equipment = PlayerManager.Instance.GetEquipmentByUDID(equipmentData);

            switch (statusType)
            {
                case Keyword.HP:
                case Keyword.MaxHP:
                    {
                        value += _equipment.HP;
                        break;
                    }
                case Keyword.Attack:
                    {
                        value += _equipment.Attack;
                        break;
                    }
                case Keyword.Defense:
                    {
                        value += _equipment.Defense;
                        break;
                    }
                case Keyword.Speed:
                    {
                        value += _equipment.Speed;
                        break;
                    }
                case Keyword.SP:
                    {
                        value += _equipment.SP;
                        break;
                    }
            }
        }
    }
}
