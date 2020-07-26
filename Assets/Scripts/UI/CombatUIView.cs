using ProjectBS.Combat;
using KahaGameCore.Interface;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBS.UI
{
    public class CombatUIView : UIView
    {
        public enum SelectType
        {
            SameSide,
            Opponent,
            All
        }

        public override bool IsShowing => throw new NotImplementedException();

        public event Action<int> OnSkillSelected = null;

        public class SelectTargetData
        {
            public SelectType selectType = SelectType.All;
            public CombatUnit attacker = null;
            public int needCount = 0;
            public bool random = false;
        }

        // 0~4:Player 5~9:Boss
        private Dictionary<int, CombatUnit> m_indexToCombatUnit = new Dictionary<int, CombatUnit>();
        private Dictionary<int, bool> m_indexToEnableState = new Dictionary<int, bool>();

        private SelectTargetData m_currentSelectData = null;
        private event Action<CombatUnit> OnUnitSelected = null;
        private List<CombatUnit> m_currentTargets = new List<CombatUnit>();

        public override void ForceShow(Manager manager, bool show)
        {
            throw new NotImplementedException();
        }

        public override void Show(Manager manager, bool show, Action onCompleted)
        {
            onCompleted?.Invoke();
        }

        public void InitBattleUnits(List<CombatUnit> units)
        {
            for(int i = 0; i < units.Count; i++)
            {
                Debug.Log("Set Unit------------------------------");
                Debug.LogFormat("units[{0}].camp={1}", i, units[i].camp.ToString());
                Debug.LogFormat("units[{0}].name={1}", i , units[i].name);
                Debug.LogFormat("units[{0}].hp={1}", i, units[i].HP);
                Debug.LogFormat("units[{0}].sp={1}", i, units[i].SP);
                Debug.Log("------------------------------");

                m_indexToCombatUnit.Add(i, units[i]);
                m_indexToEnableState.Add(i, false);
            }
        }

        public void RefreshCurrentSkillMenu(CombatUnit actor)
        {
            Debug.Log("------------------------------");
            Debug.Log("Current Actor:" + actor.name);

            string[] _skillIDs = actor.skills.Split(',');
            for(int i = 0; i < _skillIDs.Length; i++)
            {
                Debug.LogFormat("Skill {0}: ContextID={1}", i, GameDataLoader.Instance.GetSkill(_skillIDs[i]).NameContextID);
            }
        }

        public void StartSelectTarget(SelectTargetData data)
        {
            if (m_currentSelectData != null)
                return;

            m_currentTargets.Clear();
            m_currentSelectData = data;

            switch(m_currentSelectData.selectType)
            {

            }

            OnUnitSelected += OnUnitButtonPressed;
        }

        public void Button_SelectSkill(int index)
        {
            OnSkillSelected?.Invoke(index);
        }

        public void Button_SelectCharacter(int index)
        {
            if(!m_indexToEnableState[index])
            {
                return;
            }

            OnUnitSelected?.Invoke(m_indexToCombatUnit[index]);
        }

        private void OnUnitButtonPressed(CombatUnit unit)
        {
            m_currentTargets.Add(unit);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                Button_SelectSkill(0);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Button_SelectSkill(1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Button_SelectSkill(2);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Button_SelectSkill(3);
            }
        }
    }
}
