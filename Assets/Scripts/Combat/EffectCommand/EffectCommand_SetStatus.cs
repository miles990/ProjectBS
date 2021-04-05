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

            AddSkillOrEffectInfo();

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

            string _statusInfoName = "";

            float _value = CombatUtility.Calculate(
                new CombatUtility.CalculateData
                {
                    caster = processData.caster,
                    target = processData.target,
                    formula = m_valueString,
                    processData = processData,
                    useRawValue = true
                });
            int _set = Convert.ToInt32(_value);

            switch (m_statusString)
            {
                case Keyword.Attack:
                    {
                        m_targets[m_currentTargetIndex].rawAttack = _set;
                        _statusInfoName = ContextConverter.Instance.GetContext(1000020);
                        break;
                    }
                case Keyword.Defense:
                    {
                        m_targets[m_currentTargetIndex].rawDefense = _set;
                        _statusInfoName = ContextConverter.Instance.GetContext(1000021);
                        break;
                    }
                case Keyword.MaxHP:
                    {
                        m_targets[m_currentTargetIndex].rawMaxHP = _set;
                        _statusInfoName = ContextConverter.Instance.GetContext(1000018);
                        break;
                    }
                case Keyword.Speed:
                    {
                        m_targets[m_currentTargetIndex].rawSpeed = _set;
                        _statusInfoName = ContextConverter.Instance.GetContext(1000022);
                        break;
                    }
                case Keyword.Hatred:
                    {
                        m_targets[m_currentTargetIndex].Hatred = _set;
                        _statusInfoName = ContextConverter.Instance.GetContext(1000023);
                        break;
                    }
                case Keyword.HP:
                    {
                        m_targets[m_currentTargetIndex].HP = _set;
                        _statusInfoName = ContextConverter.Instance.GetContext(1000019);
                        break;
                    }
                case Keyword.SP:
                    {
                        m_targets[m_currentTargetIndex].SP = _set;
                        _statusInfoName = ContextConverter.Instance.GetContext(1000024);
                        break;
                    }
                default:
                    {
                        throw new Exception("[EffectCommand_SetStatus][Process] Invaild status:" + m_statusString);
                    }
            }

            GetPage<UI.CombatUIView>().AddCombatInfo
                (
                    string.Format
                    (
                        ContextConverter.Instance.GetContext(500029),
                        m_targets[m_currentTargetIndex].name,
                        _statusInfoName,
                        _set
                    ), null
                );

            GoNextTarget();
        }
    }
}
