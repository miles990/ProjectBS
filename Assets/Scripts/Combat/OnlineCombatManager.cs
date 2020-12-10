using System;
using System.Collections.Generic;
using ProjectBS.Network;

namespace ProjectBS.Combat
{
    public class OnlineCombatManager : CombatManagerBase
    {
        public override void AddActionIndex(CombatUnit unit, int addIndex)
        {
            throw new NotImplementedException();
        }

        public override void AddExtraAction(CombatUnit unit, bool isImmediate)
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

        public override void ForceRemoveUnit(CombatUnit unit)
        {
            throw new NotImplementedException();
        }

        public override void ForceUnitDie(CombatUnit unit, Action onDiedCommandEnded)
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
                    int _orginCamp = m_units[_refIndex].camp;
                    m_units[_refIndex] = units[i];
                    m_units[_refIndex].camp = _orginCamp; // camp will be different in different client
                }
            }
        }

        public override void StartCombat(List<CombatUnit> playerUnits, List<CombatUnit> opponentUnits)
        {
            m_units.Clear();
            m_units.AddRange(playerUnits);
            m_units.AddRange(opponentUnits);

            GetPage<UI.CombatUIView>().InitBattleUnits(m_units);
            GetPage<UI.CombatUIView>().Show(this, true, StartBattle);
        }

        private void StartBattle()
        {
            TurnCount = 0;

            List<CombatUnit> _myUnits = new List<CombatUnit>();
            for (int i = 0; i < m_units.Count; i++)
            {
                m_units[i].HP = m_units[i].GetMaxHP();
                if(m_units[i].camp == 0)
                {
                    _myUnits.Add(m_units[i]);
                }
            }

            AllUnitAllEffectProcesser = new AllCombatUnitAllEffectProcesser(_myUnits);
            if(PhotonManager.Instance.IsMaster)
            {
                TriggerOnBattleStarted();
            }
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
                PhotonManager.Instance.SetWaitCallback(CallbackCode.IsSlaveOnBattleStartedCommandEnded, StartNewTurn);
                PhotonManager.Instance.Call(nameof(TriggerOnBattleStarted));
            }
            else
            {
                PhotonManager.Instance.SyncMyCombatUnits();
                PhotonManager.Instance.SendCallback(CallbackCode.IsSlaveOnBattleStartedCommandEnded);
            }
        }

        private void StartNewTurn()
        {
            UnityEngine.Debug.Log("Start New Turn");
        }

        public void DoRPCCommand(string command)
        {
            switch(command)
            {
                case nameof(TriggerOnBattleStarted):
                    {
                        TriggerOnBattleStarted();
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
