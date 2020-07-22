using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBS.Combat
{
    public class CombatUnitEffectProcesser
    {
        private List<CombatUnit> m_units = null;
        private Action m_onEnded = null;
        private EffectProcesser.TriggerTiming m_timing = EffectProcesser.TriggerTiming.OnActived;

        private int m_currentUnitIndex = -1;
        private int m_currentEquipment = -1;
        private string[] m_currentEquipmentEffectIDs = null;
        private int m_currentEquipmentEffectIDIndex = -1;
        private int m_currentBuffIndex = -1;

        public CombatUnitEffectProcesser(List<CombatUnit> units)
        {
            m_units = new List<CombatUnit>(units);
        }

        public void Start(EffectProcesser.TriggerTiming timing, Action onEnded)
        {
            m_onEnded = onEnded;
            m_timing = timing;
            m_currentUnitIndex = -1;
        }

        private void GoNextUnit()
        {
            m_currentUnitIndex++;
            if(m_currentUnitIndex >= m_units.Count)
            {
                m_onEnded?.Invoke();
                return;
            }

            m_currentEquipment = -1;
            GoNextEquipment();
        }

        private void GoNextEquipment()
        {
            m_currentEquipment++;
            if (m_currentEquipment >= 4)
            {
                m_currentBuffIndex = -1;
                GoNextOwingBuff();
                return;
            }

            switch (m_currentEquipment)
            {
                case 0:
                    {
                        m_currentEquipmentEffectIDs = m_units[m_currentUnitIndex].head.EffectIDs.Split('$');
                        break;
                    }
                case 1:
                    {
                        m_currentEquipmentEffectIDs = m_units[m_currentUnitIndex].body.EffectIDs.Split('$');
                        break;
                    }
                case 2:
                    {
                        m_currentEquipmentEffectIDs = m_units[m_currentUnitIndex].hand.EffectIDs.Split('$');
                        break;
                    }
                case 3:
                    {
                        m_currentEquipmentEffectIDs = m_units[m_currentUnitIndex].foot.EffectIDs.Split('$');
                        break;
                    }
            }

            m_currentEquipmentEffectIDIndex = -1;
        }

        private void GoNextEquipmentEffect()
        {
            m_currentEquipmentEffectIDIndex++;
            if(m_currentEquipmentEffectIDIndex >= m_currentEquipmentEffectIDs.Length)
            {
                GoNextEquipment();
                return;
            }

            string _command = GameDataLoader.Instance.GetSkillEffect(m_currentEquipmentEffectIDs[m_currentEquipmentEffectIDIndex]).Command;
            new EffectProcesser(_command.RemoveBlankCharacters()).Start(
                new EffectProcesser.ProcessData
                {
                    caster = m_units[m_currentUnitIndex],
                    target = m_units[m_currentUnitIndex],
                    timing = m_timing,
                    onEnded = GoNextEquipmentEffect
                });
        }

        private void GoNextOwingBuff()
        {
            m_currentBuffIndex++;
            if(m_currentBuffIndex >= m_units[m_currentUnitIndex].buffs.Count)
            {
                return;
            }

            // ...
        }
    }
}
