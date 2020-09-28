using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_LockAddStatus : EffectCommandBase
    {
        private string m_statusType = "";

        private List<CombatUnit> m_targets = null;
        private int m_currentTargetIndex = -1;

        private Action m_onCompleted = null;
        
        public override void Process(string[] vars, Action onCompleted)
        {
            m_statusType = vars[1];
            m_onCompleted = onCompleted;

            CombatTargetSelecter.Instance.StartSelect(
            new CombatTargetSelecter.SelectTargetData
            {
                attacker = processData.caster,
                commandString = vars[0],
                onSelected = OnTargetSelected
            });
        }

        private void OnTargetSelected(List<CombatUnit> targets)
        {
            m_targets = targets;
            m_currentTargetIndex = -1;
            SetNextTargetLock();
        }

        private void SetNextTargetLock()
        {
            m_currentTargetIndex++;
            if (m_currentTargetIndex >= m_targets.Count)
            {
                m_onCompleted?.Invoke();
                return;
            }

            m_targets[m_currentTargetIndex].statusAddLockers.Add(
                new CombatUnit.StatusAddLocker
                {
                    parentBuff = processData.referenceBuff,
                    statusType = m_statusType
                });

            SetNextTargetLock();
        }
    }
}
