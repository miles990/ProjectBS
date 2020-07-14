using KahaGameCore.Interface;
using System.Collections.Generic;
using ProjectBS.Data;

namespace ProjectBS.Combat
{
    public class CombatState_StartCombat : StateBase
    {
        private List<CombatManager.CombatUnit> m_allUnits = null;

        public CombatState_StartCombat(List<CombatManager.CombatUnit> allUnits)
        {
            m_allUnits = allUnits;
        }

        protected override void OnStart()
        {
            for(int i = 0; i < m_allUnits.Count; i++)
            {
                // set ui
                string _startWith = "";
                if (m_allUnits[i].camp == CombatManager.CombatUnit.Camp.Player)
                    _startWith = "Character";
                else
                    _startWith = "Boss";
                UnityEngine.Debug.LogFormat(_startWith + " {0}, HP={1}, SP={2}", m_allUnits[i].name, m_allUnits[i].rawHP, m_allUnits[i].rawSP);

                CombatManager.Instance.ProcessAllEquipmentEffects(EffectProcesser.TriggerTiming.OnBattleStarted, null);
                // skills
            }
        }

        protected override void OnStop()
        {
        }

        protected override void OnTick()
        {
        }

    }
}

