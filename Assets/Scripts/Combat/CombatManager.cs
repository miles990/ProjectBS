using System.Collections.Generic;

namespace ProjectBS.Combat
{
    public class CombatManager : CombatManagerBase
    {
        private bool m_isCombating = false;

        private System.Action m_onDiedCommandEnded = null;

        public override void AddActionIndex(string unit, int addIndex)
        {
            int _currentIndex = m_unitActions.FindIndex(x => x.Actor.UDID == unit);
            if (_currentIndex != -1)
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

            GetPage<UI.CombatUIView>().RefreshActionQueueInfo(m_unitActions);
        }

        public override void AddExtraAction(string unit, bool isImmediate)
        {
            CombatUnitAction _newAction = new CombatUnitAction(GetUnitByUDID(unit), AllUnitAllEffectProcesser);
            if (isImmediate)
            {
                m_unitActions.Insert(0, _newAction);
            }
            else
            {
                m_unitActions.Add(_newAction);
            }

            GetPage<UI.CombatUIView>().RefreshActionQueueInfo(m_unitActions);
        }

        public override List<CombatUnit> GetSameCampUnits(int camp)
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

        public override void SetCurrentActionMinAttackRoll(int value)
        {
            if (value < 0)
                value = 0;

            m_currentAction.MinAttackRoll = value;
        }

        public override void SetCurrentActionMinDefenseRoll(int value)
        {
            if (value < 0)
                value = 0;

            m_currentAction.MinDefenseRoll = value;
        }

        public override void EndComabat(bool isWin)
        {
            GetPage<UI.CombatUIView>().Show(this, false,
                delegate
                {
                    GameManager.Instance.EndCombat(isWin);
                });
        }

        public override void ForceEndCurrentAction()
        {
            m_currentAction.ForceEnd();
        }

        public override void ForceUnitDie(string unit, System.Action onDiedCommandEnded)
        {
            if (m_units.Find(x => x.UDID == unit) == null)
            {
                onDiedCommandEnded?.Invoke();
                return;
            }

            CurrentDyingUnit = GetUnitByUDID(unit);
            m_onDiedCommandEnded = onDiedCommandEnded;
            AllUnitAllEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = CurrentDyingUnit,
                target = CurrentDyingUnit,
                timing = EffectProcesser.TriggerTiming.OnDied_Any,
                onEnded = OnDied_Any_Ended
            });
        }

        public override void ForceRemoveUnit(string unit)
        {
            m_currentCheckBuffEndUnitIndex--; // might remove by buff, so need to decrease back
            GetPage<UI.CombatUIView>().RemoveActor(GetUnitByUDID(unit));
            m_unitActions.Remove(m_unitActions.Find(x => x.Actor.UDID == unit));
            m_units.Remove(GetUnitByUDID(unit));
        }

        public override void StartCombat(List<CombatUnit> camp0Units, List<CombatUnit> camp1Units)
        {
            if(m_isCombating)
            {
                throw new System.Exception("[CombatManager][StartCombat] Is combating");
            }

            m_units.Clear();
            m_units.AddRange(camp0Units);
            m_units.AddRange(camp1Units);

            GetPage<UI.CombatUIView>().InitBattleUnits(m_units);
            GetPage<UI.CombatUIView>().Show(this, true, StartBattle);

            m_isCombating = true;
        }

        private void StartBattle()
        {
            TurnCount = 0;

            for(int i = 0; i < m_units.Count; i++)
            {
                m_units[i].HP = m_units[i].GetMaxHP();
            }

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

            for (int i = 0; i < m_units.Count - 1; i++)
            {
                int _spd = m_units[i].GetSpeed();
                m_units[i].actionValue = _spd + System.Convert.ToInt32(UnityEngine.Random.Range(0f, (float)_spd * 0.1f));
            }

            m_units.Sort((x, y) => y.actionValue.CompareTo(x.actionValue));

            m_unitActions.Clear();

            for (int i = 0; i < m_units.Count; i++)
            {
                m_units[i].actionIndex = i;
                m_unitActions.Add(new CombatUnitAction(m_units[i], AllUnitAllEffectProcesser));
            }

            GetPage<UI.CombatUIView>().RefreshActionQueueInfo(m_unitActions);
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

            GetPage<UI.CombatUIView>().RefreshActionQueueInfo(m_unitActions);
        }

        private void OnActionEnded()
        {
            for(int i = 0; i < m_units.Count; i++)
            {
                if(m_units[i].HP <= 0)
                {
                    ForceUnitDie(m_units[i].UDID, OnActionEnded);
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
                if (m_units[i].camp == 1)
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
                target = CurrentDyingUnit,
                timing = EffectProcesser.TriggerTiming.OnDied_Self,
                onEnded = OnDied_Self_Ended
            });
        }

        private void OnDied_Self_Ended()
        {
            ForceRemoveUnit(CurrentDyingUnit.UDID);
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
        private CombatUnit m_currentCheckingUnit = null;
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

            m_currentCheckingUnit = m_units[m_currentCheckBuffEndUnitIndex];
            m_currentCheckBuffIndex = -1;
            CheckNextBuffEnd();
        }

        private void CheckNextBuffEnd()
        {
            if(!m_units.Contains(m_currentCheckingUnit))
            {
                CheckNextUnitBuffEnd();
                return;
            }

            m_currentCheckBuffIndex++;
            if(m_currentCheckBuffIndex >= m_currentCheckingUnit.OwnBuffCount)
            {
                CheckNextUnitBuffEnd();
                return;
            }

            if(m_currentCheckingUnit.GetBuffByIndex(m_currentCheckBuffIndex).remainingTime == -1)
            {
                CheckNextBuffEnd();
                return;
            }

            m_currentCheckingUnit.GetBuffByIndex(m_currentCheckBuffIndex).remainingTime--;

            if (m_currentCheckingUnit.GetBuffByIndex(m_currentCheckBuffIndex).remainingTime <= 0)
            {
                CombatUnit.Buff _buff = m_currentCheckingUnit.GetBuffByIndex(m_currentCheckBuffIndex);
                m_currentCheckingUnit.RemoveBuff(
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
            GetPage<UI.CombatUIView>().DisplayRemoveBuff(new UI.CombatUIView.DisplayBuffData
            {
                buffName = ContextConverter.Instance.GetContext(_buff.GetBuffSourceData().NameContextID),
                taker = m_currentCheckingUnit
            }, CheckNextBuffEnd);
        }
    }
}
