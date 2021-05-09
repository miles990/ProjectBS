using ProjectBS.Data;
using System;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_GainBuff : EffectCommandBase
    {
        private int m_buffID = 0;
        private string m_buffAmountCrossBuffTime = "";
        private List<CombatUnit> m_targets = null;
        private int m_currentActiveTargetIndex = -1;

        private Action m_onCompleted = null;

        public override void Process(string[] vars, Action onCompleted)
        {
            m_buffID = int.Parse(vars[1]);
            m_buffAmountCrossBuffTime = vars[2];
            m_onCompleted = onCompleted;

            AddSkillOrEffectInfo();

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
            m_targets = targets;
            m_currentActiveTargetIndex = -1;
            SetNextTargetBuff();
        }

        private void SetNextTargetBuff()
        {
            m_currentActiveTargetIndex++;
            if(m_currentActiveTargetIndex >= m_targets.Count)
            {
                m_onCompleted?.Invoke();
                return;
            }

            CombatUnit.Buff _buff = m_targets[m_currentActiveTargetIndex].GetBuffByBuffID(m_buffID);
            BuffData _buffData = GameDataManager.GetGameData<BuffData>(m_buffID);

            GetPage<UI.CombatUIView>().AddCombatInfo
                (
                    string.Format
                    (
                        ContextConverter.Instance.GetContext(500025),
                        m_targets[m_currentActiveTargetIndex].name,
                        ContextConverter.Instance.GetContext(_buffData.NameContextID)
                    ), null
                );

            if (_buffData == null)
            {
                throw new Exception(string.Format("[EffectCommand_GainBuff][SetNextTargetBuff] invaild buff id={0}, ref skill id={1}, ref buff id=",
                    m_buffID,
                    processData.refenceSkill == null ? "null" : processData.refenceSkill.skill.ID.ToString(),
                    processData.referenceBuff == null ? "null" : processData.referenceBuff.GetBuffSourceData().ID.ToString()));
            }

            string[] _addBuffInfos = m_buffAmountCrossBuffTime.Split('t');
            int _addBuffAmount = int.Parse(_addBuffInfos[0]);
            int _addBuffTime = int.Parse(_addBuffInfos[1]);

            if (_buff != null)
            {
                _buff.amount += _addBuffAmount;
                if(_buff.amount > _buff.GetBuffSourceData().MaxAmount)
                {
                    _buff.amount = _buff.GetBuffSourceData().MaxAmount;
                }

                if (_buff.remainingTime != -1 && _buff.remainingTime < _addBuffTime)
                {
                    _buff.remainingTime = _addBuffTime;
                }
            }
            else
            {
                _buff = new CombatUnit.Buff
                {
                    soruceID = m_buffID,
                    fromUnitUDID = processData.caster.UDID,
                    ownerUnitUDID = m_targets[m_currentActiveTargetIndex].UDID,
                    remainingTime = _addBuffTime,
                    amount = _addBuffAmount
                };
                if (_buff.amount > _buff.GetBuffSourceData().MaxAmount)
                {
                    _buff.amount = _buff.GetBuffSourceData().MaxAmount;
                }

                m_targets[m_currentActiveTargetIndex].AddBuff(_buff);
            }

            GetPage<UI.CombatUIView>().DisplayGainBuff(new UI.CombatUIView.DisplayBuffData
            {
                taker = m_targets[m_currentActiveTargetIndex],
                buffName = ContextConverter.Instance.GetContext(_buffData.NameContextID),
                amount = _addBuffAmount
            }, delegate { DoActivedCommand(_buffData, _buff); });
        }

        private void DoActivedCommand(BuffData _buffData, CombatUnit.Buff _buff)
        {
            EffectProcessManager.GetBuffProcesser(_buffData.ID).Start(
                new EffectProcesser.ProcessData
                {
                    caster = m_targets[m_currentActiveTargetIndex],
                    target = null,
                    allEffectProcesser = processData.allEffectProcesser,
                    timing = EffectProcesser.TriggerTiming.OnActived,
                    referenceBuff = _buff,
                    refenceSkill = null,
                    onEnded = SetNextTargetBuff
                });
        }
    }
}
