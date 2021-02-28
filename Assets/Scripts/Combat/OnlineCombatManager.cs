using System;
using System.Collections.Generic;
using ProjectBS.Network;

namespace ProjectBS.Combat
{
    public class OnlineCombatManager : CombatManagerBase
    {
        private bool m_isCombating = false;
        private Action m_onDiedCommandEnded = null;
        private List<CombatUnit> m_myUnits = new List<CombatUnit>();

        // whenever do command, add 1 code into this.
        // if m_waitingQueue.count == 0, means all commands are done.
        private List<int> m_waitingQueue = new List<int>();

        private int m_usingCallbackCode = 300;

        private void SetNextCallbackCode()
        {
            if (m_usingCallbackCode == int.MaxValue)
            {
                m_usingCallbackCode = 300;
            }
            else
            {
                m_usingCallbackCode++;
            }
        }

        private void OnReceivedCallback()
        {
            m_waitingQueue.RemoveAt(0);
        }

        private void CallTheOtherWithDynamicCode(string method, string paras, Action onReceivedCallback)
        {
            m_waitingQueue.Add(0);
            SetNextCallbackCode();
            Action _action = OnReceivedCallback;
            if(onReceivedCallback != null)
            {
                _action += onReceivedCallback;
            }
            PhotonManager.Instance.SetWaitCallback(m_usingCallbackCode, _action);
            PhotonManager.Instance.CallTheOther(method, paras, m_usingCallbackCode);
        }

        public override void AddActionIndex(string unitUDID, int addIndex)
        {
            if(PhotonManager.Instance.IsMaster)
            {
                int _currentIndex = m_unitActions.FindIndex(x => x.Actor.UDID == unitUDID);
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
            else
            {
                CallTheOtherWithDynamicCode(nameof(SlaveToMaster_CalledToAddActionIndex), unitUDID + "," + addIndex, null);
            }
        }

        private void SlaveToMaster_CalledToAddActionIndex(int callbackCode, string udid, int addIndex)
        {
            if(!PhotonManager.Instance.IsMaster)
            {
                return;
            }

            AddActionIndex(udid, addIndex);
            PhotonManager.Instance.SendCallback(callbackCode);
        }

        public override void AddExtraAction(string unitUDID, bool isImmediate)
        {
            if(PhotonManager.Instance.IsMaster)
            {
                CombatUnitAction _newAction = new CombatUnitAction(GetUnitByUDID(unitUDID), GetNewAllProcesser());
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
            else
            {
                CallTheOtherWithDynamicCode(nameof(SalveToMaster_CalledToAddExtraAction), unitUDID + "," + isImmediate, null);
            }
        }

        private void SalveToMaster_CalledToAddExtraAction(int callbackCode, string unitUDID, bool isImmediate)
        {
            if (!PhotonManager.Instance.IsMaster)
            {
                return;
            }

            AddExtraAction(unitUDID, isImmediate);
            PhotonManager.Instance.SendCallback(callbackCode);
        }

        public override void EndComabat(bool isWin)
        {
            throw new NotImplementedException();
        }

        public override void ForceEndCurrentAction()
        {
            m_currentAction.ForceEnd();
        }

        public override void ForceRemoveUnit(string unitUDID)
        {
            if(PhotonManager.Instance.IsMaster)
            {
                GetPage<UI.CombatUIView>().RemoveActor(GetUnitByUDID(unitUDID));
                MasterToSlave_RemoveUnit(unitUDID);
                CallTheOtherWithDynamicCode(nameof(MasterToSlave_RemoveUnit), unitUDID, null);
            }
            else
            {
                CallTheOtherWithDynamicCode(nameof(SlaveToMaster_CalledToForceRemoveUnit), unitUDID, null);
            }
        }

        private void MasterToSlave_RemoveUnit(string unitUDID)
        {
            m_currentCheckBuffEndUnitIndex--; // might remove by buff, so need to decrease back
            m_unitActions.Remove(m_unitActions.Find(x => x.Actor.UDID == unitUDID));
            m_myUnits.Remove(GetUnitByUDID(unitUDID));
            m_units.Remove(GetUnitByUDID(unitUDID));
        }

        private void SlaveToMaster_CalledToForceRemoveUnit(int callbackCode, string unitUDID)
        {
            if (!PhotonManager.Instance.IsMaster)
            {
                return;
            }

            ForceRemoveUnit(unitUDID);
            PhotonManager.Instance.SendCallback(callbackCode);
        }

        public override void ForceUnitDie(string unitUDID, Action onDiedCommandEnded)
        {
            if(PhotonManager.Instance.IsMaster)
            {
                if (m_units.Find(x => x.UDID == unitUDID) == null)
                {
                    onDiedCommandEnded?.Invoke();
                    return;
                }

                CurrentDyingUnit = GetUnitByUDID(unitUDID);
                m_onDiedCommandEnded = onDiedCommandEnded;
                GetNewAllProcesser().Start(new AllCombatUnitAllEffectProcesser.ProcesserData
                {
                    caster = null,
                    target = null,
                    timing = EffectProcesser.TriggerTiming.OnDied_Other,
                    onEnded = OnDied_Any_Ended
                });
            }
            else
            {
                CallTheOtherWithDynamicCode(nameof(SlaveToMaster_CalledToForceUnitDie), unitUDID, OnActionEnded);
            }
        }

        private void SlaveToMaster_CalledToForceUnitDie(int callbackCode, string unitUDID)
        {
            if (!PhotonManager.Instance.IsMaster)
            {
                return;
            }

            ForceUnitDie(unitUDID, delegate { PhotonManager.Instance.SendCallback(callbackCode); });
        }

        private void OnDied_Any_Ended()
        {
            GetNewAllProcesser().Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = CurrentDyingUnit,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnDied_Self,
                onEnded = OnDied_Self_Ended
            });
        }

        private void OnDied_Self_Ended()
        {
            GetPage<UI.CombatUIView>().ShowUnitDied(CurrentDyingUnit, 
                delegate 
                {
                    ForceRemoveUnit(CurrentDyingUnit.UDID);
                    CurrentDyingUnit = null;
                    m_onDiedCommandEnded?.Invoke();
                });
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

        public override void Shake()
        {
            throw new NotImplementedException();
        }

        public void ForceUpdateCombatUnitsStatus(CombatUnit[] units)
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

            m_myUnits.Clear();
            for (int i = 0; i < m_units.Count; i++)
            {
                if(m_units[i].camp == 0)
                {
                    m_myUnits.Add(m_units[i]);
                    m_myUnits[m_myUnits.Count - 1].HP = m_myUnits[m_myUnits.Count - 1].GetMaxHP();
                }
            }
        }

        private void TriggerOnBattleStarted()
        {
            GetNewAllProcesser().Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = null,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnBattleStarted,
                onEnded = OnTriggerOnBattleStartEnded
            });
        }

        private void OnTriggerOnBattleStartEnded()
        {
            if (PhotonManager.Instance.IsMaster)
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

            for (int i = 0; i < m_units.Count; i++)
            {
                int _spd = m_units[i].GetSpeed();
                m_units[i].actionValue = _spd + Convert.ToInt32(UnityEngine.Random.Range(0f, (float)_spd * 0.1f));
            }

            m_units.Sort((x, y) => y.actionValue.CompareTo(x.actionValue));
            m_unitActions.Clear();

            for (int i = 0; i < m_units.Count; i++)
            {
                m_units[i].actionIndex = i;
                m_unitActions.Add(new CombatUnitAction(m_units[i], GetNewAllProcesser()));
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
            GetNewAllProcesser().Start(new AllCombatUnitAllEffectProcesser.ProcesserData
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
                // will be only called by master here
                EndTurn();
                return;
            }

            m_currentAction = m_unitActions[0];
            m_unitActions.RemoveAt(0);

            GetPage<UI.CombatUIView>().RefreshActionQueueInfo(m_unitActions);

            if(m_currentAction.Actor.camp == 0)
            {
                m_currentAction.Start(OnActionEnded);
            }
            else
            {
                PhotonManager.Instance.SetWaitCallback(CallbackCode.IsSlaveActionEnded, GoNextAction);
                PhotonManager.Instance.CallTheOther(nameof(TriggerStartAction), m_currentAction.Actor.UDID);
            }
        }

        private void TriggerStartAction(string udid)
        {
            m_currentAction = new CombatUnitAction(GetUnitByUDID(udid), GetNewAllProcesser());
            m_currentAction.Start(OnActionEnded);
        }

        private void OnActionEnded()
        {
            for (int i = 0; i < m_units.Count; i++)
            {
                if (m_units[i].HP <= 0)
                {
                    ForceUnitDie(m_units[i].UDID, OnActionEnded);
                    return;
                }
            }

            PhotonManager.Instance.SyncAllCombatUnits();
            if(!PhotonManager.Instance.IsMaster)
            {
                if(m_waitingQueue.Count == 0)
                {
                    PhotonManager.Instance.SendCallback(CallbackCode.IsSlaveActionEnded);
                }
            }
            else
            {
                GoNextAction();
            }
        }

        private void EndTurn()
        {
            GetNewAllProcesser().Start(new AllCombatUnitAllEffectProcesser.ProcesserData
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
            if (m_currentCheckBuffEndUnitIndex >= m_units.Count)
            {
                Master_StartNewTurn();
                return;
            }

            m_currentCheckingUnit = m_units[m_currentCheckBuffEndUnitIndex];
            m_currentCheckBuffIndex = -1;
            CheckNextBuffEnd();
        }

        private void CheckNextBuffEnd()
        {
            if (!m_units.Contains(m_currentCheckingUnit))
            {
                CheckNextUnitBuffEnd();
                return;
            }

            m_currentCheckBuffIndex++;
            if (m_currentCheckBuffIndex >= m_currentCheckingUnit.OwnBuffCount)
            {
                CheckNextUnitBuffEnd();
                return;
            }

            if (m_currentCheckingUnit.GetBuffByIndex(m_currentCheckBuffIndex).remainingTime == -1)
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
                m_currentCheckBuffIndex--;
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

        public void DoRPCCommand(string command, int callbackCode, string paras)
        {
            switch (command)
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
                        TriggerStartAction(paras);
                        break;
                    }
                case nameof(SlaveToMaster_CalledToAddActionIndex):
                    {
                        string[] _paraParts = paras.Split(',');
                        SlaveToMaster_CalledToAddActionIndex(callbackCode, _paraParts[1], int.Parse(_paraParts[2]));
                        break;
                    }
                case nameof(SalveToMaster_CalledToAddExtraAction):
                    {
                        string[] _paraParts = paras.Split(',');
                        SalveToMaster_CalledToAddExtraAction(callbackCode, _paraParts[0], bool.Parse(_paraParts[1]));
                        break;
                    }
                case nameof(SlaveToMaster_CalledToForceRemoveUnit):
                    {
                        SlaveToMaster_CalledToForceRemoveUnit(callbackCode, paras);
                        break;
                    }
                case nameof(SlaveToMaster_CalledToForceUnitDie):
                    {
                        SlaveToMaster_CalledToForceUnitDie(callbackCode, paras);
                        break;
                    }
                case nameof(MasterToSlave_RemoveUnit):
                    {
                        MasterToSlave_RemoveUnit(paras);
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
