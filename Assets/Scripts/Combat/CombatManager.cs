using KahaGameCore.Interface;
using ProjectBS.Data;
using System.Collections.Generic;
using System;

namespace ProjectBS.Combat
{
    public class CombatManager : Manager
    {
        public static CombatManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new CombatManager();
                }
                return m_instance;
            }
        }
        private static CombatManager m_instance = null;

        private CombatManager() { } 

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
            public int rawSP = 100;
            public int rawAttack = 10;
            public int rawDefence = 10;
            public int rawSpeed = 1;
            public OwningEquipmentData head = null;
            public OwningEquipmentData body = null;
            public OwningEquipmentData hand = null;
            public OwningEquipmentData foot = null;
            public string skills = "";
            public string ai = "";
            public List<Buff> buffs = new List<Buff>();
            public List<StatusAdder> statusAdders = new List<StatusAdder>();

            public int GetHP()
            {
                return -1;
            }

            public int GetMaxHP()
            {
                return -1;
            }

            public int GetSP()
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

        private List<CombatUnit> m_units = new List<CombatUnit>();

        public void StartCombat(PartyData player, BossData boss)
        {
            m_units.Clear();
            AddUnit(PlayerManager.Instance.Player.Characters.Find(x => x.UDID == player.MemberUDID_0));
            AddUnit(PlayerManager.Instance.Player.Characters.Find(x => x.UDID == player.MemberUDID_1));
            AddUnit(PlayerManager.Instance.Player.Characters.Find(x => x.UDID == player.MemberUDID_2));
            AddUnit(PlayerManager.Instance.Player.Characters.Find(x => x.UDID == player.MemberUDID_3));
            m_units.Add(new CombatUnit
            {
                ai = boss.AI,
                rawAttack = boss.Attack,
                camp = CombatUnit.Camp.Boss,
                rawDefence = boss.Defence,
                rawMaxHP = boss.HP,
                HP = boss.HP,
                name = boss.NameContextID.ToString(),
                skills = "",
                rawSP = boss.SP,
                rawSpeed = boss.Speed,
                sprite = GameDataLoader.Instance.GetSprite(boss.CharacterSpriteID),
                body = null,
                foot = null,
                hand = null,
                head = null,
                buffs = new List<CombatUnit.Buff>(),
                statusAdders = new List<CombatUnit.StatusAdder>()
            });
        }

        private void AddUnit(OwningCharacterData character)
        {
            m_units.Add(new CombatUnit
            {
                ai = "",
                rawAttack = character.Attack,
                camp = CombatUnit.Camp.Player,
                rawDefence = character.Defence,
                rawMaxHP = character.HP,
                HP = character.HP,
                name = GameDataLoader.Instance.GetCharacterName(character.CharacterNameID),
                skills = string.Format("{0},{1},{2},{3}", character.SkillSlot_0, character.SkillSlot_1, character.SKillSlot_2, character.SKillSlot_3),
                rawSP = character.SP,
                rawSpeed = character.Speed,
                sprite = GameDataLoader.Instance.GetSprite(character.CharacterSpriteID),
                body = PlayerManager.Instance.GetEquipmentByUDID(character.Equipment_UDID_Body),
                foot = PlayerManager.Instance.GetEquipmentByUDID(character.Equipment_UDID_Foot),
                hand = PlayerManager.Instance.GetEquipmentByUDID(character.Equipment_UDID_Hand),
                head = PlayerManager.Instance.GetEquipmentByUDID(character.Equipment_UDID_Head),
                buffs = new List<CombatUnit.Buff>(),
                statusAdders = new List<CombatUnit.StatusAdder>()
            });
        }
    }
}
