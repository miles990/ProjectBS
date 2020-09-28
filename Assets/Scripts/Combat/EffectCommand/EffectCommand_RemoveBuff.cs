using System;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_RemoveBuff : EffectCommandBase
    {
        private int m_effectID = 0;
        private int m_removeStackCount = 0;
        private List<CombatUnit> m_targets = null;
        private int m_currentTargetIndex = -1;
        private CombatUnit.Buff m_currentBuff = null;
        private Action m_onCompleted = null;

        public override void Process(string[] vars, Action onCompleted)
        {
            m_effectID = int.Parse(vars[1]);

            m_removeStackCount = int.Parse(vars[2]);

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
            m_currentTargetIndex = -1;
            GoNextTaget();
        }

        private void GoNextTaget()
        {
            GetPage<UI.CombatUIView>().RefreshAllInfo();

            m_currentTargetIndex++;
            if(m_currentTargetIndex >= m_targets.Count)
            {
                m_onCompleted?.Invoke();
                return;
            }

            m_currentBuff = m_targets[m_currentTargetIndex].buffs.Find(x => x.effectID == m_effectID);
            if (m_currentBuff != null)
            {
                if (m_removeStackCount == -1)
                {
                    StartRemoveBuff();
                }
                else
                {
                    m_currentBuff.stackCount -= m_removeStackCount;
                    if(m_currentBuff.stackCount <= 0)
                    {
                        StartRemoveBuff();
                    }
                }
            }
            else
            {
                GoNextTaget();
            }
        }

        private void StartRemoveBuff()
        {
            Data.SkillEffectData _effect = KahaGameCore.Static.GameDataManager.GetGameData<Data.SkillEffectData>(m_currentBuff.effectID);

            EffectProcessManager.GetSkillEffectProcesser(_effect.ID).Start(new EffectProcesser.ProcessData
            {
                caster = processData.caster,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnDeactived,
                allEffectProcesser = processData.allEffectProcesser,
                referenceBuff = m_currentBuff,
                refenceSkill = null,
                onEnded = delegate { OnBuffRemoved(_effect); }
            });
        }

        private void OnBuffRemoved(Data.SkillEffectData _effect)
        {
            CombatUtility.RemoveEffect(m_targets[m_currentTargetIndex], _effect.ID);
            m_targets[m_currentTargetIndex].buffs.Remove(m_currentBuff);

            GetPage<UI.CombatUIView>().DisplayRemoveBuff(new UI.CombatUIView.DisplayBuffData
            {
                buffName = ContextConverter.Instance.GetContext(_effect.NameContextID),
                taker = m_targets[m_currentTargetIndex]
            }, GoNextTaget);
        }
    }
}
