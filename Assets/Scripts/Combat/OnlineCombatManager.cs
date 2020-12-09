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
                Master_TriggerOnBattleStarted();
            }
        }

        private void Master_TriggerOnBattleStarted()
        {
            AllUnitAllEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = null,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnBattleStarted,
                onEnded = MasterCallSlaveTriggerOnBattleStarted
            });
        }

        private void MasterCallSlaveTriggerOnBattleStarted()
        {

        }

        public void DoRPCCommand()
        {

        }
    }
}
