using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            throw new NotImplementedException();
        }
    }
}
