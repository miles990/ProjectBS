﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_AddBuffTimeByTag : EffectCommandBase
    {
        private int m_tag = 0;
        private int m_addTime = 0;
        private List<CombatUnit> m_targets = null;
        private int m_currentTargetIndex = -1;
        private CombatUnit.Buff m_currentBuff = null;
        private Action m_onCompleted = null;
        private int m_currentBuffIndex = -1;

        public override void Process(string[] vars, Action onCompleted)
        {
            m_tag = int.Parse(vars[1]);

            m_addTime = int.Parse(vars[2]);

            m_onCompleted = onCompleted;

            CombatTargetSelecter.Instance.StartSelect(
            new CombatTargetSelecter.SelectTargetData
            {
                id = CombatTargetSelecter.Instance.GetSelectID(processData),
                attacker = processData.caster,
                currentEffectedTarget = processData.target,
                commandString = vars[0],
                onSelected = OnTargetSelected
            });
        }

        private void OnTargetSelected(List<CombatUnit> targets)
        {
            m_targets = targets;
            m_currentTargetIndex = -1;
            GoNextTarget();
        }

        private void GoNextTarget()
        {
            m_currentTargetIndex++;
            if (m_currentTargetIndex >= m_targets.Count)
            {
                m_onCompleted?.Invoke();
                return;
            }

            m_currentBuffIndex = -1;
            GoNextBuff();
        }

        private void GoNextBuff()
        {
            m_currentBuffIndex++;
            if (m_currentBuffIndex >= m_targets[m_currentTargetIndex].OwnBuffCount)
            {
                GoNextTarget();
                return;
            }

            if (m_targets[m_currentTargetIndex].GetBuffByIndex(m_currentBuffIndex).GetBuffSourceData().Tag == m_tag)
            {
                m_targets[m_currentTargetIndex].AddBuffTime(
                    m_targets[m_currentTargetIndex].GetBuffByIndex(m_currentBuffIndex),
                    m_addTime,
                    DisaplayRemoveBuff,
                    GoNextBuff);
            }
            else
            {
                GoNextBuff();
            }
        }

        private void DisaplayRemoveBuff()
        {
            m_currentBuffIndex--;
            GetPage<UI.CombatUIView>().DisplayRemoveBuff(new UI.CombatUIView.DisplayBuffData
            {
                buffName = ContextConverter.Instance.GetContext(m_currentBuff.GetBuffSourceData().NameContextID),
                taker = m_targets[m_currentTargetIndex]
            }, GoNextBuff);
        }
    }

}
