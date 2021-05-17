using KahaGameCore.Static;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectBS.UI
{
    public class MainMenuUI_SelectBossPanel : MainMenuUI_PanelBase
    {
        [SerializeField] private MainMenuUI_BossButton m_bossButtonPrefab = null;
        [SerializeField] private ScrollRect m_bossButtonScrollRect = null;
        [SerializeField] private RectTransform m_buttonContainer = null;

        private List<MainMenuUI_BossButton> m_clonedButtons = new List<MainMenuUI_BossButton>();

        protected override void OnShown()
        {
            RefreshBossButtons();
        }

        protected override void OnHidden()
        {
        }

        private void RefreshBossButtons()
        {
            Data.BossStageData[] _stages = GameDataManager.GetAllGameData<Data.BossStageData>();
        
            if(_stages == null)
            {
                throw new System.Exception("[MainMenuUI_SelectBossPanel][RefreshBossButtons] _stages is null");
            }

            for (int i = 0; i < m_clonedButtons.Count; i++)
            {
                m_clonedButtons[i].gameObject.SetActive(false);
            }

            List<Data.BossStageData> _allStage = new List<Data.BossStageData>(_stages);

            for (int i = 0; i < _allStage.Count; i++)
            {
                if (_allStage[i].Index == -1)
                {
                    _allStage.RemoveAt(i);
                    i--;
                }
            }
            _allStage.Sort((x, y) => x.Index.CompareTo(y.Index));

            for (int i = 0; i < _allStage.Count; i++)
            {
                bool _skip = false;
                if (!string.IsNullOrEmpty(_allStage[i].FrontStageIDs))
                {
                    string[] _allFrontStageID = _allStage[i].FrontStageIDs.Split(';');
                    for(int j = 0; j < _allFrontStageID.Length; j++)
                    {
                        if(!PlayerManager.Instance.Player.ClearedBossStage.Contains(_allFrontStageID[j].ToInt()))
                        {
                            _skip = true;
                            break;
                        }
                    }
                }

                if (_skip) continue;

                if (i < m_clonedButtons.Count)
                {
                    m_clonedButtons[i].SetUp(_allStage[i]);
                    m_clonedButtons[i].gameObject.SetActive(true);
                }
                else
                {
                    MainMenuUI_BossButton _cloneButton = Instantiate(m_bossButtonPrefab);
                    _cloneButton.transform.SetParent(m_buttonContainer);
                    _cloneButton.transform.localScale = Vector3.one;
                    _cloneButton.SetUp(_allStage[i]);
                    _cloneButton.refScrollRect = m_bossButtonScrollRect;
                    m_clonedButtons.Add(_cloneButton);
                }
            }
        }
    }
}
