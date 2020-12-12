using System;
using System.Collections.Generic;
using ProjectBS.Network;

namespace ProjectBS.Combat
{
    public class OnlineCombatManager : CombatManagerBase
    {
        private bool m_isCombating = false;

        public override void AddActionIndex(string unitUDID, int addIndex)
        {
            throw new NotImplementedException();
        }

        public override void AddExtraAction(string unitUDID, bool isImmediate)
        {
            throw new NotImplementedException();
        }

        public override void EndComabat(bool isWin)
        {
            throw new NotImplementedException();
        }

        public override void ForceEndCurrentAction()
        {
            throw new NotImplementedException();
        }

        public override void ForceRemoveUnit(string unitUDID)
        {
            throw new NotImplementedException();
        }

        public override void ForceUnitDie(string unitUDID, Action onDiedCommandEnded)
        {
            throw new NotImplementedException();
        }

        public override List<CombatUnit> GetSameCampUnits(int camp)
        {
            throw new NotImplementedException();
        }

        public override void SetCurrentActionMinAttackRoll(int value)
        {
            throw new NotImplementedException();
        }

        public override void SetCurrentActionMinDefenseRoll(int value)
        {
            throw new NotImplementedException();
        }

        public void ForceSyncCombatUnitsStatus(CombatUnit[] units)
        {
            for (int i = 0; i < units.Length; i++)
            {
                int _refIndex = m_units.FindIndex(x => x.UDID == units[i].UDID);
                if(_refIndex != -1)
                {
                    m_units[_refIndex].UpdateData(units[i]);
                }
            }
        }

        public override void StartCombat(List<CombatUnit> playerUnits, List<CombatUnit> opponentUnits)
        {
            m_units.Clear();
            m_units.AddRange(playerUnits);
            m_units.AddRange(opponentUnits);

            GetPage<UI.CombatUIView>().InitBattleUnits(m_units);
            GetPage<UI.CombatUIView>().Show(this, true, InitBattle);
        }

        public void Master_StartBattle()
        {
            if(m_isCombating)
            {
                throw new Exception("[OnlineCombatManager][StartBattle] is trying to start battle while is started");
            }
            m_isCombating = true;
            TriggerOnBattleStarted();
        }

        private void InitBattle()
        {
            TurnCount = 0;

            List<CombatUnit> _myUnits = new List<CombatUnit>();
            for (int i = 0; i < m_units.Count; i++)
            {
                if(m_units[i].camp == 0)
                {
                    _myUnits.Add(m_units[i]);
                    _myUnits[_myUnits.Count - 1].HP = _myUnits[_myUnits.Count - 1].GetMaxHP();
                }
            }

            AllUnitAllEffectProcesser = new AllCombatUnitAllEffectProcesser(_myUnits);
        }

        private void TriggerOnBattleStarted()
        {
            AllUnitAllEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = null,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnBattleStarted,
                onEnded = OnTriggerOnBattleStartEnded
            });
        }

        private void OnTriggerOnBattleStartEnded()
        {
            if(PhotonManager.Instance.IsMaster)
            {
                PhotonManager.Instance.SyncMyCombatUnits();
                PhotonManager.Instance.SetWaitCallback(CallbackCode.IsSlaveProcessCommandEnded, Master_StartNewTurn);
                PhotonManager.Instance.CallTheOther(nameof(TriggerOnBattleStarted));
            }
            else
            {
                PhotonManager.Instance.SyncMyCombatUnits();
                PhotonManager.Instance.SendCallback(CallbackCode.IsSlaveProcessCommandEnded);
            }
        }

        private void Master_StartNewTurn()
        {
            TurnCount++;

            m_units.Sort((x, y) => y.GetSpeed().CompareTo(x.GetSpeed()));
            for (int i = 0; i < m_units.Count - 1; i++)
            {
                if (m_units[i].GetSpeed() == m_units[i + 1].GetSpeed())
                {
                    if (UnityEngine.Random.Range(0, 101) <= 50)
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

            GetPage<UI.CombatUIView>().RefreshActionQueueInfo(m_unitActions);
            GetPage<UI.CombatUIView>().ShowTurnStart(TurnCount, OnTurnStartAnimationEnded);
        }

        private void OnTurnStartAnimationEnded()
        {
            // here will only be called by master
            TriggerOnTurnStarted();
        }

        private void TriggerOnTurnStarted()
        {
            AllUnitAllEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = null,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnTurnStarted,
                onEnded = OnTriggerOnTurnStartedEnded
            });
        }

        private void OnTriggerOnTurnStartedEnded()
        {
            if (PhotonManager.Instance.IsMaster)
            {
                PhotonManager.Instance.SyncMyCombatUnits();
                PhotonManager.Instance.SetWaitCallback(CallbackCode.IsSlaveProcessCommandEnded, GoNextAction);
                PhotonManager.Instance.CallTheOther(nameof(TriggerOnTurnStarted));
            }
            else
            {
                PhotonManager.Instance.SyncMyCombatUnits();
                PhotonManager.Instance.SendCallback(CallbackCode.IsSlaveProcessCommandEnded);
            }
        }

        private void GoNextAction()
        {
            if (m_unitActions.Count <= 0)
            {
                return;
            }

            m_currentAction = m_unitActions[0];
            m_unitActions.RemoveAt(0);

            GetPage<UI.CombatUIView>().RefreshActionQueueInfo(m_unitActions);

            if(m_currentAction.Actor.camp == 0)
            {
                m_currentAction.Start(null);
            }
            else
            {
                PhotonManager.Instance.CallTheOther(nameof(TriggerStartAction), m_currentAction.Actor.UDID);
            }
        }

        private void TriggerStartAction(string udid)
        {
            m_currentAction = new CombatUnitAction(GetUnitByUDID(udid), AllUnitAllEffectProcesser);
            m_currentAction.Start(null);
        }

        public void DoRPCCommand(string command, params object[] paras)
        {
            switch(command)
            {
                case nameof(TriggerOnBattleStarted):
                    {
                        TriggerOnBattleStarted();
                        break;
                    }
                case nameof(TriggerOnTurnStarted):
                    {
                        TriggerOnTurnStarted();
                        break;
                    }
                case nameof(TriggerStartAction):
                    {
                        TriggerStartAction((string)paras[0]);
                        break;
                    }
                default:
                    {
                        throw new Exception("[OnlineCombatManager][DoRPCCommand] Invaild RPC command=" + command);
                    }
            }
        }
    }
}
