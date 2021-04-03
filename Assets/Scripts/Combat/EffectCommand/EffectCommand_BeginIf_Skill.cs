using System;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_BeginIf_Skill : EffectCommandBase
    {
        private string m_checkIsOwning = "";
        private int m_skillID = 0;
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
            m_skillID = int.Parse(vars[2]);
            m_onCompleted = onCompleted;
            CombatTargetSelecter.Instance.StartSelect(
                new CombatTargetSelecter.SelectTargetData
                {
                    selectID = CombatTargetSelecter.Instance.GetSelectID(processData),
                    attacker = processData.caster,
                    currentEffectedTarget = processData.target,
                    commandString = vars[0],
                    onSelected = OnTargetSelected
                });
        }

        private void OnTargetSelected(List<CombatUnit> targets)
        {
            bool _checkIsOwning = int.Parse(m_checkIsOwning) == 1;

            for (int i = 0; i < targets.Count; i++)
            {
                bool _isOwning = false;

                for(int _skillIndex = 0; _skillIndex < targets[i].skills.Length; _skillIndex++)
                {
                    if(targets[i].skills[_skillIndex] == m_skillID)
                    {
                        _isOwning = true;
                        break;
                    }
                }

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
