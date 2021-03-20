using System;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_BeginIf_LastSkillTag : EffectCommandBase
    {
        private string m_checkIsOwning = "";
        private int m_tag = 0;
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
            m_tag = int.Parse(vars[2]);
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
            bool _checkIsOwning = int.Parse(m_checkIsOwning) == 1;

            for (int i = 0; i < targets.Count; i++)
            {
                bool _isOwning = false;

                if(!string.IsNullOrEmpty(GameDataManager.GetGameData<Data.SkillData>(targets[i].lastSkillID).Tag))
                {
                    string[] _tags = GameDataManager.GetGameData<Data.SkillData>(targets[i].lastSkillID).Tag.Split(';');
                    for (int j = 0; j < _tags.Length; j++)
                    {
                        if (int.Parse(_tags[j]) == m_tag)
                        {
                            _isOwning = true;
                        }
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
