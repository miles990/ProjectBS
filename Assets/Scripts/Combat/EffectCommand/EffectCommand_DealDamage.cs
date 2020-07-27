using System;
using ProjectBS.Combat;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_DealDamage : EffectCommandBase
    {
        private Action m_onCompleted = null;
        private string m_valueString = "";

        public override void Process(string[] vars, Action onCompleted)
        {
            m_valueString = vars[1];
            m_onCompleted = onCompleted;

            CombatTargetSelecter.Instance.StartSelect(
                new CombatTargetSelecter.SelectTargetData
                {
                    attacker = caster,
                    commandString = vars[0],
                    onSelected = OnTargetSelected
                });
        }

        private void OnTargetSelected(List<CombatUnit> targets)
        {
            for(int i = 0; i < targets.Count; i++)
            {
                UnityEngine.Debug.Log("Selected:" + targets[i].name);
                UnityEngine.Debug.Log("m_valueString:" + m_valueString);
            }

            m_onCompleted?.Invoke();
        }
    }
}

