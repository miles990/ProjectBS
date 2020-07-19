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

        private List<CombatUnit> m_allUnits = new List<CombatUnit>();
        private List<CombatUnit> m_battlingUnits = null;

        public void StartCombat(PartyData player, BossData boss)
        {
            m_allUnits.Clear();
            InitBattleUnits(player, boss);
            SortUnitsBySpeed();
            StartProcessEquipmentEffect();
        }

        private void InitBattleUnits(PartyData player, BossData boss)
        {
            AddUnit(PlayerManager.Instance.Player.Characters.Find(x => x.UDID == player.MemberUDID_0));
            AddUnit(PlayerManager.Instance.Player.Characters.Find(x => x.UDID == player.MemberUDID_1));
            AddUnit(PlayerManager.Instance.Player.Characters.Find(x => x.UDID == player.MemberUDID_2));
            AddUnit(PlayerManager.Instance.Player.Characters.Find(x => x.UDID == player.MemberUDID_3));
            m_allUnits.Add(new CombatUnit
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
            m_allUnits.Add(new CombatUnit
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

        private void SortUnitsBySpeed()
        {
            m_battlingUnits = new List<CombatUnit>(m_allUnits);
            m_battlingUnits.Sort((x, y) => x.GetSpeed().CompareTo(y.GetSpeed()));
        }

        private void StartProcessEquipmentEffect()
        {
            for(int i = 0; i < m_allUnits.Count; i++)
            {

            }
        }
    }
}
