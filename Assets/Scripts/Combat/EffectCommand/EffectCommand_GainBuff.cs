using System;
using KahaGameCore.Static;
using ProjectBS.Data;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_GainBuff : EffectCommandBase
    {
        private int m_buffID = 0;
        private int m_buffTime = 0;
        private List<CombatUnit> m_targets = null;
        private int m_currentActiveTargetIndex = -1;

        private Action m_onCompleted = null;

        public override void Process(string[] vars, Action onCompleted)
        {
            m_buffID = int.Parse(vars[1]);
            m_buffTime = int.Parse(vars[2]);
            m_onCompleted = onCompleted;

            CombatTargetSelecter.Instance.StartSelect(
                new CombatTargetSelecter.SelectTargetData
                {
                    attacker = processData.caster,
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
            GetPage<UI.CombatUIView>().RefreshAllInfo();

            m_currentActiveTargetIndex++;
            if(m_currentActiveTargetIndex >= m_targets.Count)
            {
                m_onCompleted?.Invoke();
                return;
            }

            CombatUnit.Buff _buff = m_targets[m_currentActiveTargetIndex].buffs.Find(x => x.soruceID == m_buffID);
            BuffData _skillEffectData = GameDataManager.GetGameData<BuffData>(m_buffID);

            if (_buff != null)
            {
                if (_skillEffectData.MaxStackCount > _buff.stackCount)
                {
                    _buff.stackCount++;
                }
                if(_buff.remainingTime < m_buffTime)
                {
                    _buff.remainingTime = m_buffTime;
                }
            }
            else
            {
                _buff = new CombatUnit.Buff
                {
                    soruceID = m_buffID,
                    from = processData.caster,
                    owner = m_targets[m_currentActiveTargetIndex],
                    remainingTime = m_buffTime,
                    stackCount = 1
                };

                m_targets[m_currentActiveTargetIndex].buffs.Add(_buff);
            }

            EffectProcessManager.GetBuffProcesser(_skillEffectData.ID).Start(
                new EffectProcesser.ProcessData
                {
                    caster = m_targets[m_currentActiveTargetIndex],
                    target = null,
                    allEffectProcesser = processData.allEffectProcesser,
                    timing = EffectProcesser.TriggerTiming.OnActived,
                    referenceBuff = _buff,
                    refenceSkill = null,
                    onEnded = delegate { DisplayGainBuff(_skillEffectData); }
                });
        }

        private void DisplayGainBuff(BuffData _skillEffectData)
        {
            GetPage<UI.CombatUIView>().DisplayGainBuff(new UI.CombatUIView.DisplayBuffData
            {
                taker = m_targets[m_currentActiveTargetIndex],
                buffName = ContextConverter.Instance.GetContext(_skillEffectData.NameContextID)
            }, SetNextTargetBuff);
        }
    }
}
