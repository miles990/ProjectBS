using KahaGameCore.Static;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectBS.UI
{
    public class MainMenuUI_SelectBossPanel : MainMenuUI_PanelBase
    {
        [SerializeField] private MainMenuUI_BossButton m_bossButtonPrefab = null;
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
                    m_clonedButtons.Add(_cloneButton);
                }

                if(i == 0 && PlayerManager.Instance.Player.ClearedBossStage.Count == 0)
                {
                    break;
                }
            }
        }
    }
}
