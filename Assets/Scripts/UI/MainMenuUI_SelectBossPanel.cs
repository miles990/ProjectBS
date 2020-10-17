﻿using KahaGameCore.Static;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectBS.UI
{
    public class MainMenuUI_SelectBossPanel : MainMenuUI_PanelBase
    {
        [SerializeField] private MainMenuUI_BossButton[] m_bossButtons = null;
        [SerializeField] private Button m_nextPageButton = null;
        [SerializeField] private Button m_previousPageButton = null;

        private int m_currentPage = 0;

        public override void Hide()
        {
            gameObject.SetActive(false);
        }

        public override void Show()
        {
            gameObject.SetActive(true);
            m_currentPage = 0;
            RefreshBossButtons();
        }

        public void Button_GoNextPage()
        {
            m_currentPage++;
            RefreshBossButtons();
        }

        public void Button_GoPreviousPage()
        {
            m_currentPage--;
            m_currentPage = m_currentPage < 0 ? 0 : m_currentPage;
            RefreshBossButtons();
        }

        private void RefreshBossButtons()
        {
            List<Data.BossStageData> _allStage = new List<Data.BossStageData>(GameDataManager.GetAllGameData<Data.BossStageData>());
            for(int i = 0; i < _allStage.Count; i++)
            {
                if(_allStage[i].Index == -1)
                {
                    _allStage.RemoveAt(i);
                    i--;
                }
            }
            _allStage.Sort((x, y) => x.Index.CompareTo(y.Index));

            m_previousPageButton.interactable = m_currentPage != 0;
            m_nextPageButton.interactable = m_bossButtons.Length * (m_currentPage + 1) < _allStage.Count;

            for (int i = 0; i < m_bossButtons.Length; i++)
            {
                int _currentDisplayStageIndex = i + m_bossButtons.Length * m_currentPage;
                if(_currentDisplayStageIndex >= _allStage.Count)
                {
                    m_bossButtons[i].gameObject.SetActive(false);
                }
                else
                {
                    m_bossButtons[i].SetUp(_allStage[_currentDisplayStageIndex]);
                    m_bossButtons[i].gameObject.SetActive(true);
                }
            }
        }
    }
}