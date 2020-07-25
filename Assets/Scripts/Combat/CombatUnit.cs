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
        public int HP = 100;
        public int rawMaxHP = 100;
        public int SP = 100;
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

        public int GetMaxHP()
        {
            return -1;
        }

        public int GetAttack()
        {
            return -1;
        }

        public int GetDefence()
        {
            return -1;
        }

        public int GetSpeed()
        {
            return -1;
        }
    }
}
