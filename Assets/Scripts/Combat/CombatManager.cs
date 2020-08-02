using KahaGameCore.Interface;
using ProjectBS.Data;
using System.Collections.Generic;

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

        private CombatUnitEffectProcesser m_processer = null;
        private List<CombatUnitAction> m_unitActions = new List<CombatUnitAction>();

        private CombatUnit m_currentDyingUnit = null;

        public CombatUnitAction CurrentActionInfo { get; private set; }

        public void StartCombat(PartyData player, BossData boss)
        {
            m_units.Clear();
            AddUnit(PlayerManager.Instance.Player.Characters.Find(x => x.UDID == player.MemberUDID_0));
            AddUnit(PlayerManager.Instance.Player.Characters.Find(x => x.UDID == player.MemberUDID_1));
            AddUnit(PlayerManager.Instance.Player.Characters.Find(x => x.UDID == player.MemberUDID_2));
            AddUnit(PlayerManager.Instance.Player.Characters.Find(x => x.UDID == player.MemberUDID_3));
            m_units.Add(new CombatUnit
            {
                //ai = boss.AI,
                ai = "Boss AI here",
                rawAttack = boss.Attack,
                camp = CombatUnit.Camp.Boss,
                //rawDefence = boss.Defence,
                rawDefence = 100,
                //rawMaxHP = boss.HP,
                rawMaxHP = 10000,
                //HP = boss.HP,
                HP = 10000,
                //name = ContextConverter.Instance.GetContext(boss.NameContextID),
                name = "Boss",
                skills = "",
                //SP = boss.SP,
                SP = 100,
                //rawSpeed = boss.Speed,
                rawSpeed = 100,
                hatred = 1,
                //sprite = GameDataLoader.Instance.GetSprite(boss.CharacterSpriteID),
                body = null,
                foot = null,
                hand = null,
                head = null,
                buffs = new List<CombatUnit.Buff>(),
                statusAdders = new List<CombatUnit.StatusAdder>()
            });

            GetPage<UI.CombatUIView>().InitBattleUnits(m_units);
            GetPage<UI.CombatUIView>().Show(this, true, StartBattle);
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

        private void StartBattle()
        {
            m_currentTurn = 0;

            m_processer = new CombatUnitEffectProcesser(m_units);
            m_processer.Start(new CombatUnitEffectProcesser.ProcesserData
            {
                caster = null,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnBattleStarted,
                onEnded = StartNewTurn
            });
        }

        private void StartNewTurn()
        {
            m_currentTurn++;

            m_units.Sort((x, y) => x.GetSpeed().CompareTo(y.GetSpeed()));
            m_unitActions.Clear();

            for (int i = 0; i < m_units.Count; i++)
            {
                m_unitActions.Add(new CombatUnitAction(m_units[i], m_processer));
            }

            GetPage<UI.CombatUIView>().OnTurnStartAnimationEnded += OnTurnStartAnimationEnded;
            GetPage<UI.CombatUIView>().ShowTurnStart(m_currentTurn);
        }

        private void OnTurnStartAnimationEnded()
        {
            GetPage<UI.CombatUIView>().OnTurnStartAnimationEnded -= OnTurnStartAnimationEnded;

            m_processer.Start(new CombatUnitEffectProcesser.ProcesserData
            {
                caster = null,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnTurnStarted,
                onEnded = GoNextAction
            });
        }

        private void GoNextAction()
        {
            if(m_unitActions.Count <= 0)
            {
                return;
            }

            CurrentActionInfo = m_unitActions[0];
            m_unitActions.RemoveAt(0);

            CurrentActionInfo.Start(OnActionEnded);
        }

        private void OnActionEnded()
        {
            for(int i = 0; i < m_units.Count; i++)
            {
                if(m_units[i].HP <= 0)
                {
                    m_currentDyingUnit = m_units[i];
                    m_processer.Start(new CombatUnitEffectProcesser.ProcesserData
                    {
                        caster = null,
                        target = null,
                        timing = EffectProcesser.TriggerTiming.OnDied_Any,
                        onEnded = OnDied_Any_Ended
                    });
                    return;
                }
            }

            CheckGameEnd();
        }

        private void CheckGameEnd()
        {
            int _playerCount = 0;
            int _bossCount = 0;
            for (int i = 0; i < m_units.Count; i++)
            {
                if (m_units[i].camp == CombatUnit.Camp.Boss)
                    _bossCount++;
                else
                    _playerCount++;
            }

            if (_playerCount > 0 && _bossCount > 0)
            {
                if (m_unitActions.Count > 0)
                {
                    GoNextAction();
                }
                else
                {
                    EndTurn();
                }
            }
            else
            {
                if(_playerCount == 0)
                {
                }
                else
                {
                }
            }
        }

        private void OnDied_Any_Ended()
        {
            m_processer.Start(new CombatUnitEffectProcesser.ProcesserData
            {
                caster = m_currentDyingUnit,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnDied_Self,
                onEnded = OnDied_Self_Ended
            });
        }

        private void OnDied_Self_Ended()
        {
            m_units.Remove(m_currentDyingUnit);
            m_currentDyingUnit = null;
            OnActionEnded();
        }

        private void EndTurn()
        {
            m_processer.Start(new CombatUnitEffectProcesser.ProcesserData
            {
                caster = null,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnStartToEndTurn,
                onEnded = StartNewTurn
            });
        }
    }
}
