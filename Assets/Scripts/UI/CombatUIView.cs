﻿using ProjectBS.Combat;
using KahaGameCore.Interface;
using System;
using System.Collections.Generic;
using UnityEngine;
using KahaGameCore.Static;
using Photon.Pun;

namespace ProjectBS.UI
{
    public class CombatUIView : UIView
    {
        public const float DISPLAY_INFO_TIME = 1f;

        public override bool IsShowing { get { return m_root.activeSelf; } }

        public event Action<Data.SkillData> OnSkillSelected = null;

        public class SelectTargetData
        {
            public CombatTargetSelecter.SelectRange selectRange = CombatTargetSelecter.SelectRange.All;
            public CombatUnit attacker = null;
            public int needCount = 0;
            public bool inculdeAttacker = false;
            public Action<List<CombatUnit>> onSelected = null;
        }

        [SerializeField] private GameObject m_root = null;
        [SerializeField] private CombatUI_CharacterPanel[] m_characterPanels = null;
        [SerializeField] private GameObject m_skillPanel = null;
        [SerializeField] private CombatUI_SelectSkillButton[] m_skillButtons = null;
        [SerializeField] private CombatUI_InfoText m_infoPrefab = null;
        [SerializeField] private UnityEngine.UI.Text m_actionSortInfoText = null;
        [Header("Network")]
        [SerializeField] private PhotonView m_photonView = null;

        private List<Data.SkillData> m_currentShowingSkills = null;

        // 0~3:Player 4~8:Enemy
        private Dictionary<int, CombatUnit> m_indexToUnit = new Dictionary<int, CombatUnit>();
        private Dictionary<CombatUnit, int> m_unitToIndex = new Dictionary<CombatUnit, int>();

        private List<CombatUnit> m_allUnits = new List<CombatUnit>();

        private SelectTargetData m_currentSelectData = null;
        private List<CombatUnit> m_currentSelectedTargets = new List<CombatUnit>();
        private Action<List<CombatUnit>> m_onSelected = null;

        private void Start()
        {
            for(int i = 0; i < m_skillButtons.Length; i++)
            {
                m_skillButtons[i].OnSelected += Button_SelectSkill;
                m_skillButtons[i].OnShownDetailCommanded += Button_ShowSkillInfo;
            }
        }

        private void Update()
        {
            RefreshAllInfo();
        }

        public override void ForceShow(Manager manager, bool show)
        {
            throw new NotImplementedException();
        }

        public override void Show(Manager manager, bool show, Action onCompleted)
        {
            m_root.SetActive(show);
            m_actionSortInfoText.gameObject.SetActive(show);
            for(int i = 0; i < m_characterPanels.Length; i++)
            {
                m_characterPanels[i].EnableActingHint(false);
            }
            onCompleted?.Invoke();
        }

        public void InitBattleUnits(List<CombatUnit> units)
        {
            for(int i = 0; i < m_characterPanels.Length; i++)
            {
                m_characterPanels[i].gameObject.SetActive(false);
            }

            m_allUnits.Clear();
            int _currentPlayerIndex = 0;
            int _currentEnemyIndex = 4;

            for (int i = 0; i < units.Count; i++)
            { 
                if (units[i].camp == 0)
                {
                    if(m_indexToUnit.ContainsKey(_currentPlayerIndex))
                    {
                        m_indexToUnit[_currentPlayerIndex] = units[i];
                    }
                    else
                    {
                        m_indexToUnit.Add(_currentPlayerIndex, units[i]);
                    }

                    if(m_unitToIndex.ContainsKey(units[i]))
                    {
                        m_unitToIndex[units[i]] = _currentPlayerIndex;
                    }
                    else
                    {
                        m_unitToIndex.Add(units[i], _currentPlayerIndex);
                    }
                    m_characterPanels[_currentPlayerIndex].SetUp(units[i].UDID);
                    m_characterPanels[_currentPlayerIndex].gameObject.SetActive(true);
                    _currentPlayerIndex++;
                }
                else
                {
                    if (m_indexToUnit.ContainsKey(_currentEnemyIndex))
                    {
                        m_indexToUnit[_currentEnemyIndex] = units[i];
                    }
                    else
                    {
                        m_indexToUnit.Add(_currentEnemyIndex, units[i]);
                    }

                    if (m_unitToIndex.ContainsKey(units[i]))
                    {
                        m_unitToIndex[units[i]] = _currentEnemyIndex;
                    }
                    else
                    {
                        m_unitToIndex.Add(units[i], _currentEnemyIndex);
                    }

                    m_characterPanels[_currentEnemyIndex].SetUp(units[i].UDID);
                    m_characterPanels[_currentEnemyIndex].gameObject.SetActive(true);
                    _currentEnemyIndex++;
                }
            }

            m_allUnits = new List<CombatUnit>(units);
        }

        public void RefreshActionQueueInfo(List<CombatUnitAction> actions)
        {
            string _info = "";
            for (int i = 0; i < actions.Count; i++)
            {
                _info += actions[i].Actor.name;
                _info += "\n";
            }

            if(Network.PhotonManager.Instance.IsConnected)
            {
                m_photonView.RPC(nameof(Sync_RefreshActionQueueInfo), RpcTarget.All, _info);
            }
            else
            {
                Sync_RefreshActionQueueInfo(_info);
            }
        }

        [PunRPC]
        private void Sync_RefreshActionQueueInfo(string info)
        {
            m_actionSortInfoText.text = info;
        }

        public void RemoveActor(CombatUnit unit)
        {
            if (Network.PhotonManager.Instance.IsConnected)
            {
                m_photonView.RPC(nameof(Sync_RemoveActor), RpcTarget.All, unit.UDID);
            }
            else
            {
                Sync_RemoveActor(unit.UDID);
            }
        }

        [PunRPC]
        private void Sync_RemoveActor(string UDID)
        {
            CombatUnit _unit = m_allUnits.Find(x => x.UDID == UDID);
            m_characterPanels[m_unitToIndex[_unit]].gameObject.SetActive(false);
            RefreshAllInfo();
        }

        public void ShowForceEndAction(CombatUnit actor)
        {
            SetInfoText(actor, string.Format("強制結束", actor.name));
        }

        public void ShowUnitDied(CombatUnit dyingUnit, Action onInfoShown)
        {
            SetInfoText(dyingUnit, string.Format("死亡"));
            TimerManager.Schedule(DISPLAY_INFO_TIME, onInfoShown);
        }

        public void ShowUnitDestoryed(CombatUnit unit, Action onInfoShown)
        {
            SetInfoText(unit, string.Format("消滅"));
            TimerManager.Schedule(DISPLAY_INFO_TIME, onInfoShown);
        }

        public void ShowTurnStart(int turnCount, Action onTurnStartAnimationEnded)
        {
            SetInfoText(null, string.Format("第 {0} 回合", turnCount));
            TimerManager.Schedule(DISPLAY_INFO_TIME, onTurnStartAnimationEnded);
        }

        public void ShowActorActionStart(CombatUnit actor, Action onActionAnimationEnded)
        {
            if(Network.PhotonManager.Instance.IsConnected)
            {
                m_photonView.RPC(nameof(Sync_EnableActingHint), RpcTarget.All, actor.UDID, true);
            }
            else
            {
                m_characterPanels[m_unitToIndex[actor]].EnableActingHint(true);
            }

            SetInfoText(actor, string.Format("開始行動"));
            TimerManager.Schedule(1f, onActionAnimationEnded);
        }

        public void ShowActorActionEnd(CombatUnit actor, Action onActionAnimationEnded)
        {
            if (Network.PhotonManager.Instance.IsConnected)
            {
                m_photonView.RPC(nameof(Sync_EnableActingHint), RpcTarget.All, actor.UDID, false);
            }
            else
            {
                m_characterPanels[m_unitToIndex[actor]].EnableActingHint(false);
            }

            onActionAnimationEnded?.Invoke();
        }

        [PunRPC]
        private void Sync_EnableActingHint(string UDID, bool enable)
        {
            CombatUnit _units = m_allUnits.Find(x => x.UDID == UDID);
            m_characterPanels[m_unitToIndex[_units]].EnableActingHint(enable);
        }

        public void ShowAddActionIndex(CombatUnit actor, int addIndex, Action onShown)
        {
            SetInfoText(actor, string.Format("{0} 順序", addIndex >= 0 ? "+" + addIndex : addIndex.ToString()));
            TimerManager.Schedule(1f, onShown);
        }

        public void ShowAddExtraAction(CombatUnit actor, Action onShown)
        {
            SetInfoText(actor, string.Format("追加行動"));
            TimerManager.Schedule(1f, onShown);
        }

        public class SkillAnimationData
        {
            public CombatUnit caster = null;
            public int nameContextID = 0;
            public Action onEnded = null;
        }

        public void ShowSkillAnimation(SkillAnimationData skillAnimationData)
        {
            if(skillAnimationData.nameContextID == 0)
            {
                skillAnimationData.onEnded?.Invoke();
                return;
            }

            SetInfoText(skillAnimationData.caster, ContextConverter.Instance.GetContext(skillAnimationData.nameContextID));
            TimerManager.Schedule(DISPLAY_INFO_TIME, skillAnimationData.onEnded);
        }

        public void ShowGameEnd(bool playerWin)
        {
            m_skillPanel.SetActive(false);
            for (int i = 0; i < m_characterPanels.Length; i++)
            {
                m_characterPanels[i].gameObject.SetActive(false);
            }
            TimerManager.Schedule(1.5f, 
                delegate
                {
                    CombatUtility.ComabtManager.EndComabat(playerWin);
                });
        }

        public void RefreshCurrentSkillMenu(List<Data.SkillData> datas)
        {
            for(int i = 0; i < m_skillButtons.Length; i++)
            {
                m_skillButtons[i].gameObject.SetActive(false);
            }
            m_currentShowingSkills = datas;
            for(int i = 0; i < m_currentShowingSkills.Count; i++)
            {
                if (!m_currentShowingSkills[i].Command.Contains(EffectProcesser.TriggerTiming.OnActived.ToString()))
                {
                    m_skillButtons[i].EnableButton(false);
                }
                else
                {
                    m_skillButtons[i].EnableButton(true);
                }
                m_skillButtons[i].SetUp(ContextConverter.Instance.GetContext(m_currentShowingSkills[i].NameContextID), m_currentShowingSkills[i].SP);
                m_skillButtons[i].gameObject.SetActive(true);
            }
            m_skillPanel.SetActive(true);
        }

        public void StartSelectTarget(SelectTargetData data)
        {
            if (m_currentSelectData != null)
                return;

            m_currentSelectedTargets.Clear();
            m_currentSelectData = data;
            m_onSelected = data.onSelected;

            WaitPlayerSelect();
        }

        public void Button_SelectSkill(int index)
        {
            if (m_currentShowingSkills == null
                || m_currentShowingSkills.Count <= index
                || m_currentShowingSkills[index] == null)
                return;

            Data.SkillData _selectedSkill = m_currentShowingSkills[index];

            m_currentShowingSkills = null;
            m_skillPanel.SetActive(false);

            OnSkillSelected?.Invoke(_selectedSkill);
        }

        public void Button_ShowSkillInfo(int index)
        {
            if (m_currentShowingSkills == null
            || m_currentShowingSkills.Count <= index
            || m_currentShowingSkills[index] == null)
                return;

            Data.SkillData _selectedSkill = m_currentShowingSkills[index];

            GameManager.Instance.MessageManager.ShowCommonMessage(
                _selectedSkill.GetAllDescriptionContext(),
                ContextConverter.Instance.GetContext(_selectedSkill.NameContextID), null);
        }

        public void Button_SelectCharacter(int index)
        {
            if(!m_indexToUnit.ContainsKey(index))
            {
                return;
            }

            m_currentSelectedTargets.Add(m_indexToUnit[index]);

            if(m_currentSelectedTargets.Count < m_currentSelectData.needCount)
            {
                WaitPlayerSelect();
            }
            else
            {
                EnableSelectBossButton(false);
                EnableSelectPlayerButton(false);

                m_currentSelectData = null;
                m_onSelected?.Invoke(m_currentSelectedTargets);
            }
        }

        private void RefreshAllInfo()
        {
            for(int i = 0; i < m_characterPanels.Length; i++)
            {
                if(m_indexToUnit.ContainsKey(i))
                {
                    m_characterPanels[i].RefreshInfo();
                }
            }
        }

        public void Button_SkipAction()
        {
            m_skillPanel.SetActive(false);
            CombatUtility.ComabtManager.ForceEndCurrentAction();
        }

        public class DisplayDamageData
        {
            public CombatUnit taker = null;
            public int damageValue = 0;
        }

        public void DisplayDamage(DisplayDamageData data, Action onDisplayEnded)
        {
            SetInfoText(data.taker, "-" + data.damageValue.ToString());
            TimerManager.Schedule(DISPLAY_INFO_TIME, onDisplayEnded);
        }

        public class DisplayHealData
        {
            public CombatUnit taker = null;
            public int healValue = 0;
        }

        public void DisplayHeal(DisplayHealData data, Action onDisplayEnded)
        {
            SetInfoText(data.taker, "+" + data.healValue.ToString());
            TimerManager.Schedule(DISPLAY_INFO_TIME, onDisplayEnded);
        }

        public class DisplayBuffData
        {
            public CombatUnit taker = null;
            public string buffName = "Unknown Buff";
        }

        public void DisplayGainBuff(DisplayBuffData data, Action onDisplayEnded)
        {
            SetInfoText(data.taker, "+" + data.buffName);
            TimerManager.Schedule(DISPLAY_INFO_TIME, onDisplayEnded);
        }

        public void DisplayRemoveBuff(DisplayBuffData data, Action onDisplayEnded)
        {
            SetInfoText(data.taker, "-" + data.buffName);
            TimerManager.Schedule(DISPLAY_INFO_TIME, onDisplayEnded);
        }

        private void SetInfoText(CombatUnit target, string info)
        {
            if (target != null && !m_unitToIndex.ContainsKey(target))
                return;

            if(Network.PhotonManager.Instance.IsConnected)
            {
                m_photonView.RPC(nameof(Sync_SetInfoText), RpcTarget.All, target.UDID, info);
            }
            else
            {
                Sync_SetInfoText(target != null ? target.UDID : "null", info); 
            }
        }

        [PunRPC]
        private void Sync_SetInfoText(string UDID, string info)
        {
            CombatUnit _target = m_allUnits.Find(x => x.UDID == UDID);

            CombatUI_InfoText _clone = Instantiate(m_infoPrefab);
            _clone.transform.SetParent(transform);
            if (_target == null)
                _clone.transform.localPosition = Vector3.zero;
            else
                _clone.transform.position = m_characterPanels[m_unitToIndex[_target]].transform.position;
            _clone.SetText(string.Format(info));
            _clone.gameObject.SetActive(true);

            Destroy(_clone.gameObject, DISPLAY_INFO_TIME);
        }

        private void WaitPlayerSelect()
        {
            switch (m_currentSelectData.selectRange)
            {
                case CombatTargetSelecter.SelectRange.All:
                    {
                        EnableSelectBossButton(true);
                        EnableSelectPlayerButton(true);
                        break;
                    }
                case CombatTargetSelecter.SelectRange.Opponent:
                    {
                        if (m_currentSelectData.attacker.camp == 1)
                        {
                            EnableSelectPlayerButton(true);
                        }
                        else
                        {
                            EnableSelectBossButton(true);
                        }
                        break;
                    }
                case CombatTargetSelecter.SelectRange.SameSide:
                    {
                        if (m_currentSelectData.attacker.camp == 1)
                        {
                            EnableSelectBossButton(true);
                        }
                        else
                        {
                            EnableSelectPlayerButton(true);
                        }
                        break;
                    }
            }
        }

        private void EnableSelectBossButton(bool enable)
        {
            for(int i = 4; i < 9; i++)
            {
                if(m_indexToUnit.ContainsKey(i))
                {
                    if(!m_currentSelectData.inculdeAttacker && m_indexToUnit[i] == m_currentSelectData.attacker)
                    {
                        continue;
                    }
                    m_characterPanels[i].SetEnable(m_indexToUnit[i].HP > 0 ? enable : false);
                }
            }
        }

        private void EnableSelectPlayerButton(bool enable)
        {
            for (int i = 0; i < 4; i++)
            {
                if (m_indexToUnit.ContainsKey(i))
                {
                    if (!m_currentSelectData.inculdeAttacker && m_indexToUnit[i] == m_currentSelectData.attacker)
                    {
                        continue;
                    }
                    m_characterPanels[i].SetEnable(m_indexToUnit[i].HP > 0 ? enable : false);
                }
            }
        }
    }
}
