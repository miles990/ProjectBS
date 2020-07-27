using KahaGameCore.Interface;
using ProjectBS.Data;
using System.Collections.Generic;
using System;
using ProjectBS.UI;

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

        private List<CombatUnit> m_units = new List<CombatUnit>();

        private int m_currentTurn = 0;
        private int m_currentActionIndex = -1;

        private CombatUnitEffectProcesser m_combatUnitEffectProcesserBuffer = null;

        public void StartCombat(PartyData player, BossData boss)
        {
            m_units.Clear();

            InitBattleUnits(player, boss);
            GetPage<CombatUIView>().InitBattleUnits(m_units);
            GetPage<CombatUIView>().Show(this, true, OnCombatInited);
        }

        //////////////////////////////////////////////////////////////////////////////

        private void InitBattleUnits(PartyData player, BossData boss)
        {
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
                //name = boss.NameContextID.ToString(),
                name = "Boss",
                skills = "",
                SP = boss.SP,
                rawSpeed = boss.Speed,
                hatred = 1,
                //sprite = GameDataLoader.Instance.GetSprite(boss.CharacterSpriteID),
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
                //name = GameDataLoader.Instance.GetCharacterName(character.CharacterNameID),
                name = "character " + character.UDID,
                skills = string.Format("{0},{1},{2},{3}", character.SkillSlot_0, character.SkillSlot_1, character.SKillSlot_2, character.SKillSlot_3),
                SP = character.SP,
                rawSpeed = character.Speed,
                hatred = 1,
                //sprite = GameDataLoader.Instance.GetSprite(character.CharacterSpriteID),
                body = PlayerManager.Instance.GetEquipmentByUDID(character.Equipment_UDID_Body),
                foot = PlayerManager.Instance.GetEquipmentByUDID(character.Equipment_UDID_Foot),
                hand = PlayerManager.Instance.GetEquipmentByUDID(character.Equipment_UDID_Hand),
                head = PlayerManager.Instance.GetEquipmentByUDID(character.Equipment_UDID_Head),
                buffs = new List<CombatUnit.Buff>(),
                statusAdders = new List<CombatUnit.StatusAdder>()
            });
        }

        private void OnCombatInited()
        {
            m_combatUnitEffectProcesserBuffer = new CombatUnitEffectProcesser(ref m_units);
            m_combatUnitEffectProcesserBuffer.Start(
                new CombatUnitEffectProcesser.ProcesserData
                {
                     caster = null,
                     target = null,
                     timing = EffectProcesser.TriggerTiming.OnBattleStarted,
                     onEnded = OnBattleStartedEffectEnded
                });
        }

        private void OnBattleStartedEffectEnded()
        {
            m_currentTurn = 0;
            GoNextTurn();
        }

        private void GoNextTurn()
        {
            m_units.Sort((x, y) => x.GetSpeed().CompareTo(y.GetSpeed()));

            m_currentTurn++;
            UnityEngine.Debug.Log("------------------------------");
            UnityEngine.Debug.Log("New Turn Start: Turn " + m_currentTurn);
            UnityEngine.Debug.Log("------------------------------");

            m_combatUnitEffectProcesserBuffer.Start(
                new CombatUnitEffectProcesser.ProcesserData
                {
                    caster = null,
                    target = null,
                    timing = EffectProcesser.TriggerTiming.OnTurnStarted,
                    onEnded = OnTurnStartedEffectEnded
                });
        }

        private void OnTurnStartedEffectEnded()
        {
            m_currentActionIndex = -1;
            GoNextUnitAction();
        }

        private void GoNextUnitAction()
        {
            m_currentActionIndex++;
            if(m_currentActionIndex >= m_units.Count)
            {
                EndTurn();
                return;
            }
            m_combatUnitEffectProcesserBuffer.Start(
                new CombatUnitEffectProcesser.ProcesserData
                {
                    caster = m_units[m_currentActionIndex],
                    target = null,
                    timing = EffectProcesser.TriggerTiming.OnSelfActionStarted,
                    onEnded = OnSelfActionStartedEffectEnded
                });
        }

        private void OnSelfActionStartedEffectEnded()
        {
            m_combatUnitEffectProcesserBuffer.Start(
                new CombatUnitEffectProcesser.ProcesserData
                {
                    caster = null,
                    target = null,
                    timing = EffectProcesser.TriggerTiming.OnActionStarted,
                    onEnded = OnActionStartedEffectEnded
                });
        }

        private void OnActionStartedEffectEnded()
        {
            if(m_units[m_currentActionIndex].camp == CombatUnit.Camp.Boss)
            {
                // FOR NOW
                // should process AI here
                UnityEngine.Debug.Log("Boss Turn, Skip Now");
                EndAction();
                return;
            }

            GetPage<CombatUIView>().OnSkillSelected += OnSkillSelected;
            GetPage<CombatUIView>().RefreshCurrentSkillMenu(m_units[m_currentActionIndex]);
        }

        private void OnSkillSelected(int skillIndex)
        {
            GetPage<CombatUIView>().OnSkillSelected -= OnSkillSelected;
            string[] _skills = m_units[m_currentActionIndex].skills.Split(',');
            SkillActiver.Active(m_units[m_currentActionIndex], GameDataLoader.Instance.GetSkill(_skills[skillIndex]), EndAction);
        }

        private void EndAction()
        {
            m_combatUnitEffectProcesserBuffer.Start(
                new CombatUnitEffectProcesser.ProcesserData
                {
                    caster = m_units[m_currentActionIndex],
                    target = null,
                    timing = EffectProcesser.TriggerTiming.OnStartToEndSelfAction,
                    onEnded = OnStartToEndSelfActionEffectEnded
                });
        }

        private void OnStartToEndSelfActionEffectEnded()
        {
            m_combatUnitEffectProcesserBuffer.Start(
                new CombatUnitEffectProcesser.ProcesserData
                {
                    caster = null,
                    target = null,
                    timing = EffectProcesser.TriggerTiming.OnStartToEndAction,
                    onEnded = GoNextUnitAction
                });
        }

        private void EndTurn()
        {
            m_combatUnitEffectProcesserBuffer.Start(
                new CombatUnitEffectProcesser.ProcesserData
                {
                    caster = null,
                    target = null,
                    timing = EffectProcesser.TriggerTiming.OnStartToEndTurn,
                    onEnded = OnStartToEndTurnEffectEnded
                });
        }

        private void OnStartToEndTurnEffectEnded()
        {
            // some UI effect here...
            GoNextTurn();
        }
    }
}
