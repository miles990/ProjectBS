﻿using KahaGameCore.Interface;
using ProjectBS.Data;
using System.Collections.Generic;

namespace ProjectBS.Combat
{
    public class CombatManager : Manager
    {
        public struct CombatActionInfo
        {
            public CombatUnit actor;
            public int minAttackRoll;
            public int minDefenseRoll;
        }
        public CombatActionInfo CurrentActionInfo 
        {
            get
            {
                return new CombatActionInfo
                {
                    actor = m_currentAction.Actor,
                    minAttackRoll = m_currentAction.MinAttackRoll,
                    minDefenseRoll = m_currentAction.MinDefenseRoll
                };
            }
        }

        public CombatManager() 
        {
            CombatUtility.SetCombatManager(this);
        } 

        public List<CombatUnit> AllUnit { get { return new List<CombatUnit>(m_units); } }
        private List<CombatUnit> m_units = new List<CombatUnit>();

        public int TurnCount { get; private set; } = 0;
        public CombatUnit CurrentDyingUnit { get; private set; }

        public AllCombatUnitAllEffectProcesser AllUnitAllEffectProcesser { get; private set; }
        private List<CombatUnitAction> m_unitActions = new List<CombatUnitAction>();
        private CombatUnitAction m_currentAction = null;

        private System.Action m_onDiedCommandEnded = null;

        private bool m_isCombating = false;

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

        public void AddActionIndex(CombatUnit unit, int addIndex)
        {
            int _currentIndex = m_unitActions.FindIndex(x => x.Actor == unit);
            if(_currentIndex != -1)
            {
                int _targetIndex = _currentIndex + addIndex;
                if (_targetIndex < 0)
                    _targetIndex = 0;
                if (_targetIndex >= m_unitActions.Count)
                    _targetIndex = m_unitActions.Count - 1;
                CombatUnitAction _refAction = m_unitActions[_currentIndex];
                m_unitActions.RemoveAt(_currentIndex);
                m_unitActions.Insert(_targetIndex, _refAction);
            }
        }
        
        public void AddExtraAction(CombatUnit unit, bool isImmediate)
        {
            CombatUnitAction _newAction = new CombatUnitAction(unit, AllUnitAllEffectProcesser);
            if(isImmediate)
            {
                m_unitActions.Insert(0, _newAction);
            }
            else
            {
                m_unitActions.Add(_newAction);
            }
        }

        public List<CombatUnit> GetSameCampUnits(CombatUnit.Camp camp)
        {
            List<CombatUnit> _units = new List<CombatUnit>();
            for (int i = 0; i < m_units.Count; i++)
            {
                if (m_units[i].camp == camp)
                {
                    _units.Add(m_units[i]);
                }
            }

            return _units;
        }

        public void SetCurrentActionMinAttackRoll(int value)
        {
            if (value < 0)
                value = 0;

            m_currentAction.MinAttackRoll = value;
        }

        public void SetCurrentActionMinDefenseRoll(int value)
        {
            if (value < 0)
                value = 0;

            m_currentAction.MinDefenseRoll = value;
        }

        public void StartCombat(PartyData player, List<BossData> bosses)
        {
            if(m_isCombating)
            {
                throw new System.Exception("[CombatManager][StartCombat] Is combating");
            }

            m_units.Clear();
            AddUnit(PlayerManager.Instance.Player.Characters.Find(x => x.UDID == player.MemberUDID_0));
            AddUnit(PlayerManager.Instance.Player.Characters.Find(x => x.UDID == player.MemberUDID_1));
            AddUnit(PlayerManager.Instance.Player.Characters.Find(x => x.UDID == player.MemberUDID_2));
            AddUnit(PlayerManager.Instance.Player.Characters.Find(x => x.UDID == player.MemberUDID_3));
            for(int i = 0; i < bosses.Count; i++)
            {
                AddBoss(bosses[i]);
            }
            for(int i = 0; i < m_units.Count; i++)
            {
                m_units[i].HP = m_units[i].GetMaxHP();
            }

            GetPage<UI.CombatUIView>().InitBattleUnits(m_units);
            GetPage<UI.CombatUIView>().Show(this, true, StartBattle);

            m_isCombating = true;
        }

        public void EndComabat(bool isWin)
        {
            GetPage<UI.CombatUIView>().Show(this, false, 
                delegate
                {
                    GameManager.Instance.EndCombat(isWin);
                });
        }

        public void ForceEndCurrentAction()
        {
            m_currentAction.ForceEnd();
        }

        public void ForceUnitDie(CombatUnit unit, System.Action onDiedCommandEnded)
        {
            if(!m_units.Contains(unit))
            {
                onDiedCommandEnded?.Invoke();
                return;
            }

            CurrentDyingUnit = unit;
            m_onDiedCommandEnded = onDiedCommandEnded;
            AllUnitAllEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = null,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnDied_Any,
                onEnded = OnDied_Any_Ended
            });
        }

        public void ForceRemoveUnit(CombatUnit unit)
        {
            m_unitActions.Remove(m_unitActions.Find(x => x.Actor == unit));
            m_units.Remove(unit);
            GetPage<UI.CombatUIView>().RemoveActor(unit);
        }

        private void AddUnit(OwningCharacterData character)
        {
            m_units.Add(new CombatUnit
            {
                ai = "",
                rawAttack = character.Attack,
                camp = CombatUnit.Camp.Player,
                rawDefense = character.Defense,
                rawMaxHP = character.HP,
                name = ContextConverter.Instance.GetContext(character.CharacterNameID),
                SP = character.SP,
                rawSpeed = character.Speed,
                Hatred = 1,
                //sprite = GameDataLoader.Instance.GetSprite(character.CharacterSpriteID),
                body = PlayerManager.Instance.GetEquipmentByUDID(character.Equipment_UDID_Body),
                foot = PlayerManager.Instance.GetEquipmentByUDID(character.Equipment_UDID_Foot),
                hand = PlayerManager.Instance.GetEquipmentByUDID(character.Equipment_UDID_Hand),
                head = PlayerManager.Instance.GetEquipmentByUDID(character.Equipment_UDID_Head),
                buffs = new List<CombatUnit.Buff>(),
                statusAdders = new List<CombatUnit.StatusAdder>()
            });

            m_units[m_units.Count - 1].skills[0] = character.SkillSlot_0;
            m_units[m_units.Count - 1].skills[1] = character.SkillSlot_1;
            m_units[m_units.Count - 1].skills[2] = character.SkillSlot_2;
            m_units[m_units.Count - 1].skills[3] = character.SkillSlot_3;
        }

        private void AddBoss(BossData boss)
        {
            CombatUnit _boss = new CombatUnit
            {
                ai = boss.AI,
                rawAttack = boss.Attack,
                camp = CombatUnit.Camp.Enemy,
                rawDefense = boss.Defense,
                rawMaxHP = boss.HP,
                HP = boss.HP,
                name = ContextConverter.Instance.GetContext(boss.NameContextID),
                SP = boss.SP,
                rawSpeed = boss.Speed,
                Hatred = 1,
                //sprite = GameDataLoader.Instance.GetSprite(boss.CharacterSpriteID),
                body = null,
                foot = null,
                hand = null,
                head = null,
                buffs = new List<CombatUnit.Buff>(),
                statusAdders = new List<CombatUnit.StatusAdder>()
            };

            if (!string.IsNullOrEmpty(boss.BuffIDs))
            {
                string[] _buffIDs = boss.BuffIDs.Split(';');
                for (int i = 0; i < _buffIDs.Length; i++)
                {
                    _boss.buffs.Add(new CombatUnit.Buff
                    {
                        soruceID = int.Parse(_buffIDs[i]),
                        from = _boss,
                        owner = _boss,
                        remainingTime = -1,
                        stackCount = 1
                    });
                }
            }

            m_units.Add(_boss);
        }

        private void StartBattle()
        {
            TurnCount = 0;

            AllUnitAllEffectProcesser = new AllCombatUnitAllEffectProcesser(m_units);
            AllUnitAllEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
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
            for (int i = 0; i < m_units.Count - 1; i++)
            {
                if(m_units[i].GetSpeed() == m_units[i + 1].GetSpeed())
                {
                    if(UnityEngine.Random.Range(0, 101) <= 50)
                    {
                        CombatUnit _temp = m_units[i + 1];
                        m_units[i + 1] = m_units[i];
                        m_units[i] = _temp;
                    }
                }
            }
            m_unitActions.Clear();

            for (int i = 0; i < m_units.Count; i++)
            {
                m_units[i].actionIndex = i;
                m_unitActions.Add(new CombatUnitAction(m_units[i], AllUnitAllEffectProcesser));
            }

            GetPage<UI.CombatUIView>().ShowTurnStart(TurnCount, OnTurnStartAnimationEnded);
        }

        private void OnTurnStartAnimationEnded()
        {
            AllUnitAllEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
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
                    ForceUnitDie(m_units[i], OnActionEnded);
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
                if (m_units[i].camp == CombatUnit.Camp.Enemy)
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
                m_isCombating = false;
                GetPage<UI.CombatUIView>().ShowGameEnd(_playerCount != 0);
            }
        }

        private void OnDied_Any_Ended()
        {
            AllUnitAllEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = CurrentDyingUnit,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnDied_Self,
                onEnded = OnDied_Self_Ended
            });
        }

        private void OnDied_Self_Ended()
        {
            ForceRemoveUnit(CurrentDyingUnit);
            GetPage<UI.CombatUIView>().ShowUnitDied(CurrentDyingUnit,
                delegate 
                {
                    CurrentDyingUnit = null;
                    m_onDiedCommandEnded?.Invoke();
                });
        }

        private void EndTurn()
        {
            AllUnitAllEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
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
            GetPage<UI.CombatUIView>().RefreshAllInfo();

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
                m_units[m_currentCheckBuffEndUnitIndex].RemoveBuff(
                    _buff,
                    delegate { DisplayRemoveBuff(_buff); });
            }
            else
            {
                CheckNextBuffEnd();
            }
        }

        private void DisplayRemoveBuff(CombatUnit.Buff _buff)
        {
            if(_buff.owner != CurrentActionInfo.actor)
            {
                CheckNextBuffEnd();
                return;
            }

            if(m_currentCheckBuffEndUnitIndex >= m_units.Count)
            {
                CheckNextUnitBuffEnd();
                return;
            }

            GetPage<UI.CombatUIView>().DisplayRemoveBuff(new UI.CombatUIView.DisplayBuffData
            {
                buffName = ContextConverter.Instance.GetContext(_buff.GetBuffSourceData().NameContextID),
                taker = m_units[m_currentCheckBuffEndUnitIndex]
            }, CheckNextBuffEnd);
        }
    }
}
