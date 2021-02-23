using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace ProjectBS.UI
{
    public class MainMenuUI_BossButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] private TextMeshProUGUI m_bossNameText = null;
        [SerializeField] private TextMeshProUGUI m_bossDescriptionText = null;
        [SerializeField] private TextMeshProUGUI m_levelText = null;

        private Data.BossStageData m_refBossStageData = null;
        private float m_showInfoTimer = 0f;

        public void SetUp(Data.BossStageData bossStageData)
        {
            m_refBossStageData = bossStageData;
            Data.BossData _mainBoss = GameDataManager.GetGameData<Data.BossData>(m_refBossStageData.MainBossID);
            m_bossNameText.text = _mainBoss.GetName();
            if(PlayerManager.Instance.Player.ClearedBossStage.Contains(bossStageData.ID))
            {
                m_bossNameText.text += " (Cleared)";
            }
            m_levelText.text = m_refBossStageData.Stamina.ToString();
            m_bossDescriptionText.text = ContextConverter.Instance.GetContext(m_refBossStageData.DescriptionContextID);
        }

        private void Button_SelectBoss()
        {
            GameManager.Instance.StartLocalCombat(m_refBossStageData);
        }

        private void Button_ShowInfo()
        {
            GameManager.Instance.MessageManager.ShowCommonMessage(
                ContextConverter.Instance.GetContext(m_refBossStageData.DescriptionContextID),
                m_bossNameText.text, null);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            m_showInfoTimer = GameDataManager.GameProperties.PressDownShowInfoTime;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (m_showInfoTimer > 0f)
            {
                Button_SelectBoss();
            }

            m_showInfoTimer = 0f;
        }

        private void Update()
        {
            if (m_showInfoTimer > 0f)
            {
                m_showInfoTimer -= Time.deltaTime;
                if (m_showInfoTimer <= 0f)
                {
                    Button_ShowInfo();
                    m_showInfoTimer = 0f;
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_showInfoTimer = 0f;
        }
    }
}