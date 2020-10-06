using UnityEngine;
using UnityEngine.UI;

namespace ProjectBS.UI
{
    public class MainMenuUI_CharacterPanel : MainMenuUI_PanelBase
    {
        private enum PanelType
        {
            Party,
            AllCharacter,
            Equipment,
            Skill
        }

        [System.Serializable]
        private class PanelData
        {
            public PanelType panelType = PanelType.Party;
            public Button topButton = null;
            public GameObject[] panelObjs = null;
        }

        [SerializeField] private GameObject m_characterPanelRoot = null;
        [SerializeField] private PanelData[] m_panelDatas = null;
        [Header("Buttons")]
        [SerializeField] private MainMenuUI_CharacterButton[] m_characterButtons = null;
        [SerializeField] private MainMenuUI_EquipmentButton[] m_equipmentButtons = null;
        [SerializeField] private MainMenuUI_SkillButton[] m_skillButtons = null;

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
                    UpdateAllButtonData(m_panelDatas[i].panelType);
                    m_panelDatas[i].topButton.interactable = false;
                    for (int j = 0; j < m_panelDatas[i].panelObjs.Length; j++)
                    {
                        m_panelDatas[i].panelObjs[j].SetActive(true);
                    }
                    return;
                }
            }
        }

        private void UpdateAllButtonData(PanelType panelType)
        {
            switch(panelType)
            {
                case PanelType.Party:
                    {
                        for(int i = 0; i < m_characterButtons.Length; i++)
                        {
                            m_characterButtons[i].SetUp(PlayerManager.Instance.GetCharacterByPartyIndex(i));
                        }
                        break;
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
