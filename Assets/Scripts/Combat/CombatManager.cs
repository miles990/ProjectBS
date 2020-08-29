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

        public struct CombatActionInfo
        {
            public CombatUnit actor;
        }
        public CombatActionInfo CurrentActionInfo 
        {
            get
            {
                return new CombatActionInfo
                {
                    actor = m_currentAction.Actor
                };
            }
        }

        private CombatManager() { } 

        public List<CombatUnit> AllUnit { get { return new List<CombatUnit>(m_units); } }
        private List<CombatUnit> m_units = new List<CombatUnit>();

        public int TurnCount { get; private set; } = 0;

        private AllCombatUnitAllEffectProcesser m_processer = null;
        private List<CombatUnitAction> m_unitActions = new List<CombatUnitAction>();
        private CombatUnitAction m_currentAction = null;

        private CombatUnit m_currentDyingUnit = null;

        public int GetCampCount(CombatUnit.Camp camp)
        {
            if(TurnCount == 0)
            {
                return 0;
            }

            int _count = 0;
            for(int i = 0; i < m_units.Count; i++)
            {
                if(m_units[i].camp == camp)
                {
                    _count++;
                }
            }

            return _count;
        }

        public int GetNotCampCount(CombatUnit.Camp camp)
        {
            return m_units.Count - GetCampCount(camp);
        }

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
                name = ContextConverter.Instance.GetContext(boss.NameContextID),
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

            GetPage<UI.CombatUIView>().InitBattleUnits(m_units);
            GetPage<UI.CombatUIView>().Show(this, true, StartBattle);
        }

        public void SetForceStopCurrentAction()
        {
            m_currentAction.SetForceStopOnStart();
        }

        public void ShowDamage(UI.CombatUIView.DisplayDamageData displayDamageData)
        {
            GetPage<UI.CombatUIView>().DisplayDamage(displayDamageData);
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
            TurnCount = 0;

            m_processer = new AllCombatUnitAllEffectProcesser(m_units);
            m_processer.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = null,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnBattleStarted,
                onEnded = StartNewTurn
            });
        }

        private void StartNewTurn()
        {
            TurnCount++;

            m_units.Sort((x, y) => y.GetSpeed().CompareTo(x.GetSpeed()));
            m_unitActions.Clear();

            for (int i = 0; i < m_units.Count; i++)
            {
                m_unitActions.Add(new CombatUnitAction(m_units[i], m_processer));
            }

            GetPage<UI.CombatUIView>().OnTurnStartAnimationEnded += OnTurnStartAnimationEnded;
            GetPage<UI.CombatUIView>().ShowTurnStart(TurnCount);
        }

        private void OnTurnStartAnimationEnded()
        {
            GetPage<UI.CombatUIView>().OnTurnStartAnimationEnded -= OnTurnStartAnimationEnded;

            m_processer.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
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

            m_currentAction = m_unitActions[0];
            m_unitActions.RemoveAt(0);
            m_currentAction.Start(OnActionEnded);
        }

        private void OnActionEnded()
        {
            for(int i = 0; i < m_units.Count; i++)
            {
                if(m_units[i].HP <= 0)
                {
                    m_currentDyingUnit = m_units[i];
                    m_processer.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
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
                GetPage<UI.CombatUIView>().ShowGameEnd(_playerCount != 0);
            }
        }

        private void OnDied_Any_Ended()
        {
            m_processer.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = m_currentDyingUnit,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnDied_Self,
                onEnded = OnDied_Self_Ended
            });
        }

        private void OnDied_Self_Ended()
        {
            GetPage<UI.CombatUIView>().ShowUnitDied(m_currentDyingUnit);
            m_units.Remove(m_currentDyingUnit);
            m_unitActions.Remove(m_unitActions.Find(x => x.Actor == m_currentDyingUnit));

            m_currentDyingUnit = null;

            OnActionEnded();
        }

        private void EndTurn()
        {
            m_processer.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = null,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnStartToEndTurn,
                onEnded = OnStartToEndTurnEnded
            });
        }

        private int m_currentCheckBuffEndUnitIndex = -1;
        private int m_currentCheckBuffIndex = -1;
        private void OnStartToEndTurnEnded()
        {
            m_currentCheckBuffEndUnitIndex = -1;
            CheckNextUnitBuffEnd();
        }

        private void CheckNextUnitBuffEnd()
        {
            m_currentCheckBuffEndUnitIndex++;
            if(m_currentCheckBuffEndUnitIndex >= m_units.Count)
            {
                StartNewTurn();
                return;
            }

            m_currentCheckBuffIndex = -1;
            CheckNextBuffEnd();
        }

        private void CheckNextBuffEnd()
        {
            m_currentCheckBuffIndex++;
            if(m_currentCheckBuffIndex >= m_units[m_currentCheckBuffEndUnitIndex].buffs.Count)
            {
                CheckNextUnitBuffEnd();
                return;
            }

            if(m_units[m_currentCheckBuffEndUnitIndex].buffs[m_currentCheckBuffIndex].remainingTime == -1)
            {
                CheckNextBuffEnd();
                return;
            }

            m_units[m_currentCheckBuffEndUnitIndex].buffs[m_currentCheckBuffIndex].remainingTime--;

            if (m_units[m_currentCheckBuffEndUnitIndex].buffs[m_currentCheckBuffIndex].remainingTime <= 0)
            {
                CombatUnit.Buff _buff = m_units[m_currentCheckBuffEndUnitIndex].buffs[m_currentCheckBuffIndex];
                m_units[m_currentCheckBuffEndUnitIndex].buffs.RemoveAt(m_currentCheckBuffIndex);
                m_currentCheckBuffIndex--;

                SkillEffectData _effect = KahaGameCore.Static.GameDataManager.GetGameData<SkillEffectData>(_buff.effectID);
                new EffectProcesser(_effect.Command).Start(new EffectProcesser.ProcessData
                {
                    caster = m_units[m_currentCheckBuffEndUnitIndex],
                    target = null,
                    timing = EffectProcesser.TriggerTiming.OnDeactived,
                    allEffectProcesser = m_processer,
                    referenceBuff = _buff,
                    refenceSkill = null,
                    onEnded = CheckNextBuffEnd
                });
            }
            else
            {
                CheckNextBuffEnd();
            }
        }
    }
}
