using System;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_BeginIf_HasEffectTag : EffectCommandBase
    {
        private string m_checkIsOwning = "";
        private string m_tag = "";
        private Action m_onCompleted = null;

        public override void Process(string[] vars, Action onCompleted)
        {
            if (processData.skipIfCount > 0)
            {
                processData.skipIfCount++;
                onCompleted?.Invoke();
                return;
            }

            m_checkIsOwning = vars[1];
            m_tag = vars[2];
            m_onCompleted = onCompleted;
            CombatTargetSelecter.Instance.StartSelect(
                new CombatTargetSelecter.SelectTargetData
                {
                    id = GetSelectID(),
                    attacker = processData.caster,
                    currentEffectedTarget = processData.target,
                    commandString = vars[0],
                    onSelected = OnTargetSelected
                });
        }

        private void OnTargetSelected(List<CombatUnit> targets)
        {
            bool _checkIsOwning = int.Parse(m_checkIsOwning) == 1;
            int _tag = int.Parse(m_tag);

            for (int i = 0; i < targets.Count; i++)
            {
                bool _isOwning = targets[i].HasBuffWithTag(_tag);
                if (_isOwning != _checkIsOwning)
                {
                    processData.skipIfCount++;
                    m_onCompleted?.Invoke();
                    return;
                }
            }
            m_onCompleted?.Invoke();
        }
    }
}
