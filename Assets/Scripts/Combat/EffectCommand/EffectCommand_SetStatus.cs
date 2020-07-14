using System;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_SetStatus : EffectCommandBase
    {
        private string m_targetStatus = "";
        private string m_value = "";
        private Action m_onCompleted = null;

        public EffectCommand_SetStatus(CombatManager.CombatUnit caster) : base(caster)
        {
        }

        public override void Process(string[] vars, Action onCompleted)
        {
            m_targetStatus = vars[1];
            m_value = vars[2];
            m_onCompleted = onCompleted;
            CombatUtility.SetTarget(Caster, vars[0], OnTargetsSet);
        }

        private void OnTargetsSet(List<CombatManager.CombatUnit> units)
        {
            for(int i = 0; i < units.Count; i++)
            {
                switch(m_targetStatus)
                {
                    case "HP":
                        {
                            units[i].rawHP = CombatUtility.GetValue(Caster, units[i], m_value);
                            break;
                        }
                    case "SP":
                        {
                            units[i].rawSP = CombatUtility.GetValue(Caster, units[i], m_value);
                            break;
                        }
                    case "Attack":
                        {
                            units[i].rawAttack = CombatUtility.GetValue(Caster, units[i], m_value);
                            break;
                        }
                    case "Defence":
                        {
                            units[i].rawDefence = CombatUtility.GetValue(Caster, units[i], m_value);
                            break;
                        }
                    case "Speed":
                        {
                            units[i].rawSpeed = CombatUtility.GetValue(Caster, units[i], m_value);
                            break;
                        }
                }
            }

            // some vfx here...

            m_onCompleted?.Invoke();
        }
    }
}
