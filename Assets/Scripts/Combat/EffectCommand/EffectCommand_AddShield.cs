using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_AddShield : EffectCommandBase
    {
        private Action m_onCompleted = null;
        private string m_valueString = "";
        private int m_skillID = 0;

        private List<CombatUnit> m_targets = null;
        private int m_currentTargetIndex = -1;

        public override void Process(string[] vars, Action onCompleted)
        {
            m_onCompleted = onCompleted;
            m_valueString = vars[1];
            m_skillID = int.Parse(vars[2]);

            CombatTargetSelecter.Instance.StartSelect(new CombatTargetSelecter.SelectTargetData
            {
                selectID = CombatTargetSelecter.Instance.GetSelectID(processData),
                attacker = GetSelf(),
                currentEffectedTarget = processData.target,
                commandString = vars[0],
                onSelected = OnSelected
            });
        }

        private void OnSelected(List<CombatUnit> targets)
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

            float _floatValue = CombatUtility.Calculate(new CombatUtility.CalculateData
            {
                caster = processData.caster,
                target = m_targets[m_currentTargetIndex],
                formula = m_valueString,
                processData = processData,
                useRawValue = false
            });

            if(processData.referenceBuff == null)
            {
                m_targets[m_currentTargetIndex].shields.Add(new CombatUnit.Shield
                {
                    parentBuffID = 0,
                    triggerSKillID = m_skillID,
                    value = Convert.ToInt32(_floatValue)
                });
            }
            else
            {
                CombatUnit.Shield _shield = m_targets[m_currentTargetIndex].shields.Find(x => x.parentBuffID == processData.referenceBuff.soruceID);
                if(_shield != null)
                {
                    _shield.value = Convert.ToInt32(_floatValue);
                }
                else
                {
                    m_targets[m_currentTargetIndex].shields.Add(new CombatUnit.Shield
                    {
                        parentBuffID = 0,
                        triggerSKillID = m_skillID,
                        value = Convert.ToInt32(_floatValue)
                    });
                }
            }
        }
    }
}
