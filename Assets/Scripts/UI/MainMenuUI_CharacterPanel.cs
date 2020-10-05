using UnityEngine;
using UnityEngine.UI;

namespace ProjectBS.UI
{
    public class MainMenuUI_CharacterPanel : MainMenuUI_PanelBase
    {
        [System.Serializable]
        private struct PanelData
        {
            public Button topButton;
            public GameObject[] panelObjs;
        }

        [SerializeField] private GameObject m_characterPanelRoot = null;
        [SerializeField] private PanelData[] m_panelDatas = null;
        [Header("Character Button")]
        [SerializeField] private MainMenuUI_CharacterButton[] m_characterButtons = null;

        public override void Hide()
        {
            m_characterPanelRoot.SetActive(false);
        }

        public override void Show()
        {
            DisableAllPanel();
            m_characterPanelRoot.SetActive(true);
        }

        public void Button_Select(Button button)
        {
            DisableAllPanel();
            for (int i = 0; i < m_panelDatas.Length; i++)
            {
                if(m_panelDatas[i].topButton == button)
                {
                    m_panelDatas[i].topButton.interactable = false;
                    for (int j = 0; j < m_panelDatas[i].panelObjs.Length; j++)
                    {
                        m_panelDatas[i].panelObjs[j].SetActive(true);
                    }
                    return;
                }
            }
        }

        private void DisableAllPanel()
        {
            for (int i = 0; i < m_panelDatas.Length; i++)
            {
                m_panelDatas[i].topButton.interactable = true;
                for(int j = 0; j < m_panelDatas[i].panelObjs.Length; j++)
                {
                    m_panelDatas[i].panelObjs[j].SetActive(false);
                }
            }
        }
    }
}
