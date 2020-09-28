using System;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_SetStatus : EffectCommandBase
    {
        private Action m_onCompleted = null;
        private string m_statusString = "";
        private string m_valueString = "";

        private List<CombatUnit> m_targets = null;
        private int m_currentTargetIndex = -1;

        public override void Process(string[] vars, Action onCompleted)
        {
            m_onCompleted = onCompleted;
            m_statusString = vars[1].Trim();
            m_valueString = vars[2].Trim();

            CombatTargetSelecter.Instance.StartSelect(new CombatTargetSelecter.SelectTargetData
            {
                attacker = GetSelf(),
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

            float _value = CombatUtility.Calculate(
                new CombatUtility.CalculateData
                {
                    caster = processData.caster,
                    target = processData.target,
                    formula = m_valueString,
                    useRawValue = true
                });
            int _set = Convert.ToInt32(_value);

            switch (m_statusString)
            {
                case Keyword.Attack:
                    {
                        m_targets[m_currentTargetIndex].rawAttack = _set;
                        break;
                    }
                case Keyword.Defence:
                    {
                        m_targets[m_currentTargetIndex].rawDefence = _set;
                        break;
                    }
                case Keyword.MaxHP:
                    {
                        m_targets[m_currentTargetIndex].rawMaxHP = _set;
                        break;
                    }
                case Keyword.Speed:
                    {
                        m_targets[m_currentTargetIndex].rawSpeed = _set;
                        break;
                    }
                case Keyword.Hatred:
                    {
                        m_targets[m_currentTargetIndex].Hatred = _set;
                        break;
                    }
                case Keyword.HP:
                    {
                        m_targets[m_currentTargetIndex].HP = _set;
                        break;
                    }
                case Keyword.SP:
                    {
                        m_targets[m_currentTargetIndex].SP = _set;
                        break;
                    }
                default:
                    {
                        throw new Exception("[EffectCommand_SetStatus][Process] Invaild status:" + m_statusString);
                    }
            }
            GetPage<UI.CombatUIView>().RefreshAllInfo();

            GoNextTarget();
        }
    }
}
